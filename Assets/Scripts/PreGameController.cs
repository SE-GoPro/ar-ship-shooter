using UnityEngine;
using UnityEngine.UI;

public class PreGameController : MonoBehaviour
{
    public GameObject FieldMapPrefab = null;
    public GameObject Ship4Prefab = null;
    public GameObject Ship3Prefab = null;
    public GameObject Ship2Prefab = null;
    public GameObject Ship1Prefab = null;

    private GameObject FieldMap;

    public GameObject WaterHigh = null;
    public GameObject WaterLow = null;
    public GameObject SettingsManager = null;

    private int RemainingTime = Constants.ARRANGE_SHIP_TIME;

    // Start is called before the first frame update
    void Start()
    {
        FieldMap = Instantiate(
            FieldMapPrefab,
            transform.position,
            Quaternion.identity,
            gameObject.transform
        );
        FieldMap.GetComponent<FieldMapController>().SceneController = gameObject;
        FieldMap.GetComponent<FieldMapController>().Init();
        InitShip(0, 4, new Vector3(18, 9, 34.5f));
        InitShip(1, 3, new Vector3(38, 9, 34.5f));
        InitShip(2, 3, new Vector3(58, 9, 34.5f));
        InitShip(3, 2, new Vector3(78, 9, 34.5f));
        InitShip(4, 2, new Vector3(18, 9, -12.0f));
        InitShip(5, 2, new Vector3(38, 9, -12.0f));
        InitShip(6, 1, new Vector3(58, 9, -12.0f));
        InitShip(7, 1, new Vector3(18, 9, -40.0f));
        InitShip(8, 1, new Vector3(38, 9, -40.0f));
        InitShip(9, 1, new Vector3(58, 9, -40.0f));

        // Set up water quality
        int waterQuality = (int) SettingsManager.GetComponent<SettingsManager>().WaterQuality;
        if (waterQuality >= 2)
        {
            WaterHigh.SetActive(true);
            WaterLow.SetActive(false);
        } else
        {
            WaterHigh.SetActive(false);
            WaterLow.SetActive(true);
        }
        if (waterQuality == 2)
        {
            WaterHigh.GetComponent<Water>().Enabled = false;
        }
        if (waterQuality == 3)
        {
            WaterHigh.GetComponent<Water>().Enabled = true;
        }

        SoundManager.Instance.PlaySound(SoundManager.Sound.BACKGROUND_OCEAN, true);

        // handle play with BOT
        if (!Connection.Instance.isOnline)
        {
            FieldMapController fieldMapCon = FieldMap.GetComponent<FieldMapController>();
            fieldMapCon.AutoArrangeShips();
            string serializedShips = fieldMapCon.GetSerializedShips();
            fieldMapCon.ResetAllShips();
            BOTGameManager.Instance.Ready(serializedShips);
        }

        InvokeRepeating("UpdateRemainingTime", 1.0f, 1.0f);
    }

    private void InitShip(int id, int length, Vector3 initialPos)
    {
        GameObject prefab = Ship1Prefab;
        if (length == 1) prefab = Ship1Prefab;
        if (length == 2) prefab = Ship2Prefab;
        if (length == 3) prefab = Ship3Prefab;
        if (length == 4) prefab = Ship4Prefab;

        GameObject ship = Instantiate(
            prefab,
            initialPos,
            prefab.transform.rotation,
            gameObject.transform
        );
        ship.GetComponent<ShipController>().FieldMap = FieldMap;
        ship.GetComponent<ShipController>().id = id;
        ship.GetComponent<ShipController>().UpdateColor(GameManager.Instance.isHost);
        FieldMap.GetComponent<FieldMapController>().allShips[id] = ship;
    }

    private void UpdateRemainingTime()
    {
        RemainingTime--;
        GameObject timer = GameObject.FindGameObjectWithTag("TIMER");
        timer.GetComponent<Text>().text = "( " + RemainingTime.ToString() + "s )";

        if (RemainingTime <= 0)
        {
            CancelInvoke("UpdateRemainingTime");
            Logger.Log("Time's up!");
            // TODO: handle time's up
        }
    }

    public void Ready()
    {
        GameObject[] shipArr = FieldMap.GetComponent<FieldMapController>().shipArr;
        bool allSet = true;
        foreach (GameObject ship in shipArr)
        {
            if (ship == null) allSet = false;
        }
        if (!allSet)
        {
            Debug.LogError("Should set up all ships.");
        } else
        {
            CancelInvoke("UpdateRemainingTime");
            string serializedShips = FieldMap.GetComponent<FieldMapController>().GetSerializedShips();
            GameManager.Instance.Ready(serializedShips);
        }
    }

    public void AutoGenShip()
    {
        FieldMap.GetComponent<FieldMapController>().AutoArrangeShips();
    }
}
