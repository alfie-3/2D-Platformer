using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] new bool enabled = true;
    [SerializeField] bool oneTime;
    [Space]
    [SerializeField] UnityEvent enterEvent;
    [Space]
    [SerializeField] UnityEvent exitEvent;

    private bool readyToBeTriggered = true;

    private void Start() {
        if (!TryGetComponent(out Collider2D collider)) {
            Debug.LogWarning("A collider is required to use the Trigger Event component, learn more about components at: https://moodle.glos.ac.uk/48225/resources/components");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enabled)
        {
            if (readyToBeTriggered)
            {
                enterEvent.Invoke();
                readyToBeTriggered = false;
            }

            if (!oneTime)
                readyToBeTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enabled)
        {
            if (readyToBeTriggered)
            {
                exitEvent.Invoke();
                readyToBeTriggered = false;
            }

            if (!oneTime)
                readyToBeTriggered = true;
        }
    }

    public void SetActiveAndEnabled(bool value) => enabled = value;
}
