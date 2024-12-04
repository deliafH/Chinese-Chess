using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfilePanelScript : MonoBehaviour
{
    [SerializeField] TMP_InputField nameIF, addressIF, phoneIF;
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
            }
            else
            {
                Debug.LogError("Failed to load profile data.");
            }
        });
    }

    public void LogOut()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
