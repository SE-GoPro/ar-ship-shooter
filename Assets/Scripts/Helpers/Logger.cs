using System;
using UnityEngine;
using UnityEngine.UI;

public class Logger
{
    public Logger()
    {
    }

    private static void LogToScreen(object message)
    {
        GameObject[] LoggerTexts = GameObject.FindGameObjectsWithTag("LOGGER");
        foreach (GameObject logger in LoggerTexts)
        {
            logger.GetComponent<Text>().text = logger.GetComponent<Text>().text + " - " + message.ToString();
        }
    }

    public static void Log(String text)
    {
        Debug.Log(text);
        LogToScreen(text);
    }

    public static void LogError(object error)
    {
        Debug.LogError(error);
        LogToScreen(error);
    }
}
