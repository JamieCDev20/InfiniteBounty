using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum Location
{
    NuggetRun, MotherLode, Standoff
}

public class LocationController : MonoBehaviour
{
    public static LocationController x;
    internal Location l_currentTarget = Location.NuggetRun;
    private GameObject go_loadedAreaObject;

    [SerializeField] private CannonController cc_cannon;
    [Space, SerializeField] private GameObject[] goA_planetButtons = new GameObject[0];
    [SerializeField] internal GameObject[] goA_playerObjects = new GameObject[0];

    [Header("UI Elements?")]
    [SerializeField] private TextMeshPro tmp_loadingText;
    [SerializeField] private Color c_readyColour;
    [SerializeField] private Color c_travellingColour;
    [SerializeField] private TextMeshPro tmp_nuggetsCollectedtext;
    private int i_nuggetTotal;

    [Header("Nugget Run References")]
    [SerializeField] private GameObject go_nuggetRunArea;
    [SerializeField] private GameObject[] goA_pathBlockers = new GameObject[0];

    [Header("Motherlode References")]
    [SerializeField] private GameObject go_motherLodeArea;

    [Header("Standoff References")]
    [SerializeField] private GameObject go_standOffArea;


    [Header("Universal Prefabs")]
    [SerializeField] internal GameObject go_gooPatchPrefab;
    [SerializeField] internal GameObject go_waterPuddlePrefab;
    [SerializeField] internal GameObject go_explosionPrefab;


    private void Start()
    {
        x = this;

        go_nuggetRunArea.SetActive(false);
        go_motherLodeArea.SetActive(false);

        cc_cannon.b_isReady = false;

        SetLocation(Location.NuggetRun, goA_planetButtons[0]);
        StartCoroutine(LoadArea());
    }

    internal void CompletedNuggetRun()
    {
        goA_planetButtons[0].SetActive(false);
    }

    internal void CompletedMotherlode()
    {
        PickedUpNugget(100);
        goA_planetButtons[1].SetActive(false);
    }

    internal void CompletedStandoff(bool _b_victory)
    {
        if (_b_victory)
            PickedUpNugget(100);
        goA_planetButtons[2].SetActive(false);
    }


    internal void PickedUpNugget(int _i_nuggetWorth)
    {
        i_nuggetTotal += _i_nuggetWorth;
        tmp_nuggetsCollectedtext.text = "Nuggets Collected: " + i_nuggetTotal;
    }

    public void SetLocation(Location _l_target, GameObject _go_buttonPressed)
    {
        if (go_loadedAreaObject != null)
            UnloadArea();

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
            case Location.NuggetRun:
                go_nuggetRunArea.SetActive(true);
                yield return new WaitForEndOfFrame();
                go_loadedAreaObject = go_nuggetRunArea;
                for (int i = 0; i < goA_pathBlockers.Length; i++)
                    goA_pathBlockers[i].SetActive(true);

                goA_pathBlockers[UnityEngine.Random.Range(0, goA_pathBlockers.Length)].SetActive(false);
                break;

            case Location.MotherLode:
                go_motherLodeArea.SetActive(true);
                yield return new WaitForEndOfFrame();
                go_motherLodeArea.GetComponentInChildren<Enemy>().Begin();
                go_loadedAreaObject = go_motherLodeArea;
                break;

            case Location.Standoff:
                go_standOffArea.SetActive(true);
                yield return new WaitForEndOfFrame();
                go_standOffArea.GetComponentInChildren<StockPile>().Begin();
                go_loadedAreaObject = go_standOffArea;
                break;
        }

        tmp_loadingText.text = "Ready";
        tmp_loadingText.color = c_readyColour;
        cc_cannon.b_isReady = true;
    }

    internal void UnloadArea()
    {

        go_loadedAreaObject.SetActive(false);

        for (int i = 0; i < goA_planetButtons.Length; i++)
            goA_planetButtons[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        cc_cannon.b_isReady = false;
    }
}

public interface IUseable
{
    void OnUse();
}