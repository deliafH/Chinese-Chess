using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public string accessToken;
    private RoomData roomData;
    public GameRoom gameRoom;
    public RoomData RoomData
    {
        get { return roomData; }
        set { this.roomData = value; }
    }
    public UserProfile user;
    public bool isOwner =>
        roomData.userProfiles[0].id == user.userId;
    public GameHistoryDetail gameHistoryDetail;
    public GameHistory gameHistory;
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public bool isRed()
    {
        string id = user.userId.ToString();
        if(gameRoom != null && gameRoom.playerIdToColorMap.ContainsKey(id))
        return gameRoom.playerIdToColorMap[id] == "RED";
        else
        {
            return (gameHistory.player1Id == user.userId) ? gameHistory.player1Color == "RED" : gameHistory.player2Color == "RED";
        }
    }
    void Update()
    {
    }

    
}