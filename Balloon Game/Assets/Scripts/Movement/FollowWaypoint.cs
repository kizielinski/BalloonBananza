using UnityEngine;

public class FollowWaypoint : MonoBehaviour
{
    [Header("Path Setup")]
    [SerializeField] private Transform waypointParent;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float waypointArrivalDistance = 0.05f;

    private int currentWaypointIndex;

    private void Awake()
    {
        if (GetWaypointCount() == 0)
        {
            return;
        }

        currentWaypointIndex = GetNearestWaypointIndex();
    }

    private void Update()
    {
        if (GetWaypointCount() == 0)
        {
            return;
        }

        Transform targetWaypoint = GetWaypoint(currentWaypointIndex);
        if (targetWaypoint == null)
        {
            AdvanceToNextWaypoint();
            return;
        }

        Vector3 targetPosition = targetWaypoint.position;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) <= waypointArrivalDistance)
        {
            AdvanceToNextWaypoint();
        }
    }

    private int GetNearestWaypointIndex()
    {
        int nearestIndex = 0;
        float nearestSqrDistance = float.MaxValue;
        int waypointCount = GetWaypointCount();

        for (int i = 0; i < waypointCount; i++)
        {
            Transform waypoint = GetWaypoint(i);
            if (waypoint == null)
            {
                continue;
            }

            float sqrDistance = (waypoint.position - transform.position).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private void AdvanceToNextWaypoint()
    {
        int waypointCount = GetWaypointCount();
        if (waypointCount == 0)
        {
            return;
        }

        currentWaypointIndex = (currentWaypointIndex + 1) % waypointCount;
    }

    private int GetWaypointCount()
    {
        return waypointParent == null ? 0 : waypointParent.childCount;
    }

    private Transform GetWaypoint(int index)
    {
        if (waypointParent == null || index < 0 || index >= waypointParent.childCount)
        {
            return null;
        }

        return waypointParent.GetChild(index);
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        waypointArrivalDistance = Mathf.Max(0.01f, waypointArrivalDistance);
    }

    private void OnDrawGizmosSelected()
    {
        int waypointCount = GetWaypointCount();
        if (waypointCount == 0)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypointCount; i++)
        {
            Transform waypoint = GetWaypoint(i);
            if (waypoint == null)
            {
                continue;
            }

            Gizmos.DrawSphere(waypoint.position, 0.1f);

            Transform nextWaypoint = GetWaypoint((i + 1) % waypointCount);
            if (nextWaypoint != null)
            {
                Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
            }
        }
    }
}
