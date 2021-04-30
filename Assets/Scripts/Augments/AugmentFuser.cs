using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentFuser : MonoBehaviour
{
    public Augment FuseAugments(Augment a, Augment b)
    {
        // Create new Augment
        Augment newAug = new Augment();
        if(a.Name == b.Name)
        {
            // Increase level of fused augment
            newAug = a;
            newAug.Level = a.Level + b.Level;
            Debug.Log("MY NEW LEVEL IS: " + newAug.Level);
            return newAug;
        }
        else if(a != b && a.Stage != AugmentStage.fused && b.Stage != AugmentStage.fused)
        {
            Debug.Log("Combine is also running, for some reason");
            newAug = Augment.Combine(a, b);
            newAug.Stage = AugmentStage.fused;
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
