using UnityEngine;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    public GameObject FieldMapPrefab = null;
    public GameObject Ship4Prefab = null;
    public GameObject Ship3Prefab = null;
    public GameObject Ship2Prefab = null;
    public GameObject Ship1Prefab = null;

    public GameObject OverlayManager;

    private GameObject MyFieldMap;
    private GameObject OpFieldMap;

    private int RemainingTime = Constants.ARRANGE_SHIP_TIME;

    // Start is called before the first frame update
    void Start()
    {
        // Show BEGIN overlay
        OverlayManager.GetComponent<OverlayManager>().Open("BATTLE BEGIN", 3);

        // Init my map
        MyFieldMap = Instantiate(
            FieldMapPrefab,
            transform.position,
            Quaternion.identity
        );
        FieldMapController myFieldMapController = MyFieldMap.GetComponent<FieldMapController>();
        myFieldMapController.LeftOffset = -125.5f;
        myFieldMapController.BottomOffset = -350.0f;
        myFieldMapController.Init();
        GameObject Ship141 = Instantiate(
            Ship4Prefab,
            new Vector3(45, 9, 45),
            Ship4Prefab.transform.rotation
        );
        Ship141.GetComponent<ShipController>().FieldMap = MyFieldMap;
        Ship141.GetComponent<ShipController>().id = 0;
        GameObject Ship131 = Instantiate(
            Ship3Prefab,
            new Vector3(70, 9, 45),
            Ship3Prefab.transform.rotation
        );
        Ship131.GetComponent<ShipController>().FieldMap = MyFieldMap;
        Ship131.GetComponent<ShipController>().id = 1;

        // Init op map
        OpFieldMap = Instantiate(
            FieldMapPrefab,
            transform.position,
            Quaternion.identity
        );
        FieldMapController opFieldMapController = MyFieldMap.GetComponent<FieldMapController>();
        opFieldMapController.LeftOffset = 34.5f;
        opFieldMapController.BottomOffset = -350.0f;
        opFieldMapController.Init();
        GameObject Ship241 = Instantiate(
            Ship4Prefab,
            new Vector3(45, 9, 45),
            Ship4Prefab.transform.rotation
        );
        Ship241.GetComponent<ShipController>().FieldMap = OpFieldMap;
        Ship241.GetComponent<ShipController>().id = 0;
        GameObject Ship231 = Instantiate(
            Ship3Prefab,
            new Vector3(70, 9, 45),
            Ship3Prefab.transform.rotation
        );
        Ship231.GetComponent<ShipController>().FieldMap = OpFieldMap;
        Ship231.GetComponent<ShipController>().id = 1;
    }
}
