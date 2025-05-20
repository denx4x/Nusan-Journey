using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{
    [SerializeField] private GameObject next;

    private void Start()
    {
        Time.timeScale = 0f;
    }

    public void show()
    {
        next.SetActive(true);
        this.gameObject.SetActive(false);
    }

}
