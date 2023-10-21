using System;
using System.Diagnostics.CodeAnalysis;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Editor.Cli;
using UnityEditor;
using UnityEngine;
using EditorGUI = UnityEditor.EditorGUI;

namespace SingularityGroup.HotReload.Editor {
    internal struct HotReloadSettingsTabState {
        public readonly bool running;
        public readonly bool trialLicense;
        public readonly LoginStatusResponse loginStatus;
        public readonly bool isServerHealthy;
        
        public HotReloadSettingsTabState(
            bool running,
            bool trialLicense,
            LoginStatusResponse loginStatus,
            bool isServerHealthy
        ) {
            this.running = running;
            this.trialLicense = trialLicense;
            this.loginStatus = loginStatus;
            this.isServerHealthy = isServerHealthy;
        }
    }
    
    internal class HotReloadSettingsTab : HotReloadTabBase {
        private readonly HotReloadOptionsSection optionsSection;

        // cached because changing built target triggers C# domain reload
        // Also I suspect selectedBuildTargetGroup has chance to freeze Unity for several seconds (unconfirmed).
        private readonly Lazy<BuildTargetGroup> currentBuildTarget = new Lazy<BuildTargetGroup>(
            () => BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));

        private readonly Lazy<bool> isCurrentBuildTargetSupported = new Lazy<bool>(() => {
            var target = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            return HotReloadBuildHelper.IsMonoSupported(target);
        });
        
        // Resources.Load uses cache, so it's safe to call it every frame.
        //  Retrying Load every time fixes an issue where you import the package and constructor runs, but resources aren't loadable yet.
        private Texture iconCheck => Resources.Load<Texture>("icon_check_circle");
        private Texture iconWarning => Resources.Load<Texture>("icon_warning_circle");

        [SuppressMessage("ReSharper", "Unity.UnknownResource")] // Rider doesn't check packages
        public HotReloadSettingsTab(HotReloadWindow window) : base(window,
            "Settings",
            "_Popup",
            "Make changes to a build running on-device.")
        {
            optionsSection = new HotReloadOptionsSection();
        }

        private GUIStyle headlineStyle;

        /*
         * EditorGUILayout.LabelField is designed to work with the Unity editor's inspector window,
         * which has its own layout and padding system. This can cause some issues with custom GUIStyles,
         * as the padding and layout settings of the custom style may not match the inspector window's settings.
         */

