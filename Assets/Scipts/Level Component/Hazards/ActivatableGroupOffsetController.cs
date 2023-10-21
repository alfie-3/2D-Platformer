using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class ActivatableGroupOffsetController : MonoBehaviour
    {
        [SerializeField] GameObject[] activatables;

        [SerializeField] float offset;

        [SerializeField] private bool routineEnabled;

        public void SetEnabled(bool value)
        {
            if (value == true)
            {
                routineEnabled = value;
                StartCoroutine(ActivateRoutine());
            }
            else
            {
                routineEnabled = value;
                StopAllCoroutines();
            }
        }

        public void Start()
        {
            foreach (var activatable in activatables)
            {
                if (activatable.GetComponent<IActivatable>() == null)
                    Debug.LogError("Non activatable object detected in activatable array");
            }

            StartCoroutine(ActivateRoutine());
        }

        IEnumerator ActivateRoutine()
        {
            while (routineEnabled)
            {
                for (int i = 0; i < activatables.Length; i++)
                {
                    activatables[i].GetComponent<IActivatable>().Activate();
                    yield return new WaitForSeconds(offset);
                }
            }
        }
    }

    public interface IActivatable
    {
        public void Activate();
    }
}
