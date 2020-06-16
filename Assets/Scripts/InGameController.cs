using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


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

    public GameObject WaterHigh = null;
    public GameObject WaterLow = null;
    public GameObject SettingsManager = null;

    public GameObject ARSessionPrefab;
    private GameObject ARSessionObj;
    private float PhysicsMultiplier;
    private float BottomOffset = -350.0f;

    private int RemainingTime = Constants.ARRANGE_SHIP_TIME;

    // Start is called before the first frame update
    void Start()
    {
        Logger.Log("InGameController: Start - " + SceneManager.GetActiveScene().name);
        // Check if AR
        if (SceneManager.GetActiveScene().buildIndex == Constants.SCENE_INDEX_INGAME)
        {
            if (SettingsManager.GetComponent<SettingsManager>().DefaultARView == true)
            {
                StartCoroutine(CheckAR());
            }
            else
            {
                // Init normal scene
                Init(false);
            }
        } else
        {
            // Init AR scene
            Init(true);
        }
    }

    private IEnumerator CheckAR()
    {
        if (!ARSessionObj)
        {
            ARSessionObj = Instantiate(ARSessionPrefab);
            yield return new WaitForSecondsRealtime(1);
        }
        if (!Application.isEditor)
        {
            Debug.Log("ARSession.state: " + ARSession.state);
            switch (ARSession.state)
            {
                case ARSessionState.CheckingAvailability:
                    Debug.Log("Still Checking Availability...");
                    ARSession.stateChanged += ARSessionStateChanged;
                    break;
                case ARSessionState.NeedsInstall:
                    Debug.Log("Supported, not installed, requesting installation");
                    //TODO: Request ARCore services apk installation and install only if user allows
                    StartCoroutine(InstallARCoreApp());
                    break;
                case ARSessionState.Installing:
                    Debug.Log("Supported, apk installing");
                    StartCoroutine(InstallARCoreApp());
                    break;
                case ARSessionState.Ready:
                    Debug.Log("Supported and installed");
                    NextStep(true);
                    break;
                case ARSessionState.SessionInitializing:
                    Debug.Log("Supported, apk installed. SessionInitializing...");
                    NextStep(true);
                    break;
                case ARSessionState.SessionTracking:
                    Debug.Log("Supported, apk installed. SessionTracking...");
                    NextStep(true);
                    break;
                default:
                    Debug.Log("Unsupported, Device Not Capable");
                    NextStep(false);
                    break;
            }
        }
        else
        {
            Debug.Log("Unity editor: AR not supported, Device Not Capable");
            NextStep(false);
        }
    }

    private void ARSessionStateChanged(ARSessionStateChangedEventArgs obj)
    {
        Debug.Log("Inside ARSessionStateChanged delegate...");
        switch (ARSession.state)
        {
            case ARSessionState.CheckingAvailability:
                Debug.Log("Still Checking Availability...");
                break;
            case ARSessionState.NeedsInstall:
                Debug.Log("Supported, not installed, requesting installation");
                //TODO: Request ARCore services apk installation and install only if user allows
                StartCoroutine(InstallARCoreApp());
                break;
            case ARSessionState.Installing:
                Debug.Log("Supported, apk installing");
                StartCoroutine(InstallARCoreApp());
                break;
            case ARSessionState.Ready:
                Debug.Log("Supported and installed");
                NextStep(true);
                break;
            case ARSessionState.SessionInitializing:
                Debug.Log("Supported, apk installed. SessionInitializing...");
                NextStep(true);
                break;
            case ARSessionState.SessionTracking:
                Debug.Log("Supported, apk installed. SessionTracking...");
                NextStep(true);
                break;
            default:
                Debug.Log("Unsupported, Device Not Capable");
                NextStep(false);
                break;
        }
    }

    private IEnumerator InstallARCoreApp()
    {
        yield return ARSession.Install();
        NextStep(true);
    }

    private void NextStep(bool IsARSupported)
    {
        Logger.Log("InGameController: NextStep - " + IsARSupported);
        ARSession.stateChanged -= ARSessionStateChanged;
        if (ARSessionObj)
        {
            Destroy(ARSessionObj);
        }
        if (IsARSupported)
        {
            Logger.Log("InGameController: NextStep - Loading AR scene");
            SceneManager.LoadSceneAsync(Constants.SCENE_INDEX_INGAME_AR);
        } else
        {
            Logger.Log("InGameController: NextStep - AR not supported, continue loading normal scene");
            Init(false);
        }
    }

    private void Init(bool isAR)
    {
        Logger.Log("InGameController: Init - " + (isAR ? "with AR" : "without AR"));
        if (isAR)
        {
            PhysicsMultiplier = 1f;
            BottomOffset = 0.0f;
        }
        else
        {
            PhysicsMultiplier = 1f;
            BottomOffset = -350.0f;
        }
        Physics.gravity = Physics.gravity * PhysicsMultiplier;

        // Set up water quality
        int waterQuality = (int)SettingsManager.GetComponent<SettingsManager>().WaterQuality;
        if (waterQuality >= 2)
        {
            WaterHigh.SetActive(true);
            WaterLow.SetActive(false);
        }
        else
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

        // Show BEGIN overlay
        OverlayManager.GetComponent<OverlayManager>().Open("BATTLE BEGIN", 2);

        InitSelf();
        InitOp();

        // Play sound
        SoundManager.Instance.PlaySound(SoundManager.Sound.BACKGROUND_INGAME, true);
        SoundManager.Instance.PlaySound(SoundManager.Sound.BACKGROUND_OCEAN, true);

        // Delay start turn
        StartCoroutine(DelayStartTurnForSeconds(3));
    }

    void InitSelf()
    {
        // Init my map
        MyFieldMap = Instantiate(
            FieldMapPrefab,
            transform.position,
            Quaternion.identity,
            gameObject.transform
        );
        FieldMapController myFieldMapController = MyFieldMap.GetComponent<FieldMapController>();
        myFieldMapController.SceneController = gameObject;
        myFieldMapController.IsMyField = true;
        myFieldMapController.LeftOffset = -125.5f;
        myFieldMapController.BottomOffset = BottomOffset;
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
                Vector3.zero,
                prefab.transform.rotation,
                myFieldMapController.gameObject.transform
            );
            ship.GetComponent<Transform>().transform.localPosition = new Vector3(0, 9, 0);
            ship.GetComponent<ShipController>().FieldMap = MyFieldMap;
            ship.GetComponent<ShipController>().id = shipModel.id;
            ship.GetComponent<ShipController>().UpdateColor(GameManager.Instance.isHost);
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
            Quaternion.identity,
            gameObject.transform
        );
        FieldMapController opFieldMapController = OpFieldMap.GetComponent<FieldMapController>();
        opFieldMapController.SceneController = gameObject;
        opFieldMapController.IsMyField = false;
        opFieldMapController.LeftOffset = 34.5f;
        opFieldMapController.BottomOffset = BottomOffset;
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
                Vector3.zero,
                prefab.transform.rotation,
                opFieldMapController.gameObject.transform
            );
            ship.GetComponent<Transform>().transform.localPosition = new Vector3(0, 9, 0);
            ship.GetComponent<ShipController>().FieldMap = OpFieldMap;
            ship.GetComponent<ShipController>().id = shipModel.id;
            ship.GetComponent<ShipController>().UpdateColor(!GameManager.Instance.isHost);
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
        HUDManager.ChangeDescription(isMyTurn);
        if (isMyTurn)
        {
            OpFieldMap.GetComponent<FieldMapController>().Selectable = true;
        }
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

        Vector3 cellPosition = fieldMap.GetCellByPos(target.Row, target.Col).transform.localPosition;
        GameObject fireBall = Instantiate(
            FireBallPrefab,
            new Vector3(0, 200, 0),
            FireBallPrefab.transform.rotation,
            fieldMap.gameObject.transform
        );
        Rigidbody rigidbody = fireBall.GetComponent<Rigidbody>();
        Transform transform = fireBall.GetComponent<Transform>();
        rigidbody.mass = rigidbody.mass * PhysicsMultiplier;
        transform.transform.localPosition = new Vector3(0, 200, 0) + cellPosition;
        rigidbody.velocity = new Vector3(0, -100, 0) * PhysicsMultiplier;
        SoundManager.Instance.PlaySound(SoundManager.Sound.FIREBALL_FLY);
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

    public void BOTAttack()
    {
        FieldMapController myFieldMapCon = MyFieldMap.GetComponent<FieldMapController>();
        (int predRow, int predCol) = PredictCell(myFieldMapCon);
        TargetModel target = new TargetModel(predRow, predCol, Connection.Instance.MyId);
        BOTGameManager.Instance.Attack(target);
    }

    // Check if cell is not be fired yet, and relate cells not contain revealed ship
    private bool CheckIfShouldFireCell(FieldMapController fieldMapCon, CellController cellCon)
    {
        // Check if cell is not be fired yet
        if (cellCon.Fired)
        {
            return false;
        }
        // Check if relate cells not contain revealed ship
        (int, int)[] possibleOffsets =
        {
            (-1, -1),
            (-1, 0),
            (-1, 1),
            (0, -1),
            (0, 1),
            (1, -1),
            (1, 0),
            (1, 1),
        };
        foreach ((int offsetRow, int offsetCol) in possibleOffsets)
        {
            int row = offsetRow + cellCon.row;
            int col = offsetCol + cellCon.col;
            if (!fieldMapCon.CheckValidCellCord(row, col))
            {
                continue;
            }
            GameObject relateCell = fieldMapCon.mapArr[row, col];
            GameObject shipAtCell = fieldMapCon.GetShipAtCell(relateCell);
            if (shipAtCell != null && shipAtCell.GetComponent<ShipController>().Revealed)
            {
                return false;
            }
        }
        return true;
    }

    public (int, int) PredictCell(FieldMapController fieldMapCon)
    {
        // handle pick relative cell of the last hit
        List<CellController> hitCellsCon = fieldMapCon.HitCellsCon;
        if (hitCellsCon.Count > 0)
        {
            List<(int, int)> possibleOffsets = new List<(int, int)>();
            possibleOffsets.Add((-1, 0));
            possibleOffsets.Add((1, 0));
            possibleOffsets.Add((0, -1));
            possibleOffsets.Add((0, 1));

            // Remove possible offsets in case there's >=2 cell that in the same row/col
            if (hitCellsCon.Count >= 2)
            {
                if (hitCellsCon[0].row == hitCellsCon[1].row)
                {
                    possibleOffsets.Remove((-1, 0));
                    possibleOffsets.Remove((1, 0));
                }
                else if (hitCellsCon[0].col == hitCellsCon[1].col)
                {
                    possibleOffsets.Remove((0, -1));
                    possibleOffsets.Remove((0, 1));
                }
            }
            possibleOffsets.Shuffle();

            CellController predCellCon = null;
            // Loop through all possible cells
            foreach (CellController hitCellCon in hitCellsCon)
            {
                if (predCellCon != null)
                {
                    break;
                }
                foreach ((int offsetRow, int offsetCol) in possibleOffsets)
                {
                    int row = offsetRow + hitCellCon.row;
                    int col = offsetCol + hitCellCon.col;
                    if (!fieldMapCon.CheckValidCellCord(row, col))
                    {
                        continue;
                    }
                    CellController possibleCellCon = fieldMapCon.mapArr[row, col].GetComponent<CellController>();
                    if (CheckIfShouldFireCell(fieldMapCon, possibleCellCon))
                    {
                        predCellCon = possibleCellCon;
                        break;
                    }
                }
            }

            // Should never go here, because the last hit not destroy ship
            if (predCellCon == null)
            {
                Logger.LogError("BOT error: Should never go here, because the last hit not destroy ship");
                fieldMapCon.HitCellsCon.Clear();
                return PredictCell(fieldMapCon);
            }
            return (predCellCon.row, predCellCon.col);
        }
        // handle pick random cell if last hit destroyed ship, or not hit anything yet
        else
        {
            System.Random rand = new System.Random();

            int randomRow = 0;
            int randomCol = 0;
            bool isValid = false;
            while (!isValid)
            {
                randomRow = rand.Next(Constants.MAP_SIZE);
                randomCol = rand.Next(Constants.MAP_SIZE);
                CellController possibleCell = fieldMapCon.mapArr[randomRow, randomCol].GetComponent<CellController>();
                // If the cell is not fired before -> valid
                isValid = CheckIfShouldFireCell(fieldMapCon, possibleCell);
            }
            return (randomRow, randomCol);
        }
    }
}

static class StringExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}