using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Platformer
{
    public class UI_Manager : MonoBehaviour
    {
        private static UI_Manager _instance;

        public static UI_Manager Instance { get { return _instance; } }

        public GameObject[] UIElements { get { return uiElements; } private set { UIElements = uiElements; } }
        [SerializeField] private GameObject[] uiElements;

        [Space(10)]
        [SerializeField] bool forceOnscreenControls;

        public GameObject InteractionButton { get { return interactionButtion; } private set { InteractionButton = interactionButtion; } }
        [SerializeField] private GameObject interactionButtion;

        [Header("Health Bar")]
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private List<Image> heartImages;
        [SerializeField] private Sprite[] heartSprites;

        bool isPaused;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            if (GetInputType() == InputType.mobile)
                SetUIElementDisplayed(3, true);
            else
                SetUIElementDisplayed(3, false);

        }

        public void CreateHealthbar(int currentHealth, int maxHealth)
        {
            foreach (Transform child in UIElements[0].transform)
            {
                heartImages.Remove(child.GetComponent<Image>());
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < maxHealth; i++)
            {
                heartImages.Add(Instantiate(heartPrefab, UIElements[0].transform).GetComponent<Image>());
            }

            RefreshHealthbar(currentHealth, maxHealth);
        }

        public void RefreshHealthbar(int currentHealth, int maxHealth)
        {
            for (int i = 0; i < maxHealth; i++)
            {
                if (i < currentHealth)
                    heartImages[i].GetComponent<Image>().sprite = heartSprites[1];
                else
                    heartImages[i].GetComponent<Image>().sprite = heartSprites[0];
            }
        }

        public void RefreshCollectableUI(int score) => UIElements[4].GetComponent<CollectableScore_UI>().RefreshCollectableScore(score);

        public void SetActiveInteractionButton(bool value)
        {
            if ((SystemInfo.deviceType == DeviceType.Handheld) || forceOnscreenControls)
                interactionButtion.SetActive(value);
        }

        public void TogglePause()
        {
            if (isPaused)
            {
                Time.timeScale = 1;
                isPaused = false;
                SetUIElementDisplayed(5, false);
            }
            else
            {
                Time.timeScale = 0;
                isPaused = true;
                SetUIElementDisplayed(5, true);
            }
        }

        public void SetUIElementDisplayed(int elementIndex, bool value) => UIElements[elementIndex].SetActive(value);

        public void LoadScene(int sceneIndex)
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.TransitionScene(sceneIndex);
                return;
            }

            SceneManager.LoadScene(sceneIndex);
        }

        public void ReloadScene()
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.TransitionScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public InputType GetInputType()
        {
            if ((SystemInfo.deviceType == DeviceType.Handheld) || forceOnscreenControls)
                return InputType.mobile;
            else
                return InputType.pc;
        }

        public enum InputType
        {
            pc,
            mobile
        }
    }
}