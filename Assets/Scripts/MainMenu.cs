using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject wifiDisabledObject;
    public GameObject wifiEnabledObject;
    public GameObject connectButton;

    private void Start()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.BACKGROUND_MENU, true);
        DisplayConnectButton(false);
    }

    // Start is called before the first frame update
    public void PlayGame()
    {
        if (Connection.Instance.isWifiEnabled())
        {
            wifiDisabledObject.SetActive(false);
            wifiEnabledObject.SetActive(true);
            Connection.Instance.Initialize();
        }
        else
        {
            wifiDisabledObject.SetActive(true);
            wifiEnabledObject.SetActive(false);
        }
    }

    public void PlayWithBOT()
    {
        Connection.Instance.OfflineMode();
    }

    public void StopPlaying()
    {
        Connection.Instance.StopDiscovering();
        Connection.Instance.Stop();
    }

    public void EnableWIFI()
    {
        bool enabled = Connection.Instance.setWifiEnabled(true);
        if (enabled)
        {
            wifiDisabledObject.SetActive(false);
            wifiEnabledObject.SetActive(true);
        }
    }

    public void ConnectPlayer()
    {
        List<string> deviceList = Connection.Instance.deviceList;
        Logger.Log("Device list: " + deviceList.ToString());
        Connection.Instance.MakeConnection(deviceList[deviceList.Count - 1]);
    }

    public void DisplayConnectButton(bool isActive)
    {
        connectButton.SetActive(isActive);
    }
}
