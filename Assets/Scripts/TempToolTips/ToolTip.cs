using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private string s_toolTip;
    [SerializeField] internal int i_buttonSpriteIndex;
    [SerializeField] internal bool b_hostOnly;
    public string Tip { get { return s_toolTip; } }


    [Header("H for Help")]
    [SerializeField] internal bool b_hasHelpData;
    [SerializeField, TextArea] private string s_helpText;
    public string HelpText { get { return s_helpText; } }



}