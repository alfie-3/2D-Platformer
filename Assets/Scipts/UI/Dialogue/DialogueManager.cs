using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Platformer
{

    public class DialogueManager : MonoBehaviour
    {
        private Queue<SentenceData> senetences;
        private SentenceData sentence;
        private NpcScriptableObject npc;
        private NpcManager activeNPC;
        private GameObject bloopAudioSource;

        [SerializeField] float textScrollSpeed;

        [Space(10)]

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] GameObject continuePrompt;
        [SerializeField] AudioClip bloop;

        [SerializeField] private Animator anim;

        [Space(10)]
        [SerializeField] bool inDialouge;
        [SerializeField] private bool readyForNextSentence = true;

        private void Start()
        {
            senetences = new Queue<SentenceData>();
        }

        public void StartDialogue(NpcScriptableObject npcData, DialogueScriptableObject dialogueCollection, NpcManager npcManager)
        {
            GameManager.Instance.canInteract = false;

            GetComponentInParent<UI_Manager>().SetUIElementDisplayed(3, false);

            anim.SetBool("isOpen", true);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>().SetMovementEnabled(false);

            Debug.Log("Begin dialogue with " + npcData.NPC_Name);

            activeNPC = npcManager;
            npc = npcData;
            senetences.Clear();

            foreach (SentenceData dialogue in dialogueCollection.Dialogue)
            {
                senetences.Enqueue(dialogue);
            }

            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            continuePrompt.SetActive(false);

            if (senetences.Count == 0 && readyForNextSentence)
            {
                EndDialogue();
                return;
            }

            StopAllCoroutines();

            if (readyForNextSentence)
            {
                nameText.text = GetCurrentName();
                dialogueText.fontStyle = senetences.Peek().fontStyle;
            }

            if (!readyForNextSentence)
            {
                CompleteSentence(sentence.sentenceText);
                return;
            }

            if (readyForNextSentence)
            {
                sentence = senetences.Dequeue();
                StartCoroutine(TypeSentence(sentence.sentenceText));
            }
        }

        private void CompleteSentence(string sentence)
        {
            Destroy(bloopAudioSource);
            readyForNextSentence = true;
            dialogueText.text = sentence;
            continuePrompt.SetActive(true);
        }

        IEnumerator TypeSentence(string sentence)
        {
            readyForNextSentence = false;

            dialogueText.text = "";

            bloopAudioSource = new GameObject(("TempAudio"));
            bloopAudioSource.transform.position = activeNPC.transform.position;
            AudioSource tempAudioSource = bloopAudioSource.AddComponent<AudioSource>();

            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;

                if (char.IsLetter(letter))
                {
                    tempAudioSource.pitch = Random.Range(0.95f, 1.05f);
                    tempAudioSource.PlayOneShot(bloop, 0.05f);
                }

                yield return new WaitForSecondsRealtime(textScrollSpeed);
            }

            Destroy(bloopAudioSource);
            readyForNextSentence = true;
            continuePrompt.SetActive(true);
        }

        public void EndDialogue()
        {
            if (activeNPC != null)
                activeNPC.DialogueEnded();

            activeNPC = null;

            if (GetComponentInParent<UI_Manager>().GetInputType() == UI_Manager.InputType.mobile)
                GetComponentInParent<UI_Manager>().SetUIElementDisplayed(3, true);

            anim.SetBool("isOpen", false);
            GameManager.Instance.canInteract = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>().SetMovementEnabled(true);
        }

        public string GetCurrentName()
        {
            switch (senetences.Peek().character)
            {
                case SentenceData.Character.NPC:
                    return npc.NPC_Name;
                case SentenceData.Character.Player:
                    return "Player";
                case SentenceData.Character.nameOverride:
                    return senetences.Peek().overrideName;
            }

            return "error";
        }
    }
}
