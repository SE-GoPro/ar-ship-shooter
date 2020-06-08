using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Connection : WifiDirectBase
{
    static Connection instance = null;

    public GameObject MainMenu = null;
    AndroidJavaObject mWiFiManager;
    public string MyId = SystemInfo.deviceUniqueIdentifier;
    public string OpId = null;
    public bool Connected = false;
    public bool isHost = false;

    public static Connection Instance
    {
        get
        {
            return instance ? instance : (instance = (new GameObject("ConnectionObject")).AddComponent<Connection>());
        }
    }

    public void Initialize()
    {
        base.Initialize(this.gameObject.name);
        Logger.Log("Connection: initialized");
    }

    public bool setWifiEnabled(bool enabled)
    {
        try
        {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                {
                    return wifiManager.Call<bool>("setWifiEnabled", enabled);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
        return false;
    }

    public bool isWifiEnabled()
    {
        try
        {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                {
                    return wifiManager.Call<bool>("isWifiEnabled");
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
        return false;
    }

    //when the WifiDirect services is connected to the phone, begin broadcasting and discovering services
    public override void OnServiceConnected()
    {
        Logger.Log("Connection: OnServiceConnected");
        base.BroadcastService(Constants.WFDR_NAMESPACE, new Dictionary<string, string>());
        base.DiscoverServices();
    }

    //On finding a service
    public override void OnServiceFound(string addr)
    {
        Logger.Log("Connection: OnServiceFound - " + addr);
        // Auto-connect
        MakeConnection(addr);
    }

    //When the button is clicked, connect to the service at its address
    private void MakeConnection(string addr)
    {
        base.ConnectToService(addr);
        Logger.Log("Connection: ConnectToService");
    }

    //When connected
    public override void OnConnect()
    {
        if (!Connected)
        {
            Connected = true;
            base.StopDiscovering();
            Logger.Log("Connection: StopDiscovering");
            base.Send(new Message(MessageTypes.SEND_ID, MyId));
        }
    }

    //When recieving a new message, parse the color and set it to the cube
    public override void OnReceiveMessage(string textMessage)
    {
        Logger.Log("Conection: OnReceiveMessage - " + textMessage);
        Message message = base.OnReceive(textMessage);

        if (message.type != null)
        {
            switch(message.type)
            {
                case (MessageTypes.SEND_ID):
                    {
                        OpId = message.data;
                        if (String.Compare(MyId, OpId) < 0)
                        {
                            isHost = false;
                            base.Send(new Message(MessageTypes.GRANT_HOST, null));
                        }
                        break;
                    }

                case (MessageTypes.GRANT_HOST):
                    {
                        isHost = true;
                        GameManager.Instance.isHost = true;
                        GameManager.Instance.StartGame();
                        break;
                    }

                case (MessageTypes.STATE_CHANGE):
                    {
                        GameState newState = JsonUtility.FromJson<GameState>(message.data);
                        GameManager.Instance.OnRequestChangeState(newState);
                        break;
                    }

                default: break;
            }
        }
    }

    //Kill Switch
    public override void OnServiceDisconnected()
    {
        Stop();
    }

    //Kill Switch
    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            this.OnServiceDisconnected();
            Connected = false;
        }
    }

    public void Stop()
    {
        base.Terminate();
        Connected = false;
    }
}
