using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject ConnectionPrefab = null;
    private GameObject Connection = null;

    // Start is called before the first frame update
    void Start()
    {
        StateManager.Init();
    }

    // Start is called before the first frame update
    public void PlayGame()
    {
        if (Connection == null)
        {
            Connection = Instantiate(ConnectionPrefab);
            Connection.GetComponent<Connection>().MainMenu = gameObject;
        }
        StateManager.instance.ChangeState(State.FINDING_NEARBY);
    }

    public void Quit()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
