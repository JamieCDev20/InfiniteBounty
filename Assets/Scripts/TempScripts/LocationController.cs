using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum Location
{
    GasGiant, Volcano
}

public class LocationController : MonoBehaviour
{
    public static LocationController x;
    internal Location l_currentTarget = Location.GasGiant;
    private GameObject[] goA_loadedAreaObjects = new GameObject[0];

    [SerializeField] private CannonController cc_cannon;
    [Space, SerializeField] private GameObject[] goA_planetButtons = new GameObject[0];
    [SerializeField] internal GameObject[] goA_playerObjects = new GameObject[0];

    [Header("UI Elements?")]
    [SerializeField] private TextMeshPro tmp_loadingText;
    [SerializeField] private Color c_readyColour;
    [SerializeField] private Color c_travellingColour;

    [Header("Gas Giant References")]
    [SerializeField] private GameObject[] goA_gasAreas = new GameObject[0];

    [Header("Gas Giant References")]
    [SerializeField] private GameObject[] goA_volcanoAreas = new GameObject[0];


    private void Start()
    {
        x = this;

        for (int i = 0; i < goA_gasAreas.Length; i++)
            goA_gasAreas[i].SetActive(false);

        for (int i = 0; i < goA_volcanoAreas.Length; i++)
            goA_volcanoAreas[i].SetActive(false);

        cc_cannon.b_isReady = false;

        StartCoroutine(LoadArea());
    }

    public void SetLocation(Location _l_target, GameObject _go_buttonPressed)
    {
        l_currentTarget = _l_target;
        for (int i = 0; i < goA_planetButtons.Length; i++)
            goA_planetButtons[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        _go_buttonPressed.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        StartCoroutine(LoadArea());
    }

    internal IEnumerator LoadArea()
    {
        cc_cannon.b_isReady = false;
        tmp_loadingText.text = "Travelling";
        tmp_loadingText.color = c_travellingColour;

        switch (l_currentTarget)
        {
            case Location.GasGiant:
                for (int i = 0; i < goA_gasAreas.Length; i++)
                {
                    goA_gasAreas[i].SetActive(true);
                    yield return new WaitForEndOfFrame();
                }
                goA_loadedAreaObjects = goA_gasAreas;
                break;

            case Location.Volcano:
                for (int i = 0; i < goA_volcanoAreas.Length; i++)
                {
                    goA_volcanoAreas[i].SetActive(true);
                    yield return new WaitForEndOfFrame();
                }
                goA_loadedAreaObjects = goA_volcanoAreas;
                break;
        }

        tmp_loadingText.text = "Ready";
        tmp_loadingText.color = c_readyColour;
        cc_cannon.b_isReady = true;
    }

    internal void UnloadArea()
    {
        for (int i = 0; i < goA_loadedAreaObjects.Length; i++)
            goA_loadedAreaObjects[i].SetActive(false);

    }
}

public interface IUseable
{
    void OnUse();
}