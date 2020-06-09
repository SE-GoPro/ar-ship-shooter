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

    private int RemainingTime = Constants.ARRANGE_SHIP_TIME;

    // Start is called before the first frame update
    void Start()
    {
        FieldMap = Instantiate(
            FieldMapPrefab,
            transform.position,
            Quaternion.identity
        );
        FieldMap.GetComponent<FieldMapController>().Init();
        GameObject Ship41 = Instantiate(
            Ship4Prefab,
            new Vector3(45, 9, 45),
            Ship4Prefab.transform.rotation
        );
        Ship41.GetComponent<ShipController>().FieldMap = FieldMap;
        Ship41.GetComponent<ShipController>().id = 0;
        GameObject Ship31 = Instantiate(
            Ship3Prefab,
            new Vector3(70, 9, 45),
            Ship3Prefab.transform.rotation
        );
        Ship31.GetComponent<ShipController>().FieldMap = FieldMap;
        Ship31.GetComponent<ShipController>().id = 1;

        InvokeRepeating("UpdateRemainingTime", 1.0f, 1.0f);
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


}
