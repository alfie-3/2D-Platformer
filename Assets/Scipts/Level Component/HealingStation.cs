using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class HealingStation : MonoBehaviour, IInteractable
    {
        [SerializeField] int healingCharges = 3;

        [SerializeField] Sprite[] healingStationSprites;

        [SerializeField] AudioClip collectionSound;

        public void Interact(GameObject player)
        {
            IHealable healable = player.GetComponent<IHealable>();

            if (healingCharges <= 0 || healable.GetHealth() == healable.GetMaxHealth())
                return;

            healable.Heal(1);
            healingCharges--;
            GetComponent<SpriteRenderer>().sprite = healingStationSprites[healingCharges];
            GetComponent<Animator>().Play("Collect");
            AudioTools.CreateTempAudioSource(transform.position, collectionSound, 0.2f);
        }

        public bool IsInteractable()
        {
            return healingCharges > 0;
        }

        public void OnInRange()
        {

        }

        public void OnOutOfRange()
        {

        }
    }
}
