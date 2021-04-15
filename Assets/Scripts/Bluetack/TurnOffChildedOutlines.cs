using Knife.HDRPOutline.Core;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffChildedOutlines : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        foreach (OutlineObject o in GetComponentsInChildren<OutlineObject>())
        {
            Destroy(o);
            //o.enabled = false;
        }
        foreach (FixOutlines f in GetComponentsInChildren<FixOutlines>())
        {
            f.enabled = false;
        }
    }

}
