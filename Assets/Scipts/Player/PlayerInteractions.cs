using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] PlayerInput input;
        [Space(10)]
        [SerializeField] Vector3 checkBoxLoc;
        [SerializeField] Vector2 checkBoxSize;
        [SerializeField] LayerMask interactableMask;
        [Space]
        [SerializeField] bool interactableInRange;
        [Space]
        [SerializeField] SpriteRenderer pcInteractionSprite;

        SpriteRenderer sprite;

        private void Awake()
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            if (CheckForInteractables() != null)
            {
                interactableInRange = true;
                UI_Manager.Instance.SetActiveInteractionButton(true);

                if (SystemInfo.deviceType == DeviceType.Desktop && !pcInteractionSprite.enabled) {
                    pcInteractionSprite.enabled = true;
                }
            }
            else
            {
                interactableInRange = false;
                UI_Manager.Instance.SetActiveInteractionButton(false);

                if (SystemInfo.deviceType == DeviceType.Desktop && pcInteractionSprite.enabled) {
                    pcInteractionSprite.enabled = false;
                }
            }
    ;
        }

        private void Update()
        {
            if (input.actions["Interact"].WasPressedThisFrame() && CheckForInteractables() != null)
                Interact();
        }

        private void Interact()
        {
            if (GameManager.Instance.canInteract)
                CheckForInteractables()[0].Interact(gameObject);
        }

        private IInteractable[] CheckForInteractables()
        {
            Vector3 localCheckLoc = (sprite.flipX) ? transform.position - checkBoxLoc : transform.position + checkBoxLoc;

            RaycastHit2D[] raycastHit2D = Physics2D.BoxCastAll(localCheckLoc, checkBoxSize, 0, Vector2.right, 0, interactableMask);

            if (raycastHit2D.Length == 0) return null;

            List<IInteractable> interactables = new();

            foreach (RaycastHit2D item in raycastHit2D)
            {
                if (item.transform.TryGetComponent(out IInteractable interactable))
                {
                    if (!interactable.IsInteractable())
                        return null;

                        interactables.Add(interactable);
                }
            }

            return interactables.ToArray();
        }

        private void OnDrawGizmos()
        {
            Vector3 relativeCheckLoc = checkBoxLoc;

            if (sprite != null)
            {
                relativeCheckLoc = (sprite.flipX) ? transform.position - checkBoxLoc : transform.position + checkBoxLoc;
            }

            Gizmos.DrawWireCube(relativeCheckLoc, checkBoxSize);
        }
    }
}
