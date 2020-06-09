using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OverlayManager : MonoBehaviour
{
    public Animator AnimatorController;
    public GameObject TitleLabel;

    const string OPEN_NAME = "Open";
    const string CLOSED_NAME = "Closed";
    private int OPEN_PARAM_ID;

    private void Awake()
    {
        OPEN_PARAM_ID = Animator.StringToHash(OPEN_NAME);
    }

    public void Open(string text)
    {
        if (AnimatorController.gameObject.activeSelf)
        {
            Close();
        }
        AnimatorController.gameObject.SetActive(true);
        AnimatorController.SetBool(OPEN_PARAM_ID, true);
        TitleLabel.GetComponent<Text>().text = text;
    }

    public void Open(string text, float wait)
    {
        Open(text);
        StartCoroutine(DelayCloseForSeconds(wait));
    }

    public void Close()
    {
        Debug.Log("Close");
        AnimatorController.SetBool(OPEN_PARAM_ID, false);
        StartCoroutine(DelayCloseForAnimation());
    }

    IEnumerator DelayCloseForAnimation()
    {
        bool closedStateReached = false;
        bool wantToClose = true;
        while (!closedStateReached && wantToClose)
        {
            if (!AnimatorController.IsInTransition(0))
                closedStateReached = AnimatorController.GetCurrentAnimatorStateInfo(0).IsName(CLOSED_NAME);

            wantToClose = !AnimatorController.GetBool(OPEN_PARAM_ID);

            yield return new WaitForEndOfFrame();
        }

        if (wantToClose)
        {
            AnimatorController.gameObject.SetActive(false);
        }
    }

    IEnumerator DelayCloseForSeconds(float wait)
    {
        yield return new WaitForSeconds(wait);
        Close();
    }
}
