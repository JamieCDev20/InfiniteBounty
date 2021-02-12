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

    void Start()
    {
        x = this;
    }


    public void BossDied()
    {
        StartCoroutine(BossDiedEvents());
    }

    private IEnumerator BossDiedEvents()
    {
        kb_lava.Neutralize();
        yield return new WaitForSeconds(1);
        for (int i = 0; i < lpA_lilypads.Length; i++)
        {
            yield return new WaitForSeconds(0.01f);
            lpA_lilypads[i].TakeDamage(100, false);
        }

        go_tractorBeam.SetActive(true);
    }
}
