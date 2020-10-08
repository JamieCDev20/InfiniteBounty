using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private int i_damage;
    [SerializeField] private GameObject go_hitEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().TakeDamage(i_damage);
            go_hitEffect.transform.parent = null;
            go_hitEffect.transform.position = transform.position;
            go_hitEffect.SetActive(true);
            gameObject.SetActive(false);
        }
    }

}