        HotReloadSettingsTabState currentState;
        public override void OnGUI() {
            // HotReloadAboutTabState ensures rendering is consistent between Layout and Repaint calls
            // Without it errors like this happen:
            // ArgumentException: Getting control 2's position in a group with only 2 controls when doing repaint
            // See thread for more context: https://answers.unity.com/questions/17718/argumentexception-getting-control-2s-position-in-a.html
            if (Event.current.type == EventType.Layout) {
                currentState = new HotReloadSettingsTabState(
                    running: EditorCodePatcher.Running, 
                    trialLicense: EditorCodePatcher.Status != null && (EditorCodePatcher.Status?.isTrial == true),
                    loginStatus: EditorCodePatcher.Status,
                    isServerHealthy: ServerHealthCheck.I.IsServerHealthy
                );
            }

            if (!EditorCodePatcher.LoginNotRequired) {
                using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.DynamicSectionOuterBoxCompact)) {
                    using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.DynamicSectionInnerBoxWide)) {
                        using (new EditorGUILayout.VerticalScope()) {
                            RenderLicenseInfoSection();
                        }
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.DynamicSectionOuterBoxCompact)) {
                using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.DynamicSectionInnerBoxWide)) {
                    using (new EditorGUILayout.VerticalScope()) {
                        HotReloadPrefs.Configuration = EditorGUILayout.Foldout(HotReloadPrefs.Configuration, "Configuration", true, HotReloadWindowStyles.FoldoutStyle);
                        if (HotReloadPrefs.Configuration) {
                            EditorGUILayout.Space();
                            RenderUnityAutoRefresh();
                            RenderAssetRefresh();
                            RenderConsoleWindow();
                            RenderAutostart();
                        }
                    }
                }
            }

            if (!EditorCodePatcher.LoginNotRequired && currentState.trialLicense && currentState.running) {
                using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.DynamicSectionOuterBoxCompact)) {
                    using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.DynamicSectionInnerBoxWide)) {
                        using (new EditorGUILayout.VerticalScope()) {
                            RenderPromoCodeSection();
                        }
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.DynamicSectionOuterBoxCompact)) {
                using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.DynamicSectionInnerBoxWide)) {
                    using (new EditorGUILayout.VerticalScope()) {
                        RenderOnDevice();
                    }
                }
            }
        }
        
        void RenderUnityAutoRefresh() {
            var newSettings = EditorGUILayout.BeginToggleGroup(new GUIContent("Allow to manage Unity's Auto Compile settings (recommended)"), HotReloadPrefs.AllowDisableUnityAutoRefresh);
            if (newSettings != HotReloadPrefs.AllowDisableUnityAutoRefresh) {
                HotReloadPrefs.AllowDisableUnityAutoRefresh = newSettings;
            }
            string toggleDescription;
            if (HotReloadPrefs.AllowDisableUnityAutoRefresh) {
                toggleDescription = "Hot Reload will manage Unity's Auto Refresh and Script Compilation settings when it's running. Previous settings will be restored when Hot Reload is stopped.";
            } else {
                toggleDescription = "Enable to allow Hot Reload to manage Unity's Auto Refresh and Script Compilation settings when it's running. If enabled, previous settings will be restored when Hot Reload is stopped.";
            }
            EditorGUILayout.LabelField(toggleDescription, HotReloadWindowStyles.WrapStyle);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.Space(3f);
        }
        
        void RenderAssetRefresh() {
            var newSettings = EditorGUILayout.BeginToggleGroup(new GUIContent("Asset refresh (recommended)"), HotReloadPrefs.AllAssetChanges);
            if (newSettings != HotReloadPrefs.AllAssetChanges) {
                HotReloadPrefs.AllAssetChanges = newSettings;
                // restart when setting changes
                if (ServerHealthCheck.I.IsServerHealthy) {
                    var restartServer = EditorUtility.DisplayDialog("Hot Reload",
                        $"When changing 'Asset refresh', the Hot Reload server must be restarted for this to take effect." +
                        "\nDo you want to restart it now?",
                        "Restart server", "Don't restart");
                    if (restartServer) {
                        EditorCodePatcher.RestartCodePatcher().Forget();
                    }
                }
            }
            string toggleDescription;
            if (HotReloadPrefs.AllAssetChanges) {
                toggleDescription = "Hot Reload will refresh changed assets in the project.";
            } else {
                toggleDescription = "Enable to allow Hot Reload to refresh changed assets in the project. All asset types are supported including sprites, prefabs, shaders etc.";
            }
            EditorGUILayout.LabelField(toggleDescription, HotReloadWindowStyles.WrapStyle);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.Space(3f);
        }

        void RenderConsoleWindow() {
            if (!HotReloadCli.CanOpenInBackground) {
                return;
            }
            var newSettings = EditorGUILayout.BeginToggleGroup(new GUIContent("Hide console window on start"), HotReloadPrefs.DisableConsoleWindow);
            if (newSettings != HotReloadPrefs.DisableConsoleWindow) {
                HotReloadPrefs.DisableConsoleWindow = newSettings;
                // restart when setting changes
                if (ServerHealthCheck.I.IsServerHealthy) {
                    var restartServer = EditorUtility.DisplayDialog("Hot Reload",
                        $"When changing 'Hide console window on start', the Hot Reload server must be restarted for this to take effect." +
                        "\nDo you want to restart it now?",
                        "Restart server", "Don't restart");
                    if (restartServer) {
                        EditorCodePatcher.RestartCodePatcher().Forget();
                    }
                }
            }
            string toggleDescription;
            if (HotReloadPrefs.DisableConsoleWindow) {
                toggleDescription = "Hot Reload will start without creating a console window. Logs can be accessed through \"Help\" tab.";
            } else {
                toggleDescription = "Enable to start Hot Reload without creating a console window.";
            }
            EditorGUILayout.LabelField(toggleDescription, HotReloadWindowStyles.WrapStyle);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.Space(3f);
        }
        
        void RenderAutostart() {
            var newSettings = EditorGUILayout.BeginToggleGroup(new GUIContent("Autostart on Unity open"), HotReloadPrefs.LaunchOnEditorStart);
            if (newSettings != HotReloadPrefs.LaunchOnEditorStart) {
                HotReloadPrefs.LaunchOnEditorStart = newSettings;
            }
            string toggleDescription;
            if (HotReloadPrefs.LaunchOnEditorStart) {
                toggleDescription = "Hot Reload will be launched when Unity project opens.";
            } else {
                toggleDescription = "Enable to launch Hot Reload when Unity project opens.";
            }
            EditorGUILayout.LabelField(toggleDescription, HotReloadWindowStyles.WrapStyle);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.Space();
        }

        void RenderOnDevice() {
            HotReloadPrefs.ShowOnDevice = EditorGUILayout.Foldout(HotReloadPrefs.ShowOnDevice, "On-Device", true, HotReloadWindowStyles.FoldoutStyle);
            if (!HotReloadPrefs.ShowOnDevice) {
                return;
            }
            // header with explainer image
            {
                if (headlineStyle == null) {
                    // start with textArea for the background and border colors
                    headlineStyle = new GUIStyle(GUI.skin.label) {
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft
                    };
                    headlineStyle.normal.textColor = HotReloadWindowStyles.H2TitleStyle.normal.textColor;

                    // bg color
                    if (HotReloadWindowStyles.IsDarkMode) {
                        headlineStyle.normal.background = EditorTextures.DarkGray40;
                    } else {
                        headlineStyle.normal.background = EditorTextures.LightGray225;
                    }
                    // layout
                    headlineStyle.padding = new RectOffset(8, 8, 0, 0);
                    headlineStyle.margin = new RectOffset(6, 6, 6, 6);
                }
                GUILayout.Space(9f); // space between logo and headline

                GUILayout.Label("Make changes to a build running on-device",
                    headlineStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 1.4f));
                // image showing how Hot Reload works with a phone
                // var bannerBox = GUILayoutUtility.GetRect(flowchart.width * 0.6f, flowchart.height * 0.6f);
                // GUI.DrawTexture(bannerBox, flowchart, ScaleMode.ScaleToFit);
            }

            GUILayout.Space(16f);
            
            //ButtonToOpenBuildSettings();

            {
                GUILayout.Label("Manual connect", HotReloadWindowStyles.H3TitleStyle);
                EditorGUILayout.Space();
                
                GUILayout.BeginHorizontal();
                
                // indent all controls (this works with non-labels)
                GUILayout.Space(16f);
                GUILayout.BeginVertical();

                HotReloadWindowStyles.H3TitleStyle.wordWrap = true;
                GUILayout.Label($"If auto-pair fails, use this IP to connect: {IpHelper.GetIpAddress()}" +
                                "\nMake sure you are on the same LAN/WiFi network",
                    HotReloadWindowStyles.H3TitleStyle);

                if (!currentState.isServerHealthy) {
                    DrawHorizontalCheck(ServerHealthCheck.I.IsServerHealthy,
                        "Hot Reload is running",
                        "Hot Reload is not running",
                        hasFix: false);
                }
                
                if (!HotReloadPrefs.ExposeServerToLocalNetwork) {
                    var summary = $"Enable '{new ExposeServerOption().ShortSummary}'";
                    DrawHorizontalCheck(HotReloadPrefs.ExposeServerToLocalNetwork,
                        summary,
                        summary);
                }

                HotReloadWindowStyles.H3TitleStyle.wordWrap = false;
                // explainer image that shows phone needs same wifi to auto connect ?
                
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(16f);
            
            // loading again is smooth, pretty sure AssetDatabase.LoadAssetAtPath is caching -Troy
            var settingsObject = HotReloadSettingsEditor.LoadSettingsOrDefault();
            var so = new SerializedObject(settingsObject);
            
            // if you build for Android now, will Hot Reload work?
            {
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Build Settings Checklist", HotReloadWindowStyles.H3TitleStyle);
                EditorGUI.BeginDisabledGroup(isSupported);
                // One-click to change each setting to the supported value
                if (GUILayout.Button("Fix All", GUILayout.MaxWidth(90f))) {
                    FixAllUnsupportedSettings(so);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                
                
                // NOTE: After user changed some build settings, window may not immediately repaint
                // (e.g. toggle Development Build in Build Settings window)
                // We could show a refresh button (to encourage the user to click the window which makes it repaint).
                DrawSectionCheckBuildSupport(so);
            }
            

            GUILayout.Space(16f);

            // Settings checkboxes (Hot Reload options)
            {
                GUILayout.Label("Mobile", HotReloadWindowStyles.H3TitleStyle);
                if (settingsObject) {
                    optionsSection.DrawGUI(so);
                }
            }
            GUILayout.FlexibleSpace(); // needed otherwise vertical scrollbar is appearing for no reason (Unity 2021 glitch perhaps)
        }
        
        private void RenderLicenseInfoSection() {
            _window.RunTab.RenderLicenseInfo(
                currentState.loginStatus,
                verbose: true,
                allowHide: false,
                overrideRenderFreeTrial: false,
                overrideActionButton: "Activate License",
                showConsumptions: true
            );
        }
        
        private void RenderPromoCodeSection() {
            _window.RunTab.RenderPromoCodes();
        }
        
        public void FocusLicenseFoldout() {
            HotReloadPrefs.ShowLogin = true;
        }

        // note: changing scripting backend does not force Unity to recreate the GUI, so need to check it when drawing.
        private ScriptingImplementation ScriptingBackend => HotReloadBuildHelper.GetCurrentScriptingBackend();
        private ManagedStrippingLevel StrippingLevel => HotReloadBuildHelper.GetCurrentStrippingLevel();
        public bool isSupported = true;

        /// <summary>
        /// These options are drawn in the On-device tab
        /// </summary>
        // new on-device options should be added here
        public static readonly IOption[] allOptions = new IOption[] {
            new ExposeServerOption(),
            IncludeInBuildOption.I,
            new AllowAndroidAppToMakeHttpRequestsOption(),
        };

        /// <summary>
        /// Change each setting to the value supported by Hot Reload
        /// </summary>
        private void FixAllUnsupportedSettings(SerializedObject so) {
            if (!isCurrentBuildTargetSupported.Value) {
                // try switch to Android platform
                // (we also support Standalone but HotReload on mobile is a better selling point)
                if (!TrySwitchToStandalone()) {
                    // skip changing other options (user won't readthe gray text) - user has to click Fix All again
                    return;
                }
            }
            
            foreach (var buildOption in allOptions) {
                if (!buildOption.GetValue(so)) {
                    buildOption.SetValue(so, true);
                }
            }
            so.ApplyModifiedProperties();
            var settingsObject = so.targetObject as HotReloadSettingsObject;
            if (settingsObject) {
                // when you click fix all, make sure to save the settings, otherwise ui does not update
                HotReloadSettingsEditor.EnsureSettingsCreated(settingsObject);
            }
            
            if (!EditorUserBuildSettings.development) {
                EditorUserBuildSettings.development = true;
            }
            
            HotReloadBuildHelper.SetCurrentScriptingBackend(ScriptingImplementation.Mono2x);
            HotReloadBuildHelper.SetCurrentStrippingLevel(ManagedStrippingLevel.Disabled);
        }

        public static bool TrySwitchToStandalone() {
            BuildTarget buildTarget;
            if (Application.platform == RuntimePlatform.LinuxEditor) {
                buildTarget = BuildTarget.StandaloneLinux64;
            } else if (Application.platform == RuntimePlatform.WindowsEditor) {
                buildTarget = BuildTarget.StandaloneWindows64;
            } else if (Application.platform == RuntimePlatform.OSXEditor) {
                buildTarget = BuildTarget.StandaloneOSX;
            } else {
                return false;
            }
            var current = EditorUserBuildSettings.activeBuildTarget;
            if (current == buildTarget) {
                return true;
            }
            var confirmed = EditorUtility.DisplayDialog("Switch Build Target",
                "Switching the build target can take a while depending on project size.",
                $"Switch to Standalone", "Cancel");
            if (confirmed) {
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Standalone, buildTarget);
                Log.Info($"Build target is switching to {buildTarget}.");
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Section that user can check before making a Unity Player build.
        /// </summary>
        /// <param name="so"></param>
        /// <remarks>
        /// This section is for confirming your build will work with Hot Reload.<br/>
        /// Options that can be changed after the build is made should be drawn elsewhere.
        /// </remarks>
        public void DrawSectionCheckBuildSupport(SerializedObject so) {
            isSupported = true;
            var selectedPlatform = currentBuildTarget.Value;
            DrawHorizontalCheck(isCurrentBuildTargetSupported.Value,
                $"The {selectedPlatform.ToString()} platform is selected",
                $"The current platform is {selectedPlatform.ToString()} which is not supported");

            using (new EditorGUI.DisabledScope(!isCurrentBuildTargetSupported.Value)) {
                // "Allow Mobile Builds to Connect (WiFi)"
                foreach (var option in allOptions) {
                    DrawHorizontalCheck(option.GetValue(so),
                        $"Enable \"{option.ShortSummary}\"",
                        $"Enable \"{option.ShortSummary}\"");
                }

                DrawHorizontalCheck(EditorUserBuildSettings.development,
                    "Development Build is enabled",
                    "Enable \"Development Build\"");
                
                DrawHorizontalCheck(ScriptingBackend == ScriptingImplementation.Mono2x,
                    $"Scripting Backend is set to Mono",
                    $"Set Scripting Backend to Mono");
                
                DrawHorizontalCheck(StrippingLevel == ManagedStrippingLevel.Disabled,
                    $"Stripping Level = {StrippingLevel}",
                    $"Stripping Level = {StrippingLevel}",
                    suggestedSolutionText: "Code stripping needs to be disabled to ensure that all methods are available for patching."
                );

                // if (isSupported) {
                //     GUILayout.Label("Great! Your current build settings are supported by Hot Reload.\nBuild and Run to try it.", HotReloadWindowStyles.WrapStyle);
                // }
            }
            // dont show the build settings checklist because some are relevant only for the current platform.
        }

        /// <summary>
        /// Draw a box with a tick or warning icon on the left, with text describing the tick or warning
        /// </summary>
        /// <param name="condition">The condition to check. True to show a tick icon, False to show a warning.</param>
        /// <param name="okText">Shown when condition is true</param>
        /// <param name="notOkText">Shown when condition is false</param>
        /// <param name="suggestedSolutionText">Shown when <paramref name="condition"/> is false</param>
        void DrawHorizontalCheck(bool condition, string okText, string notOkText = null, string suggestedSolutionText = null, bool hasFix = true) {
            if (okText == null) {
                throw new ArgumentNullException(nameof(okText));
            }
            if (notOkText == null) {
                notOkText = okText;
            }

            // include some horizontal space around the icon
            var boxWidth = GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.31f);
            var height = GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.01f);
            GUILayout.BeginHorizontal(HotReloadWindowStyles.BoxStyle, height, GUILayout.ExpandWidth(true));
            var style = HotReloadWindowStyles.NoPaddingMiddleLeftStyle;
            var iconRect = GUILayoutUtility.GetRect(
                Mathf.Round(EditorGUIUtility.singleLineHeight * 1.31f),
                Mathf.Round(EditorGUIUtility.singleLineHeight * 1.01f),
                style, boxWidth, height, GUILayout.ExpandWidth(false));
            // rounded so we can have pixel perfect black circle bg
            iconRect.Set(Mathf.Round(iconRect.x), Mathf.Round(iconRect.y), Mathf.CeilToInt(iconRect.width),
                Mathf.CeilToInt(iconRect.height));
            var text = condition ? okText : notOkText;
            var icon = condition ? iconCheck : iconWarning;
            if (GUI.enabled) {
                DrawBlackCircle(iconRect);
                // resource can be null when building player (Editor Resources not available)
                if (icon) {
                    GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                }
            } else {
                // show something (instead of hiding) so that layout stays same size
                DrawDisabledCircle(iconRect);
            }
            GUILayout.Space(4f);
            GUILayout.Label(text, style, height);

            if (!condition && hasFix) {
                isSupported = false;
            }

            GUILayout.EndHorizontal();
            if (!condition && !String.IsNullOrEmpty(suggestedSolutionText)) {
                // suggest to the user how they can resolve the issue
                EditorGUI.indentLevel++;
                GUILayout.Label(suggestedSolutionText, HotReloadWindowStyles.WrapStyle);
                EditorGUI.indentLevel--;
            }
        }

        void DrawDisabledCircle(Rect rect) => DrawCircleIcon(rect,
            Resources.Load<Texture>("icon_circle_gray"),
            Color.clear); // smaller circle draws less attention

        void DrawBlackCircle(Rect rect) => DrawCircleIcon(rect,
            Resources.Load<Texture>("icon_circle_black"),
            new Color(0.14f, 0.14f, 0.14f)); // black is too dark in unity light theme

        void DrawCircleIcon(Rect rect, Texture circleIcon, Color borderColor) {
            // Note: drawing texture from resources is pixelated on the edges, so it has some transperancy around the edges.
            // While building for Android, Resources.Load returns null for our editor Resources. 
            if (circleIcon != null) {
                GUI.DrawTexture(rect, circleIcon, ScaleMode.ScaleToFit);
            }
            
            // Draw smooth circle border
            const float borderWidth = 2f;
            GUI.DrawTexture(rect, EditorTextures.White, ScaleMode.ScaleToFit, true,
                0f,
                borderColor,
                new Vector4(borderWidth, borderWidth, borderWidth, borderWidth),
                Mathf.Min(rect.height, rect.width) / 2f);
        }
    }
}
