using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectBase : MonoBehaviourPunCallbacks
{
    private List<ObserverBase> L_observers = new List<ObserverBase>();
    
    protected void Notify(ObserverEvent oe_event)
    {
        foreach (ObserverBase obs in L_observers)
            if(obs != null)
                obs.OnNotify(oe_event);
    }
    public void AddObserver(ObserverBase _ob_obs)
    {
        L_observers.Add(_ob_obs);
    }
    public void RemoveObserver(ObserverBase _ob_obs)
    {
        L_observers.Remove(_ob_obs);
    }
}
