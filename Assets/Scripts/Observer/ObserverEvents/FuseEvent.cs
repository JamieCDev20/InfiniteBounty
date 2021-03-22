using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseEvent : ObserverEvent
{
    private Augment augA;
    private Augment augB;
    private Augment fusedAug;
    public Augment[] Augs { get { return new Augment[3] { augA, augB, fusedAug }; } }
    public FuseEvent(Augment _a, Augment _b, Augment _fused)
    {
        augA     = _a;
        augB     = _b;
        fusedAug = _fused;
    }
    public FuseEvent(Augment[] augs)
    {
        if (augs.Length == 3)
        {
            augA = augs[0];
            augB = augs[1];
            fusedAug = augs[2];
        }
    }
}
