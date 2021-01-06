using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{

    [SerializeField] private ScoreObjects[] so_playerScoreObjects;



}

[System.Serializable]
public class ScoreObjects
{
    public Text nameText;
    public Text boomText;
    public Text tastyText;
    public Text thunderText;
    public Text magmaText;
    public Text hydroText;
    public Text gooText;
    public Text bucksText;
}
