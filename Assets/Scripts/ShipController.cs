﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

class ShipController : MonoBehaviour
{
    bool Dragging = false;
    float ZPosition;
    Vector3 InitialPosition;
    public GameObject root;
    public Direction direction;
    public int length;
    public int id;
    public GameObject FieldMap;

    [SerializeField]
    public UnityEvent OnBeginDrag;
    [SerializeField]
    public UnityEvent OnEndDrag;

    private void Start()
    {
        Dragging = false;
        ZPosition = Camera.main.WorldToScreenPoint(transform.position).z;
        InitialPosition = transform.position;
        SetDirection(4);
    }

    private void Update()
    {
        if (Dragging)
        {
            Vector3 position = new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                ZPosition
            );
            // TODO: offset
            transform.position = Camera.main.ScreenToWorldPoint(position);
            FieldMapController fieldMapColtroller = FieldMap.GetComponent<FieldMapController>();
            Vector2 pos = fieldMapColtroller.GetCellPosFromShipPos(transform.position);
            fieldMapColtroller.CheckValidShipPos(gameObject, pos);
        }
    }

    private void OnMouseDown()
    {
        if (!Dragging)
        {
            BeginDrag();
        }
    }

    private void OnMouseUp()
    {
        EndDrag();
    }

    void BeginDrag()
    {
        OnBeginDrag.Invoke();
        Dragging = true;
        FieldMap.GetComponent<FieldMapController>().RemoveShipFromArray(gameObject);
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
            transform.position = InitialPosition;
            fieldMapColtroller.RemoveShipFromArray(gameObject);
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
        }
        fieldMapColtroller.ResetCellStatus();
    }

    public void SetDirection(int dir)
    {
        direction = new Direction(dir);
    }
}