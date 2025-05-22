using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    private PlayerControls controls;

    // Properti public untuk mengakses PlayerControls
    public PlayerControls PlayerControls {
        get { return controls; }
    }

    private void Awake() {
        controls = new PlayerControls();
        Debug.Log("PlayerControls diinisialisasi: " + (controls != null));
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }
}
