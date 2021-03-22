using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FuseSaver : MonoBehaviour
{
    public static FuseSaver x;

    // Start is called before the first frame update
    public void Init()
    {
        if (x != null)
        {
            if (x != this)
                Destroy(x);
        }
        else
            x = this;

        DontDestroyOnLoad(this);
        LoadFusedAugments();
    }

    private void LoadFusedAugments()
    {
        if(Resources.Load("FusedAugmentData") != null)
        {
            string fusedstring = AugmentLoader.LoadFusedAugmentJson();
            if(fusedstring != null)
            {

            }

        }
        else
        {
            File.Create(Application.dataPath + "/Resources/FusedAugmentData.JSON");
        }
    }
}
