using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AugmentGo : MonoBehaviour
{
    [SerializeField] Augment a_aug;
    [SerializeField] Material mat_ref;
    AudioClip[] ac_useSound;
    AudioClip[] ac_travelSound;
    AudioClip[] ac_hitSound;
    public Augment Aug { get { return a_aug; } set { a_aug = value; } }
    public Material Mat { set { mat_ref = value; } }

    public void ApplyMaterial(string _matName)
    {
        if(_matName != string.Empty && _matName != " ")
        {
            Material _mat = Resources.Load<Material>("AugmentColours/" + _matName);
            if(_mat != null)
            {
                mat_ref = _mat;
                GetComponent<MeshRenderer>().material = mat_ref;
            }
        }

    }
}
