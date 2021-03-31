using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseEvent : ObserverEvent
{
    private Augment augA;
    private Augment augB;
    private Augment fusedAug;
    private ProjectileAugment projA;
    private ProjectileAugment projB;
    private ProjectileAugment fusedProj;
    private ConeAugment coneA;
    private ConeAugment coneB;
    private ConeAugment fusedCone;
    private int i_augAIndex;
    private int i_augBIndex;
    private int i_fuseAugIndex;
    public Augment[] Augs { get { return new Augment[3] { augA, augB, fusedAug }; } }
    public ProjectileAugment[] Proj { get { return new ProjectileAugment[3] { projA, projB, fusedProj }; } }
    public ConeAugment[] Cone { get { return new ConeAugment[3] { coneA, coneB, fusedCone }; }}
    public FuseEvent(Augment _a, Augment _b, Augment _fused)
    {
        augA     = _a;
        augB     = _b;
        fusedAug = _fused;
        Debug.Log("Standard Event");
    }
    public FuseEvent(ProjectileAugment _a, ProjectileAugment _b, ProjectileAugment _fused)
    {
        projA = _a;
        projB = _b;
        fusedProj = _fused;
        Debug.Log("Proj Event");
    }
    public FuseEvent(ConeAugment _a, ConeAugment _b, ConeAugment _fused)
    {
        coneA = _a;
        coneB = _b;
        fusedCone = _fused;
        Debug.Log("Cone Event");
    }
    public FuseEvent(Augment _a, Augment _b, Augment _fused, int _aInd, int _bInd, int _fusedInd)
    {
        augA            = _a;
        augB            = _b;
        fusedAug        = _fused;
        i_augAIndex     = _aInd;
        i_augBIndex     = _bInd;
        i_fuseAugIndex  = _fusedInd;
    }

    public FuseEvent(int _aInd, int _bInd, int _fusedInd)
    {
        i_augAIndex     = _aInd;
        i_augBIndex     = _bInd;
        i_fuseAugIndex  = _fusedInd;
    }

    public FuseEvent(Augment[] augs)
    {
        if (augs.Length == 3)
        {
            augA     = augs[0];
            augB     = augs[1];
            fusedAug = augs[2];
        }
    }
}
