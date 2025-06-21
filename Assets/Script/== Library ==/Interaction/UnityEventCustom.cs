using UnityEngine;
using UnityEngine.Events;

public class UnityEventCustom : MonoBehaviour {

    public UnityEvent CallSpecific;

    public void OnCallSpecific() {
        CallSpecific?.Invoke();
    }

}
