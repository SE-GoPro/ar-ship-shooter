using UnityEngine;
using System.Collections;

public class RotateButton : MonoBehaviour
{
    public GameObject ship;

    private void OnMouseUpAsButton()
    {
        ship.GetComponent<ShipController>().Rotate();
    }
}
