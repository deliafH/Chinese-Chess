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
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public bool isRed()
    {
        string id = user.userId.ToString();
        return gameRoom.playerIdToColorMap[id] == "RED";
    }
    void Update()
    {
    }

    
}