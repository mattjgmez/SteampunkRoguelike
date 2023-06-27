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
}
