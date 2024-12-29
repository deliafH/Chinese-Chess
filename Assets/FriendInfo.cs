using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
public class FriendInfo : MonoBehaviour
{
    public TMP_Text friendName;
    public TMP_Text friendAddress;
    public TMP_Text friendPhoneNumber;
    private string apiUrl = "http://103.211.206.26:3005/users/friend";
    public void AddFriend(string _friendId){
        FriendRequest requestData = new FriendRequest
        {
                friendId = _friendId
        };
        StartCoroutine(PostJson(requestData));
    }

    IEnumerator PostJson(FriendRequest requestData)
    {
        // Convert the data object to a JSON string
        string jsonData = JsonUtility.ToJson(requestData);

        // Create UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {GameManager.Instance.accessToken}");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Request successful: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Request failed: {request.error} - {request.downloadHandler.text}");
        }
    }
}
public class FriendRequest
{
    public string friendId;
}
