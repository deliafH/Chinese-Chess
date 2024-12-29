using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FriendListInfo : MonoBehaviour
{
    public TMP_Text nameTxt;
    public TMP_Text addressTxt;
    int id;
    public void Init(UserProfile userProfile)
    {
        this.id = userProfile.userId;
        nameTxt.text = userProfile.fullName;
        addressTxt.text = userProfile.address;
    }

    public void Invite()
    {
        SocketIOManager.Instance.SendInvite(id);
    }
}
