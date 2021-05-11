using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaManager : MonoBehaviour
{
    public static BossArenaManager x;

    [SerializeField] private LilyPad[] lpA_lilypads = new LilyPad[0];
    [Space, SerializeField] private KillBox kb_lava;
    [SerializeField] private GameObject go_tractorBeam;

    void Awake()
    {
        x = this;
        LilyPad[] _lpA = FindObjectsOfType<LilyPad>();

        for (int i = 0; i < _lpA.Length; i++)
        {
            _lpA[i].Setup(i);
        }
    }


    public void BossDied()
    {
        StartCoroutine(BossDiedEvents());
    }

    private IEnumerator BossDiedEvents()
    {
        kb_lava.Neutralize();
        yield return new WaitForSeconds(1);
        FindObjectOfType<BossAudioManager>().StopPlaying();
        for (int i = 0; i < lpA_lilypads.Length; i++)
        {
            yield return new WaitForSeconds(0.01f);
            lpA_lilypads[i].TakeDamage(100, false);
        }

        go_tractorBeam.SetActive(true);
        DifficultyManager.x.IncreaseDifficultiesUnlocked();
    }
}
