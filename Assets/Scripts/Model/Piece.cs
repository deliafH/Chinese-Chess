using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameRoom
{
    public string gameId;
    public string roomId;
    public int currentPlayerId;
    public List<int> playerIds;
    public List<Piece> board; // Mảng hai chiều cho bàn cờ
    public bool gameOver;
    public Dictionary<string, string> playerIdToColorMap = new Dictionary<string, string>(); // Bản đồ ID người chơi đến màu

    public static int GetPieceAt(Vector2 v)
    {
        if (GameManager.Instance.isRed())
        {
            return (int)(v.x * 9 + v.y);
        }else
        {
            return (int)((9 - v.x) * 9 + v.y);
        }
    }

}

[Serializable]
public class Piece
{
    public string type;
    public string color;
}

[Serializable]
public class CheckMateData
{
    public int x;
    public int y;
    public Piece piece;
}

[Serializable]
public class GameRoomWrapper
{
    public List<GameRoom> gameRooms;
}