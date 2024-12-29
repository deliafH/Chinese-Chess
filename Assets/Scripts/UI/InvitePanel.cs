using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InvitePanel : Singleton<InvitePanel>
{
    string roomId;
    [SerializeField] TextMeshProUGUI nameTxt, idTxt;
    [SerializeField] GameObject invitePanel;
    public void Init(InviteApiResponse inviteApi)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            invitePanel.SetActive(true);
            this.roomId = inviteApi.room.id;
            nameTxt.text = inviteApi.inviter.userProfile.fullName;
            idTxt.text = "Invite You To Room " + roomId + "!";
        });
    }
    public void Accept()
    {
        SocketIOManager.Instance.JoinRoom(roomId);
    }
}
