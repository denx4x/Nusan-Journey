using UnityEngine;

public class ObjectMover : MonoBehaviour {    
    public void MoveToTarget(Transform target) {
        if (target != null) {
            transform.position = target.position;
        }
    }

    public void MoveToPredefinedPosition(Vector3 newPosition) {
        transform.position = newPosition;
    }
}