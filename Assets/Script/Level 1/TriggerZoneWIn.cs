using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerZoneWIn : MonoBehaviour
{
    public GameObject show;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            show.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
