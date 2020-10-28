using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{

    [Header("Explosion Effects")]
    [SerializeField] private GameObject go_explosion;

    public void Explode()
    {
        Instantiate(go_explosion, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }


}
