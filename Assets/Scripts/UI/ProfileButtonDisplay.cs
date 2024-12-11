using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileButtonDisplay : MonoBehaviour
{
    [SerializeField] Text textName;
    [SerializeField] Image avatar;
    UserProfile user;
    public void Init(UserProfile user)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            this.user = user;
            textName.text = user.fullName;
            StartCoroutine(LoadImage(GameManager.Instance.user.avatar, avatar));
        });

    }

    private IEnumerator LoadImage(string imageUrl, Image avatarImage)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error loading image: " + www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                avatarImage.sprite = sprite;
            }
        }
    }

    public void OpenProfile()
    {

    }
}
