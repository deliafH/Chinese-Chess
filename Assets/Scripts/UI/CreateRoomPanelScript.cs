using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateRoomPanelScript : MonoBehaviour
{
    public void Create()
    {
        SocketIOManager.Instance.CreateRoom();
        SceneManager.LoadScene("GamePlay");
    }
}
