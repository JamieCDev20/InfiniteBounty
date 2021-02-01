using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentFuser : MonoBehaviour
{
    public Augment FuseAugments(Augment a, Augment b)
    {
        // Create new Augment
        Augment newAug = new Augment();
        if(a == b)
        {
            // Increase level of fused augment
            newAug = a;
            newAug.Level++;
            return newAug;
        }
        if(a != b && !a.Fused && !b.Fused)
        {
            newAug = a + b;
            newAug.Fused = true;
            return newAug;
        }
        return null;
    }

    public static Augment VerbCombine(Augment a, Augment b)
    {
        return Augment.Combine(a, b);
    }

    public static ProjectileAugment VerbCombine(ProjectileAugment a, ProjectileAugment b)
    {
        return ProjectileAugment.Combine(a, b);
    }

    public static ConeAugment VerbCombine(ConeAugment a, ConeAugment b)
    {
        return ConeAugment.Combine(a, b);
    }
}
