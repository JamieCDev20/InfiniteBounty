using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private string s_toolTip;
    public string Tip { get { return s_toolTip; } }
}
