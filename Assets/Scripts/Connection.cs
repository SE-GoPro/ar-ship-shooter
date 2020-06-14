using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Connection : WifiDirectBase
{
    static Connection instance = null;

    public string MyId = null;
    public string OpId = null;
    public bool Connected = false;
    public bool isHost = false;
    public bool isOnline = true;

    public List<string> deviceList;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        MyId = SystemInfo.deviceUniqueIdentifier;
    }

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
        Logger.Log("Connection: Initializing");
    }

    public void OfflineMode()
    {
        Logger.Log("Connection: Starting Offline mode");
        Reset();
        isOnline = false;
        isHost = true;
        GameManager.Instance.isHost = true;
        OpId = "BOT_ID";
        GameManager.Instance.StartGame();
    }

    public void Reset()
    {
        Logger.Log("Connection: Reset");
        OpId = null;
        deviceList = new List<string>();
        Connected = false;
        isHost = false;
        isOnline = true;
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
        Logger.Log("Connection: Wifi-direct service initialized");
        Reset();
        ShoutAndListen();
    }

    public void ShoutAndListen()
    {
        base.BroadcastService(Constants.WFDR_NAMESPACE, new Dictionary<string, string>());
        base.DiscoverServices();
        Logger.Log("Connection: ShoutAndListen at " + base.GetDeviceAddress());
    }

    //On finding a service
    public override void OnServiceFound(string addr)
    {
        Logger.Log("Connection: OnServiceFound - " + addr);
        // Ignore if same as own device
        if (base.GetDeviceAddress().Equals(addr))
        {
            Logger.Log("Connection: ERROR same device, ignoring " + addr);
            return;
        }
        deviceList.Add(addr);
        try
        {
            GameObject.FindGameObjectWithTag(Tags.SCENE_CONTROLLER).GetComponent<MainMenu>().DisplayConnectButton(true);
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }

    //When the button is clicked, connect to the service at its address
    public void MakeConnection(string addr)
    {
        Logger.Log("Connection: Making connection to " + addr);
        base.ConnectToService(addr);
    }

    //When connected
    //public override void OnConnect(string addr)
    public override void OnConnect()
    {
        string addr = "";
        if (!Connected)
        {
            Logger.Log("Connection: OnConnect - Connected to " + addr);
            Connected = true;
            base.StopDiscovering();
            Logger.Log("Connection: StopDiscovering");
            StartCoroutine(DelaySendId(1));
        }
        else
        {
            Logger.Log("Connection: OnConnect - Connected to " + addr + " before, ignoring...");
        }
    }

    IEnumerator DelaySendId(float wait)
    {
        yield return new WaitForSeconds(wait);
        base.Send(new Message(MessageTypes.SEND_ID, MyId));
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
                            StartCoroutine(DelayForOpReceiveId(2));
                        }
                        break;
                    }

                case (MessageTypes.GRANT_HOST):
                    {
                        if (OpId == null)
                        {
                            Logger.Log("Connection: retrieve OpId from GRANT_HOST");
                            OpId = message.data;
                        }
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

    IEnumerator DelayForOpReceiveId(float wait)
    {
        yield return new WaitForSeconds(wait);
        base.Send(new Message(MessageTypes.GRANT_HOST, MyId));
    }

    //Kill Switch
    public override void OnServiceDisconnected()
    {
        Logger.Log("Connection: OnServiceDisconnected");
        Stop();
    }

    //Kill Switch
    public void OnApplicationPause(bool pause)
    {
        Logger.Log("Connection: OnApplicationPause/Resume - " + pause.ToString());
        if (pause)
        {
            this.OnServiceDisconnected();
            Connected = false;
        }
        else
        {
            // TODO: resume
        }
    }

    public void Stop()
    {
        Logger.Log("Connection: Terminating...");
        base.Terminate();
        Connected = false;
    }
}
