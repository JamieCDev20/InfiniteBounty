using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{

    [SerializeField] private ScoreObjects[] so_playerScoreObjects;

    public void SetValues(int[][] values, int nugCount)
    {
        for (int i = 0; i < values.Length; i++)
        {
            so_playerScoreObjects[i].boomText.text = values[i][0].ToString();
            //so_playerScoreObjects[i].tastyText.text = values[i, 1].ToString();
            so_playerScoreObjects[i].thunderText.text = values[i][2].ToString();
            so_playerScoreObjects[i].magmaText.text = values[i][3].ToString();
            so_playerScoreObjects[i].hydroText.text = values[i][4].ToString();
            so_playerScoreObjects[i].gooText.text = values[i][5].ToString();
        }
    }

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
