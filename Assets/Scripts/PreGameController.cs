using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameController : MonoBehaviour
{
    public GameObject FieldMapPrefab = null;
    public GameObject Ship4Prefab = null;
    public GameObject Ship3Prefab = null;
    public GameObject Ship2Prefab = null;
    public GameObject Ship1Prefab = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject FieldMap = Instantiate(
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
