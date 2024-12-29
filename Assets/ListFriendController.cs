using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class ListFriendController : MonoBehaviour
{
    private string apiUrl = "http://103.211.206.26:3005/users/friend";
    public FriendListInfo friendListInfoPref;
    public Transform spawnPos;

    public void OnClickGetList(){
        StartCoroutine(GetListFriend(apiUrl));
    }
    public IEnumerator GetListFriend(string apiUrl)
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
                foreach(var x in jsonData.data){
                    FriendListInfo j = Instantiate(friendListInfoPref, spawnPos);
                    j.Init(x);
                }
                //callback?.Invoke(request); // Return the request object
            }
        }
    }

}
