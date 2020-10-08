using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType
{
    LocationGas, LocationVolcano, ReturnToShip
}

public class WorldSpaceButton : MonoBehaviour, IUseable
{
    [SerializeField] private ButtonType bt_thisButtonType;


    public void OnUse()
    {
        switch (bt_thisButtonType)
        {
            case ButtonType.LocationGas:
                LocationController.x.SetLocation(Location.GasGiant, gameObject);
                break;

            case ButtonType.LocationVolcano:
                LocationController.x.SetLocation(Location.Volcano, gameObject);
                break;

            case ButtonType.ReturnToShip:
                for (int i = 0; i < LocationController.x.goA_playerObjects.Length; i++)                
                    LocationController.x.goA_playerObjects[i].transform.position = new Vector3(0, 0, i * 1.5f);
                LocationController.x.UnloadArea();
                break;
        }
    }
}
