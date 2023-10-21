using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Platformer
{

    public class MainMenu_Manager : MonoBehaviour
    {
        public void ChangeScene(int sceneBuildIndex)
        {
            AsyncLoadScene(sceneBuildIndex);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private void AsyncLoadScene(int sceneBuildIndex)
        {
            SceneTransitionManager.Instance.TransitionScene(1);
        }
    }
}
