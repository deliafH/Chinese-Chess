using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
public class AddfriendController : MonoBehaviour
{
    public TMP_InputField phoneNumberTxtField;
    public FriendInfo friendInfo;
    private string friendIDNumber;
    public string phoneNumber;
    public string apiUrl = "http://160.250.135.30:3005/users?phoneNumber=";
    public void SearchFriend(){
        phoneNumber = phoneNumberTxtField.text;
        StartCoroutine(GetUserData(apiUrl + phoneNumber));
    }
    public IEnumerator GetUserData(string apiUrl)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            // Set the authorization header with the token
            request.SetRequestHeader("Authorization", "Bearer " + GameManager.Instance.accessToken);

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                //callback?.Invoke(null); // Indicate failure
            }
            else
            {
                Debug.Log("Profile Response: " + request.downloadHandler.text);
                var jsonData = JsonUtility.FromJson<Response>(request.downloadHandler.text);
                friendInfo.gameObject.SetActive(true);
                friendInfo.friendName.text = jsonData.data[0].fullName;
                friendInfo.friendAddress.text = jsonData.data[0].address;
                friendInfo.friendPhoneNumber.text = jsonData.data[0].phoneNumber;
                friendIDNumber = jsonData.data[0].userId.ToString();
                //callback?.Invoke(request); // Return the request object
            }
        }
    }
    public void AddFriend(){
        friendInfo.AddFriend(friendIDNumber);
    }
}

[System.Serializable]
public class Response
{
    public int statusCode;
    public UserProfile[] data;
}