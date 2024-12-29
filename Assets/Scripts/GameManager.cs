using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public string accessToken;
    public RoomData RoomData;
    public GameRoom gameRoom;
    public UserProfile user;
    public bool isOwner =>
        RoomData.userProfiles[0].id == user.userId;
    public GameHistoryDetail gameHistoryDetail;
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public bool isRed()
    {
        string id = user.userId.ToString();
        if (gameRoom != null && gameRoom.playerIdToColorMap.ContainsKey(id))
            return gameRoom.playerIdToColorMap[id] == "RED";
        else
        {
            return (gameHistoryDetail.player1Id == user.userId) ? gameHistoryDetail.player1Color == "RED" : gameHistoryDetail.player2Color == "RED";
        }
    }
    void Update()
    {
    }

    
}