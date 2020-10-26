using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private int i_damage;
    [SerializeField] private GameObject go_hitEffect;

    public void Setup(AugmentType[] _atA_activeAugments)
    {
        for (int i = 0; i < _atA_activeAugments.Length; i++)
        {

            

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Hittable")
        {
            collision.transform.GetComponent<Enemy>().TakeDamage(i_damage);
        }
        go_hitEffect.transform.parent = null;
        go_hitEffect.transform.position = transform.position;
        go_hitEffect.SetActive(true);
        gameObject.SetActive(false);
    }

}