using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickRoomUIScript: MonoBehaviour
{
    [SerializeField] RoomDisplayScript roomDisplayPrefab;
    [SerializeField] Transform scrollViewContent;
    [SerializeField] TMP_InputField idText;
    private void Start()
    {
        OnRefresh();
    }

    public void OnRefresh()
    {
        string token = GameManager.Instance.accessToken; // Ensure you have the token from GameManager

        PacketSender.Instance.GetRooms(token, (roomsData) =>
        {
            if (roomsData != null)
            {
                // Populate the UI fields with the fetched profile data
                UpdateUI(roomsData);
            }
            else
            {
                Debug.LogError("Failed to load profile data.");
            }
        });
    }

    public void UpdateUI(List<Room> rooms)
    {
        for(int i = 0; i < scrollViewContent.childCount; i++)
        {
            scrollViewContent.GetChild(i).gameObject.SetActive(i < rooms.Count);
        }
        for(int i = 0; i < rooms.Count; i++)
        {
            RoomDisplayScript roomDisplay = (i < scrollViewContent.childCount) ?
                scrollViewContent.GetChild(i).GetComponent<RoomDisplayScript>()
                : Instantiate(roomDisplayPrefab, scrollViewContent);
            roomDisplay.Init(rooms[i]);
        }
    }

    public void Join()
    {
        SocketIOManager.Instance.JoinRoom(idText.text);
    }
}

