using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    [SerializeField]
    private InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TryLoadScene);
    }

    public void TryLoadScene()
    {
        string sceneName = inputField.text;

        if (sceneName != null)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
