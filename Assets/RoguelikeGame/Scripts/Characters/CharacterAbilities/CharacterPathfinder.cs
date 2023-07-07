using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterPathfinder : MonoBehaviour
{
    [Header("PathfindingTarget")]
    public Transform Target;
    /// the distance to waypoint at which the movement is considered complete
    public float DistanceToWaypointThreshold = 1f;

    [Header("Debug")]
    public bool DebugDrawPath;

    public NavMeshPath CurrentPath;
    public Vector3[] Waypoints;
    public int NextWaypointIndex;
    public Vector3 NextWaypointDirection;
    public float DistanceToNextWaypoint;

    protected Vector3 _direction;
    protected Vector2 _newMovement;
    protected TopDownController _topDownController;
    protected CharacterMovement _characterMovement;

    protected virtual void Awake()
    {
        CurrentPath = new NavMeshPath();
        _topDownController = GetComponent<TopDownController>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    public virtual void SetNewDestination(Transform destinationTransform)
    {
        if (destinationTransform == null) { return; }

        Target = destinationTransform;
        DeterminePath(this.transform.position, destinationTransform.position);
    }

    protected virtual void DeterminePath(Vector3 startingPos, Vector3 targetPos)
    {
        NextWaypointIndex = 0;

        NavMesh.CalculatePath(startingPos, targetPos, NavMesh.AllAreas, CurrentPath);
        Waypoints = CurrentPath.corners;
        if (CurrentPath.corners.Length >= 2) 
        { 
            NextWaypointIndex = 1;
        }
    }

    protected virtual void Update()
    {
        if (Target == null) { return; }

        DrawDebugPath();
        DetermineNextWaypoint();
        DetermineDistanceToNextWaypoint();
        MoveController();
    }

    protected virtual void DrawDebugPath()
    {
        if (DebugDrawPath)
        {
            for (int i = 0; i < CurrentPath.corners.Length; i++)
            {
                Debug.DrawLine(CurrentPath.corners[i], CurrentPath.corners[i + 1], Color.red);
            }
        }
    }

    protected virtual void DetermineNextWaypoint()
    {
        if (Waypoints.Length <= 0 || NextWaypointIndex < 0) { return; }

        if (Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]) <= DistanceToWaypointThreshold)
        {
            if (NextWaypointIndex + 1 < Waypoints.Length)
            {
                NextWaypointIndex++;
            }
            else
            {
                NextWaypointIndex = -1;
            }
        }
    }

    protected virtual void DetermineDistanceToNextWaypoint()
    {
        if (NextWaypointIndex <= 0)
        {
            DistanceToNextWaypoint = 0;
        }
        else
        {
            DistanceToNextWaypoint = Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]);
        }
    }

    protected virtual void MoveController()
    {
        if ((Target == null) || (Waypoints.Length <= 0))
        {
            _characterMovement.SetMovement(Vector2.zero);
            return;
        }
        else
        {
            _direction = (Waypoints[NextWaypointIndex] - this.transform.position).normalized;
            _newMovement.x = _direction.x;
            _newMovement.y = _direction.y;
            _characterMovement.SetMovement(_newMovement);
        }
    }
}
