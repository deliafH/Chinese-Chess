﻿
using System;

[System.Serializable]
public class UserProfileData
{
    public int id;
    public string status;
    public UserProfile userProfile;
}

[System.Serializable]
public class UserProfile
{
    public string address;
    public string fullName;
    public string phoneNumber;
    public int userId;
    public string avatar;
    public int totalWins;
    public int totalLoses;
    public int totalGames;
}

[System.Serializable]
public class UserProfileDataWrapper
{
    public UserProfileData[] userProfiles;
}

[System.Serializable]
public class GameResult
{
    public UserProfileData winner;
    public UserProfileData loser;
}


[Serializable]
public class InviteApiResponse
{
    public UserProfileData inviter;
    public RoomData room;
}