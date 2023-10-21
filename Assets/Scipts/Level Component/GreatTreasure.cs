using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class GreatTreasure : MonoBehaviour, IInteractable
    {
        bool isCollected = false;

        public void Interact(GameObject player)
        {
            GameManager.Instance.LevelComplete(player);
            isCollected = true;
        }

        public bool IsInteractable()
        {
            return !isCollected;
        }

        public void OnInRange()
        {

        }

        public void OnOutOfRange()
        {

        }
    }
}
