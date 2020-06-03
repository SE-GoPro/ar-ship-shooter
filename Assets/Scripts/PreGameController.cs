using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreGameController : MonoBehaviour
{
    public GameObject FieldMapPrefab = null;
    public GameObject Ship4Prefab = null;
    public GameObject Ship3Prefab = null;
    public GameObject Ship2Prefab = null;
    public GameObject Ship1Prefab = null;

    private GameObject FieldMap;


    // Start is called before the first frame update
    void Start()
    {
        FieldMap = Instantiate(
            FieldMapPrefab,
            transform.position,
            Quaternion.identity
        );
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

        PreGameStateManager.Init();
        PreGameStateManager.instance.ChangeState(State.ARRANGE_SHIPS);
        InvokeRepeating("UpdateRemainingTime", 1.0f, 1.0f);
    }

    private void UpdateRemainingTime()
    {
        PreGameStateManager.instance.RemainingTime--;
        GameObject timer = GameObject.FindGameObjectWithTag("TIMER");
        timer.GetComponent<TextMeshProUGUI>().text = PreGameStateManager.instance.RemainingTime.ToString() + " s";

        if (PreGameStateManager.instance.RemainingTime <= 0)
        {
            CancelInvoke("UpdateRemainingTime");
            if (!PreGameStateManager.instance.SelfReady || !PreGameStateManager.instance.OpReady)
            {
                Debug.LogError("Someone is not ready");
                return;
            }
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
            PreGameStateManager.instance.ChangeState(State.NULL);
            StateManager.instance.ChangeState(State.IN_GAME);
        }
    }


}
