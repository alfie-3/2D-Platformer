using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectableItem : MonoBehaviour
{
   [SerializeField] protected float circleCastRadius;
    private LayerMask playerLayerMask;
    Vector3 startPosition;

    public void Awake()
    {
        playerLayerMask = LayerMask.GetMask("Player");
        startPosition = transform.position;
    }

    public void Update()
    {
        if (Physics2D.OverlapCircle(transform.position, circleCastRadius, playerLayerMask))
        {
            Collect();
        }

        transform.position = startPosition + new Vector3(0.0f, Mathf.Sin(Time.time) / 5, 0.0f);
    }

    public abstract void Collect();
}
