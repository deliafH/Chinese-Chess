using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanelScript : MonoBehaviour
{
    public void Surrender()
    {
        SocketIOManager.Instance.SendSurrender();
    }

    public void RequestForPeace()
    {
        SocketIOManager.Instance.SendPeace();
    }
}
