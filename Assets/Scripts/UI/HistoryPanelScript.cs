using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryPanelScript : MonoBehaviour
{
    List<GameHistory> gameHistories;
    [SerializeField]MatchDisplayScript matchPrefab;
    [SerializeField] Transform parentTf;
    private void Start()
    {
        for (int i = 0; i < parentTf.childCount; i++) parentTf.GetChild(i).gameObject.SetActive(false);
        string token = GameManager.Instance.accessToken; // Ensure you have the token from GameManager

        PacketSender.Instance.FetchHistory(token, (profileData) =>
        {
            if (profileData != null)
            {
                gameHistories = profileData;
                // Populate the UI fields with the fetched profile data
                foreach(GameHistory gh in gameHistories)
                {
                    MatchDisplayScript match = Instantiate(matchPrefab, parentTf);
                    match.Init(gh);
                }
            }
            else
            {
                Debug.LogError("Failed to load history data.");
            }
        });
    }
}
