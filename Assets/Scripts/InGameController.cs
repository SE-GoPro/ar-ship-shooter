using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    public GameObject FieldMapPrefab = null;
    public GameObject Ship4Prefab = null;
    public GameObject Ship3Prefab = null;
    public GameObject Ship2Prefab = null;
    public GameObject Ship1Prefab = null;
    public GameObject FireBallPrefab = null;

    public GameObject OverlayManager;
    public HUDController HUDManager;

    private GameObject MyFieldMap;
    private GameObject OpFieldMap;

    private int RemainingTime = Constants.ARRANGE_SHIP_TIME;

    // Start is called before the first frame update
    void Start()
    {
        // Show BEGIN overlay
        OverlayManager.GetComponent<OverlayManager>().Open("BATTLE BEGIN", 2);

        InitSelf();
        InitOp();

        // Delay start turn
        StartCoroutine(DelayStartTurnForSeconds(3));
    }

    void InitSelf()
    {
        // Init my map
        MyFieldMap = Instantiate(
            FieldMapPrefab,
            transform.position,
            Quaternion.identity
        );
        FieldMapController myFieldMapController = MyFieldMap.GetComponent<FieldMapController>();
        myFieldMapController.SceneController = gameObject;
        myFieldMapController.IsMyField = true;
        myFieldMapController.LeftOffset = -125.5f;
        myFieldMapController.BottomOffset = -350.0f;
        myFieldMapController.Init();

        // Init my ships
        Logger.Log("My ships: " + GameManager.Instance.GetShips(true));
        ShipModel[] shipsModels = myFieldMapController.DeserializeShips(GameManager.Instance.GetShips(true));
        foreach (ShipModel shipModel in shipsModels)
        {
            GameObject prefab = Ship1Prefab;
            if (shipModel.length == 1) prefab = Ship1Prefab;
            if (shipModel.length == 2) prefab = Ship2Prefab;
            if (shipModel.length == 3) prefab = Ship3Prefab;
            if (shipModel.length == 4) prefab = Ship4Prefab;

            GameObject ship = Instantiate(
                prefab,
                new Vector3(0, 9, 0),
                prefab.transform.rotation
            );
            ship.GetComponent<ShipController>().FieldMap = MyFieldMap;
            ship.GetComponent<ShipController>().id = shipModel.id;
            ship.GetComponent<ShipController>().FixedPos();
            // We don't need ship's Box Collider for InGame
            Destroy(ship.GetComponent<BoxCollider>());
            ship.GetComponent<ShipController>().SetShipToMap(shipModel.rootRow, shipModel.rootCol, shipModel.dir);
        }
    }

    void InitOp()
    {
        // Init op map
        OpFieldMap = Instantiate(
            FieldMapPrefab,
            transform.position,
            Quaternion.identity
        );
        FieldMapController opFieldMapController = OpFieldMap.GetComponent<FieldMapController>();
        opFieldMapController.SceneController = gameObject;
        opFieldMapController.IsMyField = false;
        opFieldMapController.LeftOffset = 34.5f;
        opFieldMapController.BottomOffset = -350.0f;
        opFieldMapController.Init();

        // Init op ships
        Logger.Log("OP ships: " + GameManager.Instance.GetShips(false));
        ShipModel[] shipsModels = opFieldMapController.DeserializeShips(GameManager.Instance.GetShips(false));
        foreach (ShipModel shipModel in shipsModels)
        {
            GameObject prefab = Ship1Prefab;
            if (shipModel.length == 1) prefab = Ship1Prefab;
            if (shipModel.length == 2) prefab = Ship2Prefab;
            if (shipModel.length == 3) prefab = Ship3Prefab;
            if (shipModel.length == 4) prefab = Ship4Prefab;

            GameObject ship = Instantiate(
                prefab,
                new Vector3(0, 9, 0),
                prefab.transform.rotation
            );
            ship.GetComponent<ShipController>().FieldMap = OpFieldMap;
            ship.GetComponent<ShipController>().id = shipModel.id;
            ship.GetComponent<ShipController>().FixedPos();
            // We don't need ship's Box Collider for InGame
            Destroy(ship.GetComponent<BoxCollider>());
            ship.GetComponent<ShipController>().SetShipToMap(shipModel.rootRow, shipModel.rootCol, shipModel.dir);
            ship.SetActive(false);
        }
    }

    IEnumerator DelayStartTurnForSeconds(float wait)
    {
        yield return new WaitForSeconds(wait);
        HUDManager.ShowHUD(true);
        GameManager.Instance.BeginTurn();
    }

    public void StartTurn(bool isMyTurn)
    {
        string text = isMyTurn ? "YOUR TURN" : "OPPONENT'S TURN";
        Logger.Log("New Turn: " + text);
        OverlayManager.GetComponent<OverlayManager>().Open(text, 2);
        HUDManager.ChangeTitle(isMyTurn);
        if (isMyTurn)
        {
            OpFieldMap.GetComponent<FieldMapController>().Selectable = true;
        }
        // dosomething
    }

    public void Attack()
    {
        CellController cell = OpFieldMap.GetComponent<FieldMapController>().SelectedCell.GetComponent<CellController>();
        OpFieldMap.GetComponent<FieldMapController>().Selectable = false;
        OpFieldMap.GetComponent<FieldMapController>().UnselectSelectedCell();
        TargetModel target = new TargetModel(cell.row, cell.col, Connection.Instance.OpId);
        GameManager.Instance.Attack(target);
    }

    public void DisplayAttack(TargetModel target)
    {
        FieldMapController fieldMap;
        Logger.Log("DisplayAttack - " + target.TargetId + " - " + Connection.Instance.MyId);
        bool isMyField = target.TargetId.Equals(Connection.Instance.MyId);
        if (isMyField)
        {
            fieldMap = MyFieldMap.GetComponent<FieldMapController>();
        }
        else
        {
            fieldMap = OpFieldMap.GetComponent<FieldMapController>();
        }

        Vector3 cellPosition = fieldMap.GetCellByPos(target.Row, target.Col).transform.position;
        GameObject fireBall = Instantiate(
            FireBallPrefab,
            new Vector3(0, 200, 0) + cellPosition,
            FireBallPrefab.transform.rotation
        );
        fireBall.GetComponent<Rigidbody>().velocity = new Vector3(0, -100, 0);
    }

    public void UpdateHPAndCheckWin(int MyHp, int OpHp)
    {
        HUDManager.UpdateHp(MyHp, true);
        HUDManager.UpdateHp(OpHp, false);
        bool win = OpHp == 0;
        bool lose = MyHp == 0;
        Logger.Log(
            "InGame: UpdateHPAndCheckWin - "
            + "HP(" + OpHp + "," + MyHp + ") - "
            + "win(" + win.ToString() + ") - "
            + "lose(" + lose.ToString() + ")"
        );
        if (win)
        {
            GameManager.Instance.EndGame(true);
            return;
        }
        if (lose)
        {
            GameManager.Instance.EndGame(false);
            return;
        }
        // If not win or lose, change to next turn
        if (GameManager.Instance.CurrentState.Hit)
        {
            GameManager.Instance.BeginTurn();
        }
        else
        {
            GameManager.Instance.ChangeTurn();
        }
    }

    public void HandleWinner(bool isWin)
    {
        OverlayManager.GetComponent<OverlayManager>().Open(isWin ? "YOU WON" : "YOU LOSE");
    }
}
