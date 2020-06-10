using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour
{
    public GameObject[] MyHp;
    public GameObject[] OpHp;

    public GameObject HUDTitle;
    public GameObject HUDTimer;
    public GameObject HUDDescription;

    public GameObject HUDAttackButton;

    public void ShowHUD(bool active)
    {
        gameObject.SetActive(active);
    }

    public void ChangeTitle(bool isMyTurn)
    {
        string text = isMyTurn ? "YOUR TURN" : "OP'S TURN";
        HUDTitle.GetComponent<Text>().text = text;
    }

    public void ChangeTimer(int seconds)
    {
        HUDTimer.GetComponent<Text>().text = seconds.ToString();
    }

    public void ChangeDescription()
    {
        // TODO
    }

    public void UpdateHp(int hp, bool isMyHp)
    {
        GameObject[] HPs = isMyHp ? MyHp : OpHp;
        for (int i = hp; i < HPs.Length; i++)
        {
            // TODO: HP lose animation
            HPs[i].SetActive(false);
        }
    }
}
