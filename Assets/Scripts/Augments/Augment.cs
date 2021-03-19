using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Augment
{
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected string s_name;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected int i_level;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected int i_cost;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected AugmentStage as_stage;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected string mat_augColor;
    public string Name { get { return s_name; } }
    public int Level { get { return i_level; } set { i_level = value; } }
    public int Cost { get { return i_cost; } set { i_cost = value; } }
    public AugmentStage Stage { get { return as_stage; } set { as_stage = value; } }
    public string AugmentMaterial { get { return mat_augColor; } set { mat_augColor = value; } }
    #region Audio

    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected string[] ac_useSound;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected string[] ac_travelSound;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected string[] ac_hitSound;

    #endregion

    #region Tool Information Properties

    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_weight;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_recoil;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_speed;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_heatsink;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_knockback;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_energyGauge;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected int i_damage;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected int i_lodeDamage;

    #endregion

    #region Tool Physical Properties

    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_trWidth;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_trLifetime;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected Color[] A_trKeys;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected string go_weaponProjectile;

    #endregion

    #region EXPLOSION

    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected int i_explosionDamage;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected int i_expLodeDamage;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected bool b_impact;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_explockBack;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_detonationTime;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected float f_expRad;
    [Newtonsoft.Json.JsonProperty]
    [SerializeField] protected string[] go_explarticles;

    #endregion

    #region Elemental 

    [SerializeField] protected Element[] eo_element;
    public Element[] AugElement { get { return eo_element; } set { eo_element = value; } }

    #endregion

    public AugmentType at_type;

    public void InitAudio(AudioClip[] _ac_use, AudioClip[] _ac_travel, AudioClip[] _ac_hit)
    {
        if(_ac_use != null)
        {
            ac_useSound = new string[_ac_use.Length];
            for(int i = 0; i < _ac_use.Length; i++)
                ac_useSound[i] = _ac_use[i].name;
        }
        if(_ac_travel != null)
        {
            ac_travelSound = new string[_ac_travel.Length];
            for(int i = 0; i < _ac_travel.Length; i++)
                ac_travelSound[i] = _ac_travel[i].name;
        }
        if(_ac_hit != null)
        {
            ac_hitSound = new string[_ac_hit.Length];
            for(int i = 0; i < _ac_hit.Length; i++)
                ac_hitSound[i] = _ac_hit[i].name;
        }
    }
    public void InitAudio(string[] _ac_use, string[] _ac_travel, string[] _ac_hit)
    {
        ac_useSound     = _ac_use;
        ac_travelSound  = _ac_travel;
        ac_hitSound     = _ac_hit;
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
    public void InitPhysical(float _f_width, float _f_lifetime, Color[] _a_keys, string _go_projectile)
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

    public void InitExplosion(int _i_dmg, int _i_lodedmg, float _f_knockback, float _f_detTime, float _f_rad, bool _b_imp, string[] _go_explarticles)
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

    public List<string[]> GetAudioProperties()
    {
        List<string[]> aAudio = new List<string[]>();
        aAudio.Add(ac_useSound);
        aAudio.Add(ac_travelSound);
        aAudio.Add(ac_hitSound);
        return aAudio;
    }

    public AugmentProperties GetAugmentProperties()
    {
        return new AugmentProperties(s_name, i_cost, f_weight, f_recoil, f_speed, f_heatsink, f_knockback, f_energyGauge, i_damage, i_lodeDamage);
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
        c.i_cost = a.Cost + b.Cost;
        // If any of them are set to impact, set to be impact
        if (a.b_impact)
            c.b_impact = true;
        else
            c.b_impact = false;
        return c;
    }

    /// <summary>
    /// UNFINISHED FUNCTION DO NOT USE
    /// TODO:
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
