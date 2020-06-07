using System;
using UnityEngine;
using UnityEngine.UI;

public class Logger
{
    public Logger()
    {
    }

    public static void Log(String text)
    {
        GameObject[] LoggerTexts = GameObject.FindGameObjectsWithTag("LOGGER");
        foreach (GameObject logger in LoggerTexts)
        {
            logger.GetComponent<Text>().text = logger.GetComponent<Text>().text + " - " + text;

            Debug.Log("ARGAME " + text);
        }
    }
}
