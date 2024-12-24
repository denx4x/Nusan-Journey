using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private int index;       

    public void PlayGame() {
        SceneManager.LoadScene(index);
        Time.timeScale = 1f;
    }

    public void GameSetting() {
        settingPanel.SetActive(true);
        Time.timeScale = 0f;


        /*settingPanel.SetActive(false);
            Time.timeScale = 1f;*/        
    }

    public void QuitGame() {
        Application.Quit();        
    }

}