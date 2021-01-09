using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{

    [SerializeField] private ScoreObjects[] so_playerScoreObjects;
    [SerializeField] private Text t_totalEarned;
    private int[] nugValues = new int[6] { 1, 1, 1, 2, 2, 3 };

    public void SetValues(int[][] values, int[] nugCount, string[] _names)
    {
        int totalEarned = 0;
        int playerTotal;
        for (int i = 0; i < values.Length; i++)
        {
            so_playerScoreObjects[i].nameText.text = _names[i];
            so_playerScoreObjects[i].gooText.text = values[i][0].ToString();
            so_playerScoreObjects[i].hydroText.text = values[i][1].ToString();
            so_playerScoreObjects[i].tastyText.text = values[i][2].ToString();
            so_playerScoreObjects[i].thunderText.text = values[i][3].ToString();
            so_playerScoreObjects[i].boomText.text = values[i][4].ToString();
            so_playerScoreObjects[i].magmaText.text = values[i][5].ToString();
            playerTotal = CalculateValues(values[i]);
            so_playerScoreObjects[i].bucksText.text = playerTotal.ToString();
            totalEarned += playerTotal;
        }
        t_totalEarned.text = totalEarned.ToString();

    }

    private int CalculateValues(int[] _Vals)
    {
        int total = 0;
        for (int i = 0; i < _Vals.Length; i++)
        {
            total += (_Vals[i] * nugValues[i]);
        }
        return total;
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
