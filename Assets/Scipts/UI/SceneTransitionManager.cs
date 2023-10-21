using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Platformer
{
    public class SceneTransitionManager : MonoBehaviour
    {
        private static SceneTransitionManager _instance;

        public static SceneTransitionManager Instance { get { return _instance; } }

        [SerializeField] float transitionSpeed = 0.1f;

        [SerializeField] bool isTransitioning;
        [SerializeField] Material mat;

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

            Object.DontDestroyOnLoad(gameObject);

            mat = GetComponent<Image>().material;
            mat = Instantiate(mat);
            GetComponent<Image>().material = mat;
        }

        public async void TransitionScene(int sceneBuildIndex)
        {
            if (isTransitioning)
                return;

            isTransitioning = true;
            await CircleShrink();
            var asyncOp = SceneManager.LoadSceneAsync(sceneBuildIndex);

            while (!asyncOp.isDone)
                await Task.Yield();

            isTransitioning = false;

            await Task.Delay(300);

            await CircleGrow();
        }

        private async Task CircleGrow()
        {
            float currentAmount = 0;

            while (currentAmount < 0.995f)
            {
                currentAmount = Mathf.Lerp(currentAmount, 1, transitionSpeed * Time.unscaledDeltaTime);
                mat.SetFloat("_CutoutSize", currentAmount);

                await Task.Yield();
            }

            mat.SetFloat("_CutoutSize", 1);
            await Task.Yield();
        }

        private async Task CircleShrink()
        {
            float currentAmount = 1;

            while (currentAmount > 0.009f)
            {
                currentAmount = Mathf.Lerp(currentAmount, 0, transitionSpeed * Time.unscaledDeltaTime);
                mat.SetFloat("_CutoutSize", currentAmount);

                await Task.Yield();
            }

            mat.SetFloat("_CutoutSize", -0.1f);
            await Task.Yield();
        }
    }
}
