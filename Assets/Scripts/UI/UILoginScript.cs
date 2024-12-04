using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILoginScript : MonoBehaviour
{
    [SerializeField] TMP_InputField loginUsername, loginPassword, registerUsername, registerPassword, registerConfirm;
    [SerializeField] GameObject loginPanel, registerPanel;

    public void Login()
    {
        string username = loginUsername.text;
        string password = loginPassword.text;
        PacketSender.Instance.Login(username, password);
    }

    public void Register()
    {
        string username = registerUsername.text;
        string password = registerPassword.text;
        string confirm = registerConfirm.text;

        if (password != confirm)
        {
            Debug.Log("Passwords do not match!");
            return; 
        }

        PacketSender.Instance.Register(username, password, (isOk) =>
        {
            if (isOk)
            {
                registerPanel.SetActive(false);
                loginPanel.SetActive(true);
            }
            else
            {
                Debug.Log("Registration failed.");
            }
        });
    }

}
