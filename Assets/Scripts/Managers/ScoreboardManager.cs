using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{

    [SerializeField] private ScoreObjects[] so_playerScoreObjects;
    [SerializeField] private Text t_totalEarned;

    public void Start()
    {
        if (PhotonNetwork.InRoom)
            UniversalNugManager.x?.DoScoring();
    }

    public void SetValues(int[][] values, string[] _names)
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
            playerTotal = UniversalNugManager.x.CalculateValues(values[i]);
            so_playerScoreObjects[i].bucksText.text = playerTotal.ToString();
            totalEarned += playerTotal;
        }
        t_totalEarned.text = totalEarned.ToString();
        NetworkedPlayer.x.CollectEndLevelNugs(totalEarned);
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
