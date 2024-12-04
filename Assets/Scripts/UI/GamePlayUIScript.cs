using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayUIScript : Singleton<GamePlayUIScript>
{
    [SerializeField] private Text urProfile, enemyProfile, readyText;
    [SerializeField]
    private GameObject urProfileButton, enemyProfileButton,
        readyButton, urReadyIcon, enemyReadyIcon;
    [SerializeField] private Image urAvatarImage, enemyAvatarImage;
    private bool isReady;

    public void Leave()
    {
        SocketIOManager.Instance.LeaveRoom();
        SceneManager.LoadScene("PickRoom");
    }

    private void Start()
    {
        isReady = false;
        Refresh();
    }

    public void Refresh()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // Kích hoạt nút hồ sơ
            urProfileButton.SetActive(true);
            urProfile.text = GameManager.Instance.user.fullName;
            StartCoroutine(LoadImage(GameManager.Instance.user.avatar, urAvatarImage));

            enemyProfileButton.SetActive(false);
            readyButton.SetActive(false);

            if (GameManager.Instance.RoomData != null && GameManager.Instance.RoomData.userProfiles != null
                && GameManager.Instance.RoomData.userProfiles.Count > 1)
            {
                enemyProfileButton.SetActive(true);
                readyButton.SetActive(true);

                var enemyIndex = GameManager.Instance.RoomData.userProfiles[0].id != GameManager.Instance.user.userId ? 0 : 1;
                var friendIndex = 1 - enemyIndex;

                enemyProfile.text = GameManager.Instance.RoomData.userProfiles[enemyIndex].userProfile.fullName;
                enemyReadyIcon.SetActive(GameManager.Instance.RoomData.userProfiles[enemyIndex].status == "ready");
                StartCoroutine(LoadImage(GameManager.Instance.RoomData.userProfiles[enemyIndex].userProfile.avatar, enemyAvatarImage));
                urReadyIcon.SetActive(GameManager.Instance.RoomData.userProfiles[friendIndex].status == "ready");
            }
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

    public void OpenProfilePanel(UserProfile userProfile)
    {
        // Logic to open profile panel should be implemented here
    }

    public void SetReady()
    {
        isReady = !isReady;
        readyText.text = isReady ? "Ready" : "Not Ready";
        SocketIOManager.Instance.SendReady(isReady);
    }

    public void StartGame()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            urReadyIcon.SetActive(false);
            enemyReadyIcon.SetActive(false);
            readyButton.SetActive(false);
        });
    }
}