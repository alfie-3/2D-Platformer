using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Platformer
{

    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance { get { return _instance; } }

        [Header("CheckPoint")]
        public GameObject activeCheckpoint;

        [Header("Player")]
        public bool canInteract = true;

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

            Time.timeScale = 1.0f;

            if (SystemInfo.deviceType.Equals(DeviceType.Handheld))
                Application.targetFrameRate = 60;
        }

        public void LevelComplete(GameObject player)
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.TransitionScene(2);
                return;
            }

            SceneManager.LoadScene(2);
        }

        public GameObject TryGetCurrentPlayer() => GameObject.FindGameObjectWithTag("Player");
    }
}
