using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchDisplayScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI idText, timeText;
    GameHistory gameHistory;
    public void Init(GameHistory gameHistory)
    {
        idText.text = gameHistory.gameId;
        timeText.text = gameHistory.GetFormattedStartTime();
        this.gameHistory = gameHistory;
    }

    public void Select()
    {
        string token = GameManager.Instance.accessToken;

        PacketSender.Instance.FetchHistoryDetail(token,gameHistory.id, (profileData) =>
        {
            if (profileData != null)
            {
                SceneManager.LoadScene("MatchHistory");
                GameManager.Instance.gameHistory = this.gameHistory;
            }
            else
            {
                Debug.LogError("Failed to load history data.");
            }
        });
    }
}
