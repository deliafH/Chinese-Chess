using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[Serializable]
public class GameHistory
{
    public int id;
    public string roomId;
    public string gameId;
    public int player1Id;
    public int player2Id;
    public int winnerId;
    public string player1Color;
    public string player2Color;
    public string startTime;
    public string endTime;
    public string createdAt;
    public string GetFormattedStartTime()
    {
        // Parse the ISO 8601 date string
        DateTime parsedStartTime = DateTime.Parse(startTime, null, DateTimeStyles.RoundtripKind);

        // Return formatted start time
        return parsedStartTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[Serializable]
public class HistoryApiResponse
{
    public int statusCode;
    public List<GameHistory> data; // Use List<GameHistory> to match the JSON structure
}
[Serializable]
public class HistoryDetailApiResponse
{
    public int statusCode;
    public GameHistoryDetail data; // Use List<GameHistory> to match the JSON structure
}

[Serializable]
public class GameHistoryDetail:GameHistory
{
    public List<GameMove> gameMoves;
    public UserProfile player1, player2;
}
[Serializable]
public class GameMove {
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;
}