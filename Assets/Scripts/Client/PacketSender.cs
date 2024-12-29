using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PacketSender : Singleton<PacketSender>
{
    string ip = "http://160.250.135.30:3005";
    public void Login(string username, string password)
    {
        var loginRequestData = new LoginRequestData { username = username, password = password };
        string jsonData = JsonUtility.ToJson(loginRequestData);
        string url = ip + "/auth/login";
        StartCoroutine(SocketClient.Instance.SendMessageToServer(jsonData, url, (result, response) =>
        {
            if (result == UnityWebRequest.Result.Success)
            {

                // Deserialize the JSON response to LoginResponse object
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(response);

                // Check if the status code indicates success
                if (loginResponse != null && loginResponse.statusCode == 201)
                {
                    // Save the access token to GameManager
                    GameManager.Instance.accessToken = loginResponse.data.accessToken; // Ensure this accesses the correct property
                    
                    // Optionally load the next scene
                    SceneManager.LoadScene("PickRoom");
                    SocketIOManager.Instance.StartConnection(loginResponse.data.accessToken);
                }
                else
                {
                    Debug.LogError("Login failed or status code not 201.");
                }
            }
            else
            {
                Debug.LogError("Error: " + response);
            }
        }));
    }

    public void Register(string username, string password, Action<bool> callback)
    {
        var loginData = new LoginData { username = username, password = password };
        string jsonData = JsonUtility.ToJson(loginData);
        string url = ip + "/auth/register";

        StartCoroutine(SocketClient.Instance.SendMessageToServer(jsonData, url, (result, response) =>
        {
            bool isOk = result == UnityWebRequest.Result.Success;
            if (isOk)
            {
                Debug.Log("Response: " + response);
            }
            else
            {
                Debug.LogError("Error: " + response);
            }

            // Invoke the callback with the result
            callback?.Invoke(isOk);
        }));
    }

    public void FetchUserProfile(string token, Action<UserProfile> callback)
    {
        string url = ip + "/users/profile";

        StartCoroutine(SocketClient.Instance.GetDataFromServer(url, token, (request) =>
        {
            if (request != null)
            {
                // Deserialize the JSON response to ApiResponse object
                ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);

                // Check if the status code indicates success
                if (apiResponse != null && apiResponse.statusCode == 200)
                {
                    // Invoke the callback with the ProfileData
                    callback?.Invoke(apiResponse.data);
                    Debug.Log(apiResponse);
                }
                else
                {
                    Debug.LogError("Failed to fetch profile data or status code not 200.");
                    callback?.Invoke(null); // Indicate failure
                }
            }
            else
            {
                callback?.Invoke(null); // Indicate failure
            }
        }));
    }

    public void UpdateProfile(UserProfile userProfile)
    {
        string jsonData = JsonUtility.ToJson(userProfile);
        string url = ip + "/users/profile";
        string token = GameManager.Instance.accessToken;

        StartCoroutine(SocketClient.Instance.SendMessageToServer(jsonData, url, token, (request) =>
        {
            if (request != null)
            {
                // Deserialize the JSON response to ApiResponse object
                ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);

                // Check if the status code indicates success
                if (apiResponse != null && apiResponse.statusCode == 201)
                {
                    // Invoke the callback with the ProfileData
                    GameManager.Instance.user = apiResponse.data;
                    Debug.Log(apiResponse);
                }
                else
                {
                    Debug.LogError("Failed to fetch profile data or status code not 200.");
                }
            }
            else
            {
                //callback?.Invoke(null); // Indicate failure
            }

        }));
    }


    public void GetRooms(string token, Action<List<Room>> callback)
    {
        string url = ip + "/games/rooms";
        StartCoroutine(SocketClient.Instance.GetDataFromServer(url, token, (request) =>
        {
            if (request != null)
            {
                // Deserialize the JSON response to ApiResponse object
                GameRoomResponse gameRoomResponse = JsonUtility.FromJson<GameRoomResponse>(request.downloadHandler.text);
                
                // Check if the status code indicates success
                if (gameRoomResponse != null && gameRoomResponse.statusCode == 200)
                {
                    callback?.Invoke(gameRoomResponse.data);
                }
                else
                {
                    Debug.LogError("Failed to fetch profile data or status code not 200.");
                    callback?.Invoke(null); // Indicate failure
                }
            }
            else
            {
                Debug.LogError("Request is null.");
                callback?.Invoke(null); // Indicate failure
            }
        }));
    }

    public void FetchHistory(string token, Action<List<GameHistory>> callback)
    {
        string url = ip + "/game-histories";

        StartCoroutine(SocketClient.Instance.GetDataFromServer(url, token, (request) =>
        {
            if (request != null)
            {
                Debug.Log("Response: " + request.downloadHandler.text);

                // Deserialize the JSON response
                HistoryApiResponse apiResponse = JsonUtility.FromJson<HistoryApiResponse>(request.downloadHandler.text);

                // Log the API response for debugging
                Debug.Log("API Response: " + JsonUtility.ToJson(apiResponse));

                // Check if the status code indicates success
                if (apiResponse != null && apiResponse.statusCode == 200)
                {
                    // Log the data to verify it's populated
                    if (apiResponse.data != null)
                    {
                        Debug.Log("Data Count: " + apiResponse.data.Count);
                        callback?.Invoke(apiResponse.data);
                    }
                    else
                    {
                        Debug.LogError("Data is null.");
                        callback?.Invoke(null); // Indicate failure
                    }
                }
                else
                {
                    Debug.LogError("Failed to fetch profile data or status code not 200.");
                    callback?.Invoke(null); // Indicate failure
                }
            }
            else
            {
                Debug.LogError("Request was null.");
                callback?.Invoke(null); // Indicate failure
            }
        }));
    }
    public void FetchHistoryDetail(string token, int id, Action<GameHistoryDetail> callback)
    {
        string url = ip + "/game-histories/" + id.ToString();

        StartCoroutine(SocketClient.Instance.GetDataFromServer(url, token, (request) =>
        {
            if (request != null)
            {
                Debug.Log("Response: " + request.downloadHandler.text);

                // Deserialize the JSON response
                HistoryDetailApiResponse apiResponse = JsonUtility.FromJson<HistoryDetailApiResponse>(request.downloadHandler.text);

                // Log the API response for debugging
                Debug.Log("API Response: " + JsonUtility.ToJson(apiResponse));

                // Check if the status code indicates success
                if (apiResponse != null && apiResponse.statusCode == 200)
                {
                    // Log the data to verify it's populated
                    if (apiResponse.data != null)
                    {
                        GameManager.Instance.gameHistoryDetail = apiResponse.data;
                        callback?.Invoke(apiResponse.data);
                    }
                    else
                    {
                        Debug.LogError("Data is null.");
                        callback?.Invoke(null); // Indicate failure
                    }
                }
                else
                {
                    Debug.LogError("Failed to fetch profile data or status code not 200.");
                    callback?.Invoke(null); // Indicate failure
                }
            }
            else
            {
                Debug.LogError("Request was null.");
                callback?.Invoke(null); // Indicate failure
            }
        }));
    }

}



[System.Serializable]
public class ApiResponse
{
    public int statusCode;
    public UserProfile data;
}

[System.Serializable]
public class LoginRequestData
{
    public string username;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public int statusCode;
    public LoginResponseData data;
}

[System.Serializable]
public class LoginResponseData
{
    public string accessToken;
}
[System.Serializable]
public class LoginData
{
    public string username;
    public string password;
}
[System.Serializable]
public class GameRoomResponse
{
    public int statusCode;
    public List<Room> data;
}