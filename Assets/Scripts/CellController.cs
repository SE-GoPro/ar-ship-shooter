using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public int row = 0;
    public int col = 0;
    public CellStatus Status = CellStatus.NORMAL;
    public GameObject fieldMap;
    public GameObject CellPlane;
    public GameObject SelectIndicator;
    public GameObject Flame;
    public GameObject Smoke;
    public GameObject Boom;

    public bool Fired = false;
    public bool HasShip = false;

    // Start is called before the first frame update
    void Start()
    {
        ChangeStatus(CellStatus.NORMAL);
    }

    // Update is called once per frame
    public void ChangeStatus(CellStatus status)
    {
        Status = status;
        switch (status)
        {
            case CellStatus.NORMAL:
            case CellStatus.VALID:
            case CellStatus.FIRED:
            case CellStatus.INVALID:
                {
                    SelectIndicator.gameObject.SetActive(false);
                    break;
                }
            case CellStatus.SELECTED:
                {
                    SelectIndicator.gameObject.SetActive(true);
                    break;
                }
            default: break;
        }
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

            case CellStatus.FIRED:
                {
                    if (HasShip)
                    {
                        ColorUtility.TryParseHtmlString(Constants.CELL_COLOR_FIRING, out color);
                        Boom.GetComponent<ParticleSystem>().Play();
                        Flame.GetComponent<ParticleSystem>().Play();
                    }
                    else
                    {
                        ColorUtility.TryParseHtmlString(Constants.CELL_COLOR_GREYED, out color);
                        Smoke.GetComponent<ParticleSystem>().Play();
                    }
                    break;
                }

            case CellStatus.SELECTED:
                {
                    ColorUtility.TryParseHtmlString(Constants.CELL_COLOR_SELECTED, out color);
                    break;
                }

            default:
                {
                    color = new Color(0, 0, 0);
                    break;
                }
        }
        CellPlane.GetComponent<Renderer>().material.color = color;
        CellPlane.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
    }

    private void OnMouseUpAsButton()
    {
        Logger.Log("Hit Cell: - (" + row + "," + col + ")");
        if (
            !Fired
            && fieldMap.GetComponent<FieldMapController>().Selectable
        )
        {
            fieldMap.GetComponent<FieldMapController>().SelectCell(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == Tags.FIRE_BALL)
        {
            Logger.Log("Cell: OnCollisionEnter - FIRE_BALL");
            Destroy(collision.collider.gameObject);
            fieldMap.GetComponent<FieldMapController>().CheckDestroyCell(gameObject);
        }
    }
}
