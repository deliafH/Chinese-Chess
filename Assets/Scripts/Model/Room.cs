using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class Room
{
    public string id;
    public List<int> playerIds;
    public int ownerId;
    public string status;
}
[System.Serializable]
public class RoomData:Room
{
    public string createdAt;
    public List<UserProfileData> userProfiles;

}
[System.Serializable]
public class ListRoomData
{
    public RoomData[] rooms; // Change to array as the input is an array
}