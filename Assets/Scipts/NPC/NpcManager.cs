using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class NpcManager : MonoBehaviour, IInteractable
    {
        public NpcScriptableObject npcData;

        [Header("Dialogue")]
        public DialogueScriptableObject currentDialogue;

        [SerializeField]
        public enum DialogueActivationType
        {
            interact,
            trigger,
        }

        [SerializeField] DialogueActivationType activationType = DialogueActivationType.interact;
        [Space]
        [SerializeField] UnityEvent startDialogueEvent;
        [SerializeField] UnityEvent endDialogueEvent;

        public void TriggerDialogueStart()
        {
            startDialogueEvent.Invoke();
            UI_Manager.Instance.GetComponentInChildren<DialogueManager>(true).StartDialogue(npcData, currentDialogue, this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (activationType == DialogueActivationType.interact || !collision.CompareTag("Player"))
                return;

            if (currentDialogue != null)
                TriggerDialogueStart();
        }

        public DialogueActivationType GetDialogueActivationType() => activationType;

        public void Interact(GameObject player)
        {
            TriggerDialogueStart();
        }

        public void DialogueEnded() => endDialogueEvent.Invoke();

        public void OnInRange()
        {

        }

        public void OnOutOfRange()
        {

        }

        public bool IsInteractable()
        {
            if (GameManager.Instance.canInteract && currentDialogue != null)
                return true;
            else
                return false;
        }

        public void SetCurrentDialogue(DialogueScriptableObject dialogue = null) => Debug.Log(dialogue);
        public void DisableDialogue() => currentDialogue = null;

    }
}
