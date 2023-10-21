using System;
using UnityEditor;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform[] pathPoints;
    int currentTarget = 1;
    [Space]
    [SerializeField] float speed = 2;
    [SerializeField] MovementType movementType = MovementType.PingPong;
    bool playerColliding;
    [SerializeField] bool movementEnabled = true;
    bool reverse;
    [Space]
    [SerializeField] GameObject pointPrefab;

    private enum MovementType
    {
        PingPong,
        ActivateOnStand
    }

    private void Awake()
    {
        if (movementType == MovementType.ActivateOnStand)
            SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!movementEnabled)
            return;

        switch (movementType)
        {
            case MovementType.PingPong:
                MoveAlongPath();
                break;

            case MovementType.ActivateOnStand:
                MoveAlongPath();
                break;
        };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player") && !collision.enabled)
            return;

        if (movementType == MovementType.ActivateOnStand && currentTarget == 1)
        {
            SetActive(true);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player") && !collision.enabled)
            return;

        collision.transform.parent = transform;
        playerColliding = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        collision.transform.parent = null;
        playerColliding = false;
    }

    private void MoveAlongPath()
    {
        if (Vector2.Distance(pathPoints[currentTarget].position, transform.position) < 0.05)
        {
            if (currentTarget >= pathPoints.Length - 1)
                reverse = true;
            else if (currentTarget <= 0)
                reverse = false;

            if (reverse)
                currentTarget--;
            else
                currentTarget++;

            if (currentTarget == 1 && !playerColliding && movementType == MovementType.ActivateOnStand)
                SetActive(false);

        }

        transform.position = Vector2.MoveTowards(transform.position, pathPoints[currentTarget].position, Time.deltaTime * speed);
    }

    public void SetActive(bool value)
    {
        movementEnabled = value;
        GetComponent<Animator>().SetBool("Active", value);
    }

#if UNITY_EDITOR

    public void AddPoint()
    {
        GameObject point = Instantiate(pointPrefab, transform.parent);

        if (pathPoints.Length > 0)
            point.transform.position = new Vector2(pathPoints[^1].position.x + 5, pathPoints[^1].position.y);
        else
            point.transform.position = transform.position;

        point.name = "Platform Point " + (pathPoints.Length + 1);

        if (pathPoints.Length > 0)
            Array.Resize(ref pathPoints, pathPoints.Length + 1);
        else
            Array.Resize(ref pathPoints, 1);

        pathPoints[^1] = point.transform;
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }

    public void RemovePoint()
    {
        if (pathPoints.Length == 0)
            return;

        GameObject lastPoint = pathPoints[^1].gameObject;
        Array.Resize(ref pathPoints, pathPoints.Length - 1);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        DestroyImmediate(lastPoint);
    }

#endif

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < pathPoints.Length; i++)
        {
            if (i + 1 != pathPoints.Length)
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
        }
    }
}
