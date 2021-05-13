using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTextEvent : ObserverEvent
{
    string textToDisplay;
    public string TextToDisplay { get { return textToDisplay; } }
    public InfoTextEvent(string _newText)
    {
        textToDisplay = _newText;
    }
}
