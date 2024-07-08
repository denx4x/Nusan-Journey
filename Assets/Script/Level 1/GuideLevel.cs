using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideLevel : MonoBehaviour
{

    public void Next()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

}
