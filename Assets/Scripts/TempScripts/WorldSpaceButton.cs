using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType
{
    LocationNuggetRun, LocationMotherlode, ReturnToShip, LocationStandoff
}

public class WorldSpaceButton : MonoBehaviour, IUseable
{
    [SerializeField] private ButtonType bt_thisButtonType;


    public virtual void OnUse()
    {
        switch (bt_thisButtonType)
        {
            case ButtonType.LocationNuggetRun:
                LocationController.x.SetLocation(Location.NuggetRun, gameObject);
                break;

            case ButtonType.LocationMotherlode:
                LocationController.x.SetLocation(Location.MotherLode, gameObject);
                break;

            case ButtonType.ReturnToShip:
                for (int i = 0; i < LocationController.x.goA_playerObjects.Length; i++)                
                    LocationController.x.goA_playerObjects[i].transform.position = new Vector3(0, 0, i * 1.5f);
                LocationController.x.UnloadArea();
                break;

            case ButtonType.LocationStandoff:
                LocationController.x.SetLocation(Location.Standoff, gameObject);
                break;
        }
    }
}