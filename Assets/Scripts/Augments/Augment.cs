using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Augment
{
    #region Audio
    
    AudioClip ac_useSound;
    AudioClip ac_travelSound;
    AudioClip ac_hitSound;

    #endregion

    #region Tool Information Properties

    float f_weight;
    float f_recoil;
    float f_speed;
    float f_heatsink;
    float f_knockback;
    float f_energyGauge;
    int i_damage;
    int i_lodeDamage;

    #endregion

    #region Tool Physical Properties

    float f_trWidth;
    float f_trLifetime;
    Color[] A_trKeys;
    GameObject go_weaponProjectile;

    #endregion

    #region EXPLOSION

    float f_explockBack;
    float f_detonationTime;
    GameObject go_explosion;
    GameObject go_explarticles;

    #endregion

    #region Elemental 

    // Coming soon to a tool near you!

    #endregion

    

}
