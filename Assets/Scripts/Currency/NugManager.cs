using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NugManager : SubjectBase, ObserverBase
{
    int i_currentNugs = 0;
    /// <summary>
    /// The amount of nugs collected since last network update
    /// </summary>
    int i_nugsCollected = 0;
    public int i_playerID;
    [SerializeField] Text t_nugText;
    // Start is called before the first frame update
    void Start()
    {
        foreach (NugPlayer np in FindObjectsOfType<NugPlayer>())
            if (i_playerID == np.i_playerID)
            {
                np.AddObserver(this);
                Debug.Log("Got the player!" + np.name);
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnNotify(ObserverEvent oe_event)
    {
        switch (oe_event)
        {
            case CurrencyEvent ce:
                if (ce.AddOrSubtract)
                    CollectNugs(ce.AmountToChange);
                else
                    CollectNugs(-ce.AmountToChange);
                break;
        }
    }

    public void CollectNugs(int _i_value)
    {
        i_currentNugs += _i_value;
        i_nugsCollected += _i_value;
        t_nugText.text = i_currentNugs.ToString();
    }
    public void SendNugs()
    {
        // Send Total nugs and nugs collected
        
        // Send nugs
        i_nugsCollected = 0;
    }
    public void ReceiveNugs(int _i_value)
    {
        i_currentNugs += _i_value;
    }
    
}
