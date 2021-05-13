using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultySelector : MonoBehaviourPun, IInteractible
{

    [SerializeField] private TextMeshPro tmp_difficultyAbove;
    [SerializeField] private TextMeshPro tmp_difficultyCurrent;
    [SerializeField] private TextMeshPro tmp_difficultyBelow;
    [Space, SerializeField] private Transform t_textParent;
    [SerializeField] private int i_difficultyChangeOnClick;
    private AudioSource as_source;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();

        ChangeDifficulty(-1);

        for (int i = 0; i < DifficultyManager.x.MaximumDifficulty; i++)
            ChangeDifficulty(1);

    }

    public void ChangeDifficulty(int _i_difficultyChange)
    {
        StartCoroutine(ScrollDifficulty(_i_difficultyChange));
    }

    private IEnumerator ScrollDifficulty(int _i_change)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForEndOfFrame();
            t_textParent.position += Vector3.down * _i_change * 0.05f;
        }

        DifficultyManager.x.ChangeCurrentDifficulty(_i_change);

        if (DifficultyManager.x.ReturnCurrentDifficultyInt() <= DifficultyManager.x.MaximumDifficulty)
            tmp_difficultyAbove.text = DifficultyManager.x.ReturnDifficultyByIndex(DifficultyManager.x.ReturnCurrentDifficultyInt() + 1).s_name;
        else tmp_difficultyAbove.text = "???";

        tmp_difficultyCurrent.text = DifficultyManager.x.ReturnCurrentDifficulty().s_name;

        if (DifficultyManager.x.ReturnCurrentDifficultyInt() > 0)
            tmp_difficultyBelow.text = DifficultyManager.x.ReturnDifficultyByIndex(DifficultyManager.x.ReturnCurrentDifficultyInt() - 1).s_name;
        else tmp_difficultyBelow.text = "";

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForEndOfFrame();
            t_textParent.position += Vector3.up * _i_change * 0.05f;
        }
        NetworkManager.x.SetDiffDisplay(DifficultyManager.x.ReturnCurrentDifficultyInt());
        InfoText.x?.OnNotify(new InfoTextEvent("Difficulty changed to " + DifficultyManager.x.ReturnCurrentDifficulty().s_name));
    }

    internal void SetScreenView(int _i)
    {
        DifficultyManager.x.SetCurrentDifficulty(_i);

        if (DifficultyManager.x.ReturnCurrentDifficultyInt() <= DifficultyManager.x.MaximumDifficulty)
            tmp_difficultyAbove.text = DifficultyManager.x.ReturnDifficultyByIndex(DifficultyManager.x.ReturnCurrentDifficultyInt() + 1).s_name;
        else tmp_difficultyAbove.text = "???";

        tmp_difficultyCurrent.text = DifficultyManager.x.ReturnCurrentDifficulty().s_name;

        if (DifficultyManager.x.ReturnCurrentDifficultyInt() > 0)
            tmp_difficultyBelow.text = DifficultyManager.x.ReturnDifficultyByIndex(DifficultyManager.x.ReturnCurrentDifficultyInt() - 1).s_name;
        else tmp_difficultyBelow.text = "";
        if(i_difficultyChangeOnClick > 0)
            InfoText.x?.OnNotify(new InfoTextEvent("Difficulty changed to " + DifficultyManager.x.ReturnCurrentDifficulty().s_name));


    }

    public void Interacted()
    {
        TutorialManager.x.UsedRiskSelector();
        ChangeDifficulty(i_difficultyChangeOnClick);
        as_source.Play();
    }

    public void Interacted(Transform interactor) { }
}
