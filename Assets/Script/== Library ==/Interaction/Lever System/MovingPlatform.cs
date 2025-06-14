using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float moveSpeed = 3f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Start() {
        // Platform dimulai dari posisi startPoint
        transform.position = startPoint.position;
        targetPosition = endPoint.position;
    }

    private void Update() {
        if (isMoving) {
            // Gerakkan platform menuju targetPosition
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Jika platform sudah sangat dekat dengan target, hentikan pergerakan
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
                isMoving = false;
            }
        }
    }

    // Fungsi ini akan dipanggil oleh tuas (lever)
    public void ActivatePlatform() {
        // Jika sedang tidak bergerak, mulai bergerak
        if (!isMoving) {
            // Tukar target posisi: jika di awal, targetnya akhir, begitu pula sebaliknya
            targetPosition = (Vector3.Distance(transform.position, startPoint.position) < 0.01f) ? endPoint.position : startPoint.position;
            isMoving = true;
        }
    }
}