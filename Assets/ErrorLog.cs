using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public delegate void ErrorLogEventHandler(object sender, string error);

public class ErrorUIController : MonoBehaviour
{
    public Text errorText;
    public Button OKButton;
    public Image backdrop;

    void Start()
    {
        errorText = GetComponentInChildren<Text>();
        OKButton = GetComponentInChildren<Button>();
        backdrop = GetComponentInChildren<Image>();

        IsVisible(false);
        ClearErrorText();
    }

    public void OnErrorLogged(object sender, string e)
    {
       IsVisible(true);
       errorText.text = errorText.text.Insert(0, "\n" + e);
    }

    private void ClearErrorText()
    {
        errorText.text = "";
    }

    public void ErrorAcknowledged()
    {
        ClearErrorText();
       IsVisible(false);
    }

    private void IsVisible(bool state)
    {
        errorText.gameObject.SetActive(state);
        OKButton.gameObject.SetActive(state);
        backdrop.gameObject.SetActive(state);
    }
}

/// <summary>
/// Handles debugging output to the in-app debugging console
/// </summary>
static class App
{
    //Event handler and delegate
    public static event ErrorLogEventHandler ErrorThrown;
    /// <summary>
    /// logs errors and displays them in the apps debug console
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="error"></param>
    public static void Log (object sender, string error)
    {
        if (ErrorThrown != null)
        {
            ErrorThrown(sender, error);
        }
    }
}

public class ErrorLog : MonoBehaviour
{
    public ErrorUIController UI;

    private void Update()
    {
        
    }
    public void OKButtonPress()
    {
        UI.ErrorAcknowledged();
    }

    void Start()
    {
        this.gameObject.AddComponent<ErrorUIController>();
        UI = GetComponent<ErrorUIController>();
        App.ErrorThrown += UI.OnErrorLogged;
    }
}
