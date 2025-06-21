using UnityEngine;

// Skrip ini bertanggung jawab untuk mendorong objek fisika saat player menabraknya.
public class PlayerPusher : MonoBehaviour {
    [SerializeField] private float pushPower = 2.0f;

    // Fungsi ini harus berada di skrip yang sama dengan CharacterController untuk bisa dipanggil.
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // Cek apakah objek yang ditabrak punya Rigidbody
        Rigidbody body = hit.collider.attachedRigidbody;

        // Kondisi untuk tidak mendorong
        if (body == null || body.isKinematic) {
            return;
        }

        // Cek apakah kita menabrak dari samping, bukan dari atas
        if (hit.moveDirection.y < -0.3f) {
            return;
        }

        // Hitung dan terapkan gaya dorong
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.linearVelocity = pushDir * pushPower;
    }
}