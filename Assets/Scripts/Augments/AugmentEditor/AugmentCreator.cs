using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AugmentCreator
{
    public void CreateAugment(string[] _s_parsedData)
    {
        Augment aug;
        switch (_s_parsedData[1])
        {
            case "Projectile Augment":
                aug = CreateProjectile(_s_parsedData[6]);
                break;
            case "Cone Augment":
                aug = CreateCone(_s_parsedData[7]);
                break;
        }
    }

    private ProjectileAugment CreateProjectile(string _s_projectileData)
    {
        ProjectileAugment projAug = new ProjectileAugment();
        int shots = int.Parse(_s_projectileData.Split(':', ',')[1]);
        float grav = float.Parse(_s_projectileData.Split(':', ',')[3]);
        PhysicMaterial pm = (PhysicMaterial)Resources.Load(_s_projectileData.Split(':', ',')[5]);
        Vector3 scale = new Vector3(float.Parse(_s_projectileData.Split(':', ',')[6]), float.Parse(_s_projectileData.Split(':', ',')[8]), float.Parse(_s_projectileData.Split(':', '}')[6]));
        projAug.InitProjectile(shots, grav, pm, scale);
        return projAug;
    }

    private ConeAugment CreateCone(string _s_coneData)
    {
        ConeAugment coneAug = new ConeAugment();
        float ang = float.Parse(_s_coneData.Split(':', ',')[1]);
        float len = float.Parse(_s_coneData.Split(':', '}')[2]);
        coneAug.InitCone(ang, len);
        return coneAug;
    }
}
