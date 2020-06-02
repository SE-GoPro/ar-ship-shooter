using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public int row = 0;
    public int col = 0;
    public CellStatus Status = CellStatus.NORMAL;
    // Start is called before the first frame update
    void Start()
    {
        ChangeStatus(CellStatus.NORMAL);
    }

    // Update is called once per frame
    public void ChangeStatus(CellStatus status)
    {
        Status = status;
        Color color;
        switch (status)
        {
            case CellStatus.NORMAL:
                {
                    ColorUtility.TryParseHtmlString(Constants.CELL_COLOR_NORMAL, out color);
                    break;
                }

            case CellStatus.VALID:
                {
                    ColorUtility.TryParseHtmlString(Constants.CELL_COLOR_VALID, out color);
                    break;
                }

            case CellStatus.INVALID:
                {
                    ColorUtility.TryParseHtmlString(Constants.CELL_COLOR_INVALID, out color);
                    break;
                }

            default:
                {
                    color = new Color(0, 0, 0);
                    break;
                }
        }
        GetComponent<Renderer>().material.color = color;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
    }
}
