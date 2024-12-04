using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomDisplayScript : MonoBehaviour
{
    [SerializeField] Transform playersTransform;
    [SerializeField] GameObject fightIcon;
    Room room;
    public void Init(Room room)
    {
        this.room = room;
        for (int i = 0; i< room.playerIds.Count; i++)
        {
            playersTransform.GetChild(i).gameObject.SetActive(true);
        }
        fightIcon.SetActive(room.status == "playing");
    }

    public void JoinRoom()
    {
        SocketIOManager.Instance.JoinRoom(room.id);
        SceneManager.LoadScene("GamePlay");
    }
}

