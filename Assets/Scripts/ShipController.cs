﻿using System;
using UnityEngine;
using UnityEngine.Events;

class ShipController : MonoBehaviour
{
    public bool Draggable = true;
    bool Dragging = false;
    float ZPosition;
    Vector3 InitialPosition;
    public GameObject root;
    public Direction direction;
    public int length;
    public int id;
    public GameObject FieldMap;
    public GameObject RotateButton;
    public int Attacked = 0;
    public bool Revealed = false;

    [SerializeField]
    public UnityEvent OnBeginDrag;
    [SerializeField]
    public UnityEvent OnEndDrag;

    private void Awake()
    {
        Dragging = false;
        ZPosition = Camera.main.WorldToScreenPoint(transform.position).z;
        InitialPosition = transform.position;
        SetDirection(4);
    }

    private void Update()
    {
        if (Draggable)
        {
            if (Dragging)
            {
                Vector3 position = new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    ZPosition
                );
                transform.position = Camera.main.ScreenToWorldPoint(position);
                FieldMapController fieldMapColtroller = FieldMap.GetComponent<FieldMapController>();
                Vector2 pos = fieldMapColtroller.GetCellPosFromShipPos(transform.position);
                fieldMapColtroller.CheckValidShipPos(gameObject, pos);
            }
        }
    }

    public void Rotate()
    {
        int nextDir = direction.dir - 1;
        if (nextDir < 1)
        {
            nextDir = 4;
        }
        SetDirection(nextDir);
    }

    public void FixedPos()
    {
        Draggable = false;
        RotateButton.gameObject.SetActive(false);
}

    private void OnMouseDown()
    {
        if (Draggable)
        {
            if (!Dragging)
            {
                BeginDrag();
            }
        }
    }

    private void OnMouseUp()
    {
        if (Draggable)
        {
            EndDrag();
        }
    }

    void BeginDrag()
    {
        OnBeginDrag.Invoke();
        Dragging = true;
        FieldMap.GetComponent<FieldMapController>().RemoveShipFromArray(gameObject);
        SoundManager.Instance.PlaySound(SoundManager.Sound.BOAT_OUT_WATER);
    }

    void EndDrag()
    {
        OnEndDrag.Invoke();
        Dragging = false;
        FieldMapController fieldMapColtroller = FieldMap.GetComponent<FieldMapController>();
        Vector2 pos = fieldMapColtroller.GetCellPosFromShipPos(transform.position);
        bool isValid = fieldMapColtroller.CheckValidShipPos(gameObject, pos);
        if (!isValid)
        {
            ResetShip();
        } else
        {
            GameObject cell = fieldMapColtroller.GetCellByPos(pos);
            Vector3 position = new Vector3(
                    Camera.main.WorldToScreenPoint(cell.transform.position).x,
                    Camera.main.WorldToScreenPoint(cell.transform.position).y,
                    ZPosition
                );
            transform.position = Camera.main.ScreenToWorldPoint(position);
            fieldMapColtroller.SetShipIntoArray(gameObject);
            RotateButton.gameObject.SetActive(false);
        }
        fieldMapColtroller.ResetCellStatus();
        SoundManager.Instance.PlaySound(SoundManager.Sound.BOAT_TO_WATER);
    }

    public void ResetShip()
    {
        FieldMapController fieldMapColtroller = FieldMap.GetComponent<FieldMapController>();
        transform.position = InitialPosition;
        SetDirection(4);
        fieldMapColtroller.RemoveShipFromArray(gameObject);
        RotateButton.gameObject.SetActive(true);
    }

    public void SetShipToMap(int row, int col, int dir)
    {
        FieldMapController fieldMapColtroller = FieldMap.GetComponent<FieldMapController>();
        GameObject cell = fieldMapColtroller.GetCellByPos(row, col);
        transform.position = new Vector3(
            cell.transform.position.x,
            transform.position.y,
            cell.transform.position.z
        );
        SetDirection(dir);
        root = cell;
        fieldMapColtroller.SetShipIntoArray(gameObject);
        RotateButton.gameObject.SetActive(false);
    }

    public void SetDirection(int dir)
    {
        direction = new Direction(dir);
        transform.rotation = direction.rotation;
    }

    public string Serialize()
    {
        ShipModel model = new ShipModel(
            root.GetComponent<CellController>().row,
            root.GetComponent<CellController>().col,
            direction.dir,
            length,
            id
        );
        return JsonUtility.ToJson(model);
    }

    public void Deserialize(string json)
    {
        FieldMapController fieldMapColtroller = FieldMap.GetComponent<FieldMapController>();
        ShipModel model = JsonUtility.FromJson<ShipModel>(json);
        this.root = fieldMapColtroller.mapArr[model.rootRow, model.rootCol];
        this.direction = new Direction(model.dir);
        this.length = model.length;
        this.id = model.id;
    }

    public void UpdateColor(bool isHost)
    {
        Color color;
        string colorCode = isHost ? Constants.SHIP_COLOR_BLUE : Constants.SHIP_COLOR_RED;
        ColorUtility.TryParseHtmlString(colorCode, out color);
        GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
    }

    public void Reveal()
    {
        gameObject.SetActive(true);
        Revealed = true;
    }
}