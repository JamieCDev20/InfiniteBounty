using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimController : MonoBehaviour
{

    private BossAI boss;

    private void Start()
    {
        boss = GetComponentInParent<BossAI>();
    }


    public void FireMissile()
    {
        boss.DoHomingAttack();
    }

    public void SummonMeteor()
    {
        boss.DoMortarAttack();
    }

    public void Move()
    {
        boss.MoveAttack();
    }

    public void DoTreeIteration()
    {
        if (PhotonNetwork.IsMasterClient)
            boss.ChooseAction();
    }
}
