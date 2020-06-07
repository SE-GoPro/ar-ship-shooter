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
    public string MyId = null;
    public string OpId = null;

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
        Logger.Log("Connected");
        base.BroadcastService(Constants.WFDR_NAMESPACE, new Dictionary<string, string>());
        base.DiscoverServices();
    }

    //On finding a service
    public override void OnServiceFound(string addr)
    {
        Logger.Log(addr);
        OpId = addr;
        // TODO: check GetSelfId
        Logger.Log("OpId: " + addr);
        MakeConnection(addr);
    }

    //When the button is clicked, connect to the service at its address
    private void MakeConnection(string addr)
    {
        base.ConnectToService(addr);
    }

    //When connected
    public override void OnConnect()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("type", "send_id");
        data.Add("id", OpId);
        base.Send(data);
    }

    //When recieving a new message, parse the color and set it to the cube
    public override void OnReceiveMessage(string message)
    {
        Logger.Log(message);
        Dictionary<string, string> data = base.OnReceive(message);
        string type;
        string newGlobalState;
        string newPreGameState;
        string newInGameState;

        if (data.TryGetValue("type", out type))
        {
            if (type == "state_change")
            {
                if (data.TryGetValue("newGlobalState", out newGlobalState))
                {
                    Logger.Log(newGlobalState);
                    StateManager.instance.ChangeState((State)Int32.Parse(newGlobalState));
                }
                if (data.TryGetValue("newPreGameState", out newPreGameState))
                {
                    Logger.Log(newPreGameState);
                    PreGameStateManager.instance.ChangeState((State)(Int32.Parse(newPreGameState)));
                }
                if (data.TryGetValue("newInGameState", out newInGameState))
                {
                    // TODO
                }
            }
            if (type == "ready")
            {
                PreGameStateManager.instance.OnReady(false);
            }
            if (type == "send_id")
            {
                if (data.TryGetValue("id", out MyId))
                {
                    Logger.Log("MyId: " + MyId);
                    if (String.Compare(OpId, MyId, StringComparison.Ordinal) > 0)
                    {
                        Logger.Log("is host");
                        StateManager.instance.isHost = true;
                        StateManager.instance.ChangeState(State.PRE_GAME);
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("type", "state_change");
                        dict.Add("newGlobalState", ((int)State.PRE_GAME).ToString());
                        base.Send(dict);
                    }
                }
            }
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
