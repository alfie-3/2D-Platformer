using UnityEngine;

namespace Platformer
{
    public class ResearchBaseController : MonoBehaviour
    {
        Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void ToggleFacade(bool value) => anim.SetBool("Disolve", value);

        public void ToggleFrontDoor(bool value) => anim.SetBool("FrontDoorOpen", value);

        public void ToggleBackDoor(bool value)
        {
                anim.SetBool("BackDoorOpen", value);
        }

        public void ToggleScreenFlicker(bool value) => anim.SetBool("ScreenOn", value);
    }
}
