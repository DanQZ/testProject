using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// this script will let the stance objects move along a path and disable this script when it hits the target position
public class CustomPathFollower : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 1f;
    float distanceTravelled;
    public Vector3 targetPosition;
    float distanceToTarget;

    // resets variables
    void Start()
    {
        distanceTravelled = 0f;
        distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;
        }
    }

    void Update()
    {
        distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (pathCreator != null)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }
    }

    void Finish(){
        this.enabled = false;
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}

