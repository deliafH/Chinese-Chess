using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


// Class to handle socket communication
public class SocketClient : Singleton<SocketClient>
{

    public IEnumerator SendMessageToServer(string jsonData, string url, Action<UnityWebRequest.Result, string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, jsonData))
        {
            // Set the request header to indicate JSON content
            request.SetRequestHeader("Content-Type", "application/json");

            // Upload the JSON data
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            // Call the callback with the result and response text
            callback?.Invoke(request.result, request.downloadHandler.text);
        }
    }

    public IEnumerator GetDataFromServer(string url, string token, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Set the authorization header with the token
            request.SetRequestHeader("Authorization", "Bearer " + token);

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                callback?.Invoke(null); // Indicate failure
            }
            else
            {
                Debug.Log("Profile Response: " + request.downloadHandler.text);
                callback?.Invoke(request); // Return the request object
            }
        }
    }
}
