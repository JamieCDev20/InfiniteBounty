using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiversifierManager : MonoBehaviour
{
    public static DiversifierManager x;
    private Diversifier[] dA_activeDivers = new Diversifier[3];

    private void Awake()
    {
        if (x) Destroy(gameObject);
        else
        {
            x = this;
            DontDestroyOnLoad(gameObject);
            transform.parent = null;
        }
    }

    public void ReceiveDiversifiers(Diversifier[] _dA_diversGotten)
    {
        dA_activeDivers[0] = _dA_diversGotten[0];
        dA_activeDivers[1] = _dA_diversGotten[1];
        dA_activeDivers[2] = _dA_diversGotten[2];
    }

    public Diversifier[] ReturnActiveDivers()
    {
        return dA_activeDivers;
    }


    public void ApplyDiversifiers()
    {
        for (int i = 0; i < dA_activeDivers.Length; i++)
        {
            switch (dA_activeDivers[i])
            {
                case Diversifier.None: break;

                case Diversifier.JackedRabbits:
                    break;
                case Diversifier.GigaGeysers:
                    break;
                case Diversifier.SolarStorm:
                    break;

                default: break;
            }
        }
    }


}
