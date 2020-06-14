using UnityEngine;
using UnityEngine.UI;

public class ButtonColtroller : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.Sound.BUTTON_CLICK);
        });
    }
}
