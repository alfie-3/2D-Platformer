using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
    public class InteractableButton : MonoBehaviour, IInteractable
    {
        [SerializeField] float invokeDelay;
        [Space]
        [SerializeField] UnityEvent unityEvent;
        [Space]
        [SerializeField] Sprite[] buttonSprites;

        private bool isPressed;

        public void Interact(GameObject player)
        {
            isPressed = true;

            GetComponent<AudioSource>().Play();
            GetComponent<SpriteRenderer>().sprite = buttonSprites[1];

            if (invokeDelay > 0)
                StartCoroutine(InvokeWithDelay());
            else
                unityEvent.Invoke();
        }

        public bool IsInteractable()
        {
            return !isPressed;
        }

        public void OnInRange()
        {

        }

        public void OnOutOfRange()
        {

        }

        private IEnumerator InvokeWithDelay()
        {
            yield return new WaitForSeconds(invokeDelay);
            unityEvent.Invoke();
        }
    }
}
