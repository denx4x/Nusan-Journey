using UnityEngine;
 
public class Door : MonoBehaviour, IInteractable {
 
	float targetYRotation;
 
	public float smooth;
	public bool autoClose;
 
	Transform player;
 
	float defaultYRotation = 0f;
	float timer = 0f;
 
	public Transform pivot;
 
	bool isOpen;
 
	void Start() {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		defaultYRotation = transform.eulerAngles.y;
	}
 
	void Update() {
		pivot.rotation = Quaternion.Lerp(pivot.rotation, Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f), smooth * Time.deltaTime);
 
		timer -= Time.deltaTime;
 
		if (timer <= 0f && isOpen && autoClose) {
			ToggleDoor(player.position);
		}
	}
 
	public void ToggleDoor(Vector3 pos) {
		isOpen = !isOpen;
 
		if (isOpen) {
			Vector3 dir = (pos - transform.position);
			targetYRotation = Mathf.Sign(Vector3.Dot(transform.right, dir)) * 90f;
			timer = 5f;
		} else {
			targetYRotation = 0f;
		}
	}
 
	public void Open(Vector3 pos) {
		if (!isOpen) {
			ToggleDoor(pos);
		}
	}
	public void Close(Vector3 pos) {
		if (isOpen) {
			ToggleDoor(pos);
		}
	}
 
	public void Interact() {
		ToggleDoor(player.position);
	}
 
	public string GetDescription() {
		if (isOpen) return "Close the door";
		return "Open the door";
	}
}