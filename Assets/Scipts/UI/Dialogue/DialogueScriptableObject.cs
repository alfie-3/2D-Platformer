using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "CharacterData/Dialogue", order = 1), System.Serializable]
public class DialogueScriptableObject : ScriptableObject
{
    public SentenceData[] Dialogue = new SentenceData[1];
}

[System.Serializable]
public class SentenceData
{
    public enum Character
    {
        NPC,
        Player,
        nameOverride
    }

    [Space(10)]

    public Character character = Character.NPC;
    public string overrideName;

    [Space(10)]

    [TextArea(3, 10)]
    public string sentenceText;
    public TMPro.FontStyles fontStyle;
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(SentenceData))]
public class CounterDrawer : PropertyDrawer
{
    float FOLDOUT_HEIGHT = 16f;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty character = property.FindPropertyRelative("character");
        SerializedProperty overrideName = property.FindPropertyRelative("overrideName");
        SerializedProperty sentenceText = property.FindPropertyRelative("sentenceText");
        SerializedProperty fontStyle = property.FindPropertyRelative("fontStyle");

        EditorGUI.BeginProperty(position, label, property);
        Rect foldoutRect = new Rect(position.x, position.y, position.width, FOLDOUT_HEIGHT);

        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, (label.ToString().Replace("Element", "Sentence")));

        if (property.isExpanded)
        {
            Rect characterRect = new Rect(position.x, position.y += 16, position.width, EditorGUI.GetPropertyHeight(character));
            Rect ovrdnameRect = new Rect(position.x, position.y + 38, position.width, EditorGUI.GetPropertyHeight(overrideName));
            Rect sentenceRect = new Rect(position.x, position.y + 64, position.width, EditorGUI.GetPropertyHeight(sentenceText));
            Rect fontStyleRect = new Rect(position.x, position.y + 144, position.width, 16);
            Rect EndBox = new Rect(position.x, position.y + 176, position.width, 16);

            EditorGUI.PropertyField(characterRect, character);

            if (character.enumValueIndex == 2)
                EditorGUI.PropertyField(ovrdnameRect, overrideName);
            else
                EditorGUI.LabelField(ovrdnameRect, "Override Name");

            sentenceText.stringValue = EditorGUI.TextArea(sentenceRect, sentenceText.stringValue);
            EditorGUI.PropertyField(fontStyleRect, fontStyle);
            EditorGUI.LabelField(EndBox, "================================================================================================================================================================================================================================================================================================================================");
        }

        EditorGUI.EndProperty();
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty character = property.FindPropertyRelative("character");
        SerializedProperty overrideName = property.FindPropertyRelative("overrideName");
        SerializedProperty sentenceText = property.FindPropertyRelative("sentenceText");
        SerializedProperty fontStyle = property.FindPropertyRelative("fontStyle");

        float height = FOLDOUT_HEIGHT;
        if (property.isExpanded)
        {
            height += EditorGUI.GetPropertyHeight(character);
            height += EditorGUI.GetPropertyHeight(overrideName);
            height += EditorGUI.GetPropertyHeight(sentenceText);
            height += EditorGUI.GetPropertyHeight(fontStyle);
            height += 64;
        }

        return height;
    }
}
#endif
