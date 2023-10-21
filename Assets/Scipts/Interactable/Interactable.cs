using UnityEngine;

namespace Platformer
{
    public interface IInteractable { void Interact(GameObject player); void OnInRange(); void OnOutOfRange(); bool IsInteractable(); };
}