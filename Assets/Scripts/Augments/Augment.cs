using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Augment
{
    [SerializeField] protected string s_name;
    [SerializeField] protected int i_level;
    [SerializeField] protected AugmentStage as_stage;
    [SerializeField] protected bool b_fused;
    [SerializeField] protected Material mat_augColor;
    public string Name { get { return s_name; } }
    public int Level { get { return i_level; } set { i_level = value; } }
    public bool Fused { get { return b_fused; } set{ b_fused = value; } }
    public AugmentStage Stage { get { return as_stage; } set { as_stage = value; } }
    public Material AugmentMaterial { get { return mat_augColor; } set { mat_augColor = value; } }
    #region Audio

    [SerializeField] protected AudioClip[] ac_useSound;
    [SerializeField] protected AudioClip[] ac_travelSound;
    [SerializeField] protected AudioClip[] ac_hitSound;

    #endregion

    #region Tool Information Properties

    [SerializeField] protected float f_weight;
    [SerializeField] protected float f_recoil;
    [SerializeField] protected float f_speed;
    [SerializeField] protected float f_heatsink;
    [SerializeField] protected float f_knockback;
    [SerializeField] protected float f_energyGauge;
    [SerializeField] protected int i_damage;
    [SerializeField] protected int i_lodeDamage;

    #endregion

    #region Tool Physical Properties

    [SerializeField] protected float f_trWidth;
    [SerializeField] protected float f_trLifetime;
    [SerializeField] protected Color[] A_trKeys;
    [SerializeField] protected GameObject go_weaponProjectile;

    #endregion

    #region EXPLOSION

    [SerializeField] protected int i_explosionDamage;
    [SerializeField] protected int i_expLodeDamage;
    [SerializeField] protected bool b_impact;
    [SerializeField] protected float f_explockBack;
    [SerializeField] protected float f_detonationTime;
    [SerializeField] protected float f_expRad;
    [SerializeField] protected GameObject[] go_explarticles;

    #endregion

    #region Elemental 

    [SerializeField] protected Element[] eo_element;
    public Element[] AugElement { get { return eo_element; } set { eo_element = value; } }

    #endregion

    public AugmentType at_type;

    public void InitAudio(AudioClip[] _ac_use, AudioClip[] _ac_travel, AudioClip[] _ac_hit)
    {
        ac_useSound =       _ac_use;
        ac_travelSound =    _ac_travel;
        ac_hitSound =       _ac_hit;
    }

    public void InitInfo(float _f_weight, float _f_recoil, float _f_speed, float _f_heatsink, float _f_knockback, float _f_energy, int _i_damage, int _i_lode)
    {
        f_weight        = _f_weight;
        f_recoil        = _f_recoil;
        f_speed         = _f_speed;
        f_heatsink      = _f_heatsink;
        f_knockback     = _f_knockback;
        f_energyGauge   = _f_energy;
        i_damage        = _i_damage;
        i_lodeDamage    = _i_lode;
    }

    public void InitInfo(AugmentProperties _ap_data)
    {
        s_name      = _ap_data.s_name;
        f_weight    = _ap_data.f_weight;
        f_speed     = _ap_data.f_speed;
        f_heatsink  = _ap_data.f_heatsink;
        f_knockback = _ap_data.f_knockback;
        f_energyGauge = _ap_data.f_energyGauge;
        i_damage    = _ap_data.i_damage;
        i_lodeDamage = _ap_data.i_lodeDamage;
    }
    public void InitPhysical(float _f_width, float _f_lifetime, Color[] _a_keys, GameObject _go_projectile)
    {
        f_trWidth   = _f_width;
        f_trLifetime = _f_lifetime;
        A_trKeys    = _a_keys;
        go_weaponProjectile = _go_projectile;
    }
    public void InitPhysical(AugmentPhysicals _phys_aug)
    {
        f_trWidth = _phys_aug.f_trWidth;
        f_trLifetime = _phys_aug.f_trLifetime;
        if(_phys_aug.A_trKeys != null)
            A_trKeys = _phys_aug.A_trKeys;
        go_weaponProjectile = _phys_aug.go_projectile;
    }

    public void InitExplosion(int _i_dmg, int _i_lodedmg, float _f_knockback, float _f_detTime, float _f_rad, bool _b_imp, GameObject[] _go_explarticles)
    {
        i_explosionDamage   = _i_dmg;
        i_expLodeDamage     = _i_lodedmg;
        f_explockBack       = _f_knockback;
        f_detonationTime    = _f_detTime;
        f_expRad            = _f_rad;
        b_impact            = _b_imp;
        go_explarticles     = _go_explarticles;
    }

    /// <summary>
    /// Set the explosion data
    /// </summary>
    /// <param name="_ae_boom">Explosion data</param>
    public void InitExplosion(AugmentExplosion _ae_boom)
    {
        i_explosionDamage   = _ae_boom.i_damage;
        i_expLodeDamage     = _ae_boom.i_lodeDamage;
        f_explockBack       = _ae_boom.f_explockBack;
        f_detonationTime    = _ae_boom.f_detonationTime;
        f_expRad            = _ae_boom.f_radius;
        b_impact            = _ae_boom.b_impact;
        go_explarticles     = _ae_boom.go_explarticles;
    }

    public List<AudioClip[]> GetAudioProperties()
    {
        List<AudioClip[]> aAudio = new List<AudioClip[]>();
        aAudio.Add(ac_useSound);
        aAudio.Add(ac_travelSound);
        aAudio.Add(ac_hitSound);
        return aAudio;
    }

    public AugmentProperties GetAugmentProperties()
    {
        return new AugmentProperties(s_name, f_weight, f_recoil, f_speed, f_heatsink, f_knockback, f_energyGauge, i_damage, i_lodeDamage);
    }

    public AugmentPhysicals GetPhysicalProperties()
    {
        return new AugmentPhysicals(f_trWidth, f_trLifetime, A_trKeys, go_weaponProjectile);
    }

    public AugmentExplosion GetExplosionProperties()
    {
        return new AugmentExplosion(i_explosionDamage, i_expLodeDamage, f_explockBack, f_detonationTime, f_expRad, b_impact, go_explarticles);
    }

    /// <summary>
    /// Combine two augments, and return an entirely new augment
    /// </summary>
    /// <param name="a">Base augment</param>
    /// <param name="b">Augment to be added</param>
    /// <returns>New augment with both augment traits</returns>
    public static Augment Combine(Augment a, Augment b)
    {
        Augment c = new Augment();
        // Name stuffs
        string newName = "";
        if (a.Name.Contains(" "))
        {
            string newNamea = a.s_name.Split(' ')[0];
            string newNameb = b.s_name.Split(' ')[0];
            if (newNamea == newNameb)
                newName += "Super ";
            else
                newName += newNamea + " " + newNameb + " ";
            newName += a.s_name.Split(' ')[1];
            c.s_name += newName;
        }
        else
        {
            newName = a.Name + " " + b.Name;
            c.s_name = newName;
        }
        // Audio data
        c.ac_useSound = Utils.CombineArrays(a.ac_useSound, b.ac_useSound);
        c.ac_travelSound = Utils.CombineArrays(a.ac_travelSound, b.ac_travelSound);
        c.ac_hitSound = Utils.CombineArrays(a.ac_hitSound, b.ac_hitSound);
        // Info data
        c.i_damage = a.i_damage + b.i_damage;
        c.i_lodeDamage = a.i_lodeDamage + b.i_lodeDamage;
        c.f_weight = a.f_weight + b.f_weight;
        c.f_recoil = a.f_recoil + b.f_recoil;
        c.f_energyGauge = a.f_energyGauge + b.f_energyGauge;
        c.f_knockback = a.f_knockback + b.f_knockback;
        c.f_heatsink = a.f_heatsink + b.f_heatsink;
        // Physical data
        c.f_trWidth = a.f_trWidth + b.f_trWidth;
        c.f_trLifetime = a.f_trLifetime + b.f_trLifetime;
        c.A_trKeys = Utils.CombineArrays(a.A_trKeys, b.A_trKeys);
        ///TODO:
        ///figure out how to deal with projectiles
        c.go_weaponProjectile = a.go_weaponProjectile;
        // Explosion data
        c.i_explosionDamage = a.i_explosionDamage + b.i_explosionDamage;
        c.i_expLodeDamage = a.i_expLodeDamage + b.i_expLodeDamage;
        c.f_explockBack = a.f_explockBack + b.f_explockBack;
        c.f_detonationTime = a.f_detonationTime + b.f_detonationTime;
        c.f_expRad = a.f_expRad + b.f_expRad;
        c.go_explarticles = Utils.CombineArrays(a.go_explarticles, b.go_explarticles);
        // If any of them are set to impact, set to be impact
        if (a.b_impact)
            c.b_impact = true;
        else
            c.b_impact = false;
        return c;
    }

    /// <summary>
    /// UNFINISHED FUNCTION DO NOT USE
    /// Todo:
    /// Figure out how to seperate augments
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Augment Separate(Augment a, Augment b)
    {
        Augment c = new Augment();
        // Name stuffs
        string newName = "";
        if (a.Name.Contains(" "))
        {
            string newNamea = a.s_name.Split(' ')[0];
            string newNameb = b.s_name.Split(' ')[0];
            if (newNamea == newNameb)
                newName += "Super ";
            else
                newName += newNamea + " " + newNameb + " ";
            newName += a.s_name.Split(' ')[1];
            c.s_name += newName;
        }
        else
        {
            newName = a.Name + " " + b.Name;
            c.s_name = newName;
        }
        // Audio data
        c.ac_useSound = Utils.CombineArrays(a.ac_useSound, b.ac_useSound);
        c.ac_travelSound = Utils.CombineArrays(a.ac_travelSound, b.ac_travelSound);
        c.ac_hitSound = Utils.CombineArrays(a.ac_hitSound, b.ac_hitSound);
        // Info data
        c.i_damage = a.i_damage - b.i_damage;
        c.i_lodeDamage = a.i_lodeDamage - b.i_lodeDamage;
        c.f_weight = a.f_weight - b.f_weight;
        c.f_recoil = a.f_recoil - b.f_recoil;
        c.f_energyGauge = a.f_energyGauge - b.f_energyGauge;
        c.f_knockback = a.f_knockback - b.f_knockback;
        c.f_heatsink = a.f_heatsink - b.f_heatsink;
        // Physical data
        c.f_trWidth += b.f_trWidth;
        c.f_trLifetime += b.f_trLifetime;
        c.A_trKeys = Utils.CombineArrays(a.A_trKeys, b.A_trKeys);
        c.go_weaponProjectile = b.go_weaponProjectile;
        // Explosion data
        c.i_explosionDamage += b.i_explosionDamage;
        c.i_expLodeDamage += b.i_expLodeDamage;
        c.f_explockBack += b.f_explockBack;
        c.f_detonationTime += b.f_detonationTime;
        c.f_expRad += b.f_expRad;
        c.go_explarticles = Utils.CombineArrays(a.go_explarticles, b.go_explarticles);
        // If any of them are set to impact, set to be impact
        if (a.b_impact || b.b_impact)
            c.b_impact = true;
        else
            c.b_impact = false;
        return c;
    }

    /// <summary>
    /// Add augment a to augment b
    /// </summary>
    /// <param name="a">Base augment</param>
    /// <param name="b">Augment being added</param>
    /// <returns>Base augment</returns>
    public static Augment operator +(Augment a, Augment b)
    {
        a = Combine(a, b);
        return a;
    }
    /// <summary>
    /// UNFINISHED OPERATOR DO NOT USE
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Augment operator -(Augment a, Augment b)
    {
        a = Separate(a, b);
        return a;
    }
}
