using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Connection : WifiDirectBase
{
    public GameObject MainMenu = null;
    AndroidJavaObject mWiFiManager;

    void Start()
    {
        base.Initialize(this.gameObject.name);
        Debug.Log("Finding");
    }

    string GetSelfId()
    {
        string macAddr = "";
        if (mWiFiManager == null)
        {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            {
                mWiFiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
            }
        }
        macAddr = mWiFiManager.Call<AndroidJavaObject>("getConnectionInfo").Call<string>("getMacAddress");
        return macAddr;
    }

    //when the WifiDirect services is connected to the phone, begin broadcasting and discovering services
    public override void OnServiceConnected()
    {
        base.BroadcastService(Constants.WFDR_NAMESPACE, new Dictionary<string, string>());
        base.DiscoverServices();
    }

    //On finding a service
    public override void OnServiceFound(string addr)
    {
        // TODO: check GetSelfId
        if (String.Compare(addr, GetSelfId(), StringComparison.Ordinal) > 0)
        {
            StateManager.instance.isHost = true;
            MakeConnection(addr);
        }
    }

    //When the button is clicked, connect to the service at its address
    private void MakeConnection(string addr)
    {
        base.ConnectToService(addr);
    }

    //When connected
    public override void OnConnect()
    {
        if (StateManager.instance.isHost)
        {
            StateManager.instance.ChangeState(State.PRE_GAME);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("newGlobalState", State.PRE_GAME.ToString());
            base.Send(data);
        }
    }

    //When recieving a new message, parse the color and set it to the cube
    public override void OnReceiveMessage(string message)
    {
        Dictionary<string, string> data = base.OnReceive(message);
        string newGlobalState;
        string newPreGameState;
        string newInGameState;

        if (data.TryGetValue("newGlobalState", out newGlobalState))
        {
            StateManager.instance.ChangeState((State)(Int32.Parse(newGlobalState)));
        }
        if (data.TryGetValue("newPreGameState", out newPreGameState))
        {
            PreGameStateManager.instance.ChangeState((State)(Int32.Parse(newPreGameState)));
        }
        if (data.TryGetValue("newInGameState", out newInGameState))
        {
            // TODO
        }
    }

    //Kill Switch
    public override void OnServiceDisconnected()
    {
        base.Terminate();
    }

    //Kill Switch
    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            this.OnServiceDisconnected();
        }
    }
}
