using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayUIScript : Singleton<GamePlayUIScript>
{
    [SerializeField] private Text readyText;
    [SerializeField] ProfileButtonDisplay urProfile, enemyProfile;
    [SerializeField]
    private GameObject urProfileButton, enemyProfileButton,
        readyButton, urReadyIcon, enemyReadyIcon;
    private bool isReady;
    [SerializeField] Text idText;

    public void Leave()
    {
        SocketIOManager.Instance.LeaveRoom();
        SceneManager.LoadScene("PickRoom");
    }

    private void Start()
    {
        isReady = false;
        idText.text = "ID: "+ GameManager.Instance.RoomData.id;
        Refresh();
    }

    public void Refresh()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // Kích hoạt nút hồ sơ
            urProfileButton.SetActive(true);
            urProfile.Init(GameManager.Instance.user);

            enemyProfileButton.SetActive(false);
            readyButton.SetActive(false);

            if (GameManager.Instance.RoomData != null && GameManager.Instance.RoomData.userProfiles != null
                && GameManager.Instance.RoomData.userProfiles.Count > 1)
            {
                enemyProfileButton.SetActive(true);
                readyButton.SetActive(true);

                var enemyIndex = GameManager.Instance.RoomData.userProfiles[0].id != GameManager.Instance.user.userId ? 0 : 1;
                var friendIndex = 1 - enemyIndex;

                enemyReadyIcon.SetActive(GameManager.Instance.RoomData.userProfiles[enemyIndex].status == "ready");
                enemyProfile.Init(GameManager.Instance.RoomData.userProfiles[enemyIndex].userProfile);
                urReadyIcon.SetActive(GameManager.Instance.RoomData.userProfiles[friendIndex].status == "ready");
            }
        });
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