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
    private Text t_nugText;
    // Start is called before the first frame update
    void Start()
    {
        foreach (NugGO np in Resources.FindObjectsOfTypeAll<NugGO>())
        {
            np.AddObserver(this);
        }

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
            case PoolLoadEvent ple:
                foreach (NugGO np in FindObjectsOfType<NugGO>())
                {
                    np.AddObserver(this);
                    Debug.Log("Nugget Added");
                }
                break;
        }
    }
    public void Init()
    {

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
    
    public void SetNugTextRef(Text _txt_)
    {
        t_nugText = _txt_;
    }

}
