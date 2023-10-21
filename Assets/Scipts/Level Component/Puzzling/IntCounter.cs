using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntCounter : MonoBehaviour
{
    [SerializeField] int counterGoal;
    [SerializeField] int counter;

    [SerializeField] UnityEvent counterGoalReached;

    public void IncrementCounter()
    {
        counter++;
        CheckCounter();
    }

    public void DecrementCounter()
    {
        counter--;
        CheckCounter();
    }

    public void CustomCounterIncrement(int increment)
    {
        counter += increment;
        CheckCounter();
    }

    public void CheckCounter()
    {
        if (counter >= counterGoal)
            counterGoalReached.Invoke();
    }
}
