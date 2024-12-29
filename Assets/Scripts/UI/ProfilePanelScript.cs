using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfilePanelScript : MonoBehaviour
{
    [SerializeField] TMP_InputField nameIF, addressIF, phoneIF;
    [SerializeField] FileBrowserUpdate avatar;
    public string avatarLink;
    private void Start()
    {
        string token = GameManager.Instance.accessToken; // Ensure you have the token from GameManager
        
        PacketSender.Instance.FetchUserProfile(token, (profileData) =>
        {
            if (profileData != null)
            {
                GameManager.Instance.user = profileData;
                // Populate the UI fields with the fetched profile data
                nameIF.text = profileData.fullName;
                addressIF.text = profileData.address;
                phoneIF.text = profileData.phoneNumber;
                avatarLink = profileData.avatar;
                StartCoroutine(avatar.LoadImage(avatarLink));
            }
            else
            {
                Debug.LogError("Failed to load profile data.");
            }
        });
    }

    public void Edit()
    {
        UserProfile userProfile = new UserProfile 
        { fullName = nameIF.text, address = addressIF.text, 
            phoneNumber = phoneIF.text, avatar = avatarLink };
        PacketSender.Instance.UpdateProfile(userProfile);
    }
    public void LogOut()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
