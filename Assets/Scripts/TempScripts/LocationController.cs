using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum Location
{
    SmashGrab, BossLevel
}

public class LocationController : MonoBehaviour
{
    public static LocationController x;
    internal Location l_currentTarget = Location.SmashGrab;
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

    [Header("Smash Grab References")]
    [SerializeField] private GameObject go_smashGrabArea;
    [SerializeField] private GameObject[] goA_pathBlockers = new GameObject[0];

    [Header("Boss References")]
    [SerializeField] private GameObject go_bossArea;

    private void Start()
    {
        x = this;

        go_smashGrabArea.SetActive(false);
        go_bossArea.SetActive(false);

        cc_cannon.b_isReady = false;

        SetLocation(Location.SmashGrab, goA_planetButtons[0]);
        StartCoroutine(LoadArea());
    }

    internal void PickedUpNugget(int _i_nuggetWorth)
    {
        i_nuggetTotal += _i_nuggetWorth;
        tmp_nuggetsCollectedtext.text = "Nuggets Collected: " + i_nuggetTotal;
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
            case Location.SmashGrab:
                go_smashGrabArea.SetActive(true);
                yield return new WaitForEndOfFrame();
                go_loadedAreaObject = go_smashGrabArea;
                goA_pathBlockers[UnityEngine.Random.Range(0, goA_pathBlockers.Length)].SetActive(false);
                break;

            case Location.BossLevel:
                go_bossArea.SetActive(true);
                yield return new WaitForEndOfFrame();
                go_bossArea.GetComponentInChildren<Enemy>().Begin();
                go_loadedAreaObject = go_bossArea;
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