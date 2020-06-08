using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject wifiDisabledObject;
    public GameObject wifiEnabledObject;

    // Start is called before the first frame update
    void Start()
    {
        //StateManager.Init();
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
        //StateManager.instance.ChangeState(State.FINDING_NEARBY);
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
}
