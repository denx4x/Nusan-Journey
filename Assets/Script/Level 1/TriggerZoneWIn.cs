using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TriggerZoneWIn : MonoBehaviour
{
    public GameObject show;
    public TextMeshProUGUI uiText; // UI Text untuk pesan "Butuh satu orang lagi untuk menyelesaikan permainan"
    public string player1Tag = "Player"; // Customizable tag for Player 1 in the Inspector
    public string player2Tag = "Player2"; // Customizable tag for Player 2 in the Inspector

    private bool player1InZone = false;
    private bool player2InZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(player1Tag))
        {
            player1InZone = true;
        }
        else if (other.CompareTag(player2Tag))
        {
            player2InZone = true;
        }

        CheckWinCondition();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(player1Tag))
        {
            player1InZone = false;
        }
        else if (other.CompareTag(player2Tag))
        {
            player2InZone = false;
        }

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (player1InZone && player2InZone)
        {
            show.SetActive(true);
            uiText.gameObject.SetActive(false); // Sembunyikan pesan saat menang
            Time.timeScale = 0;
        }
        else if (player1InZone || player2InZone)
        {
            uiText.gameObject.SetActive(true);
            uiText.text = "Butuh satu orang lagi untuk menyelesaikan permainan";
        }
        else
        {
            uiText.gameObject.SetActive(false); // Sembunyikan pesan jika tidak ada yang di zona
        }
    }
}
