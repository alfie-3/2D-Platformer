using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    [SerializeField] bool activated;

    public void ChangeGizmo()
    {
        activated = true;
    }

    private void OnDrawGizmos()
    {
        if (!activated)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
