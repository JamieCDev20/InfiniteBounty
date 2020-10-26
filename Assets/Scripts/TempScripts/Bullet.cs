using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private int i_damage;
    [SerializeField] private GameObject go_hitEffect;

    [Header("TrailEffects")]
    [SerializeField] private GameObject go_flameTrails;
    [SerializeField] private GameObject go_electricTrails;

    private bool b_explosive;
    private bool b_gooey;
    private bool b_soaked;


    public void Setup(AugmentType[] _atA_activeAugments)
    {
        for (int i = 0; i < _atA_activeAugments.Length; i++)
        {
            switch (_atA_activeAugments[i])
            {
                case AugmentType.Flame:
                    GetComponent<Systemic>().b_fire = true;
                    break;

                case AugmentType.Electric:
                    GetComponent<Systemic>().b_electric = true;
                    break;

                case AugmentType.Heavy:
                    GetComponent<Rigidbody>().useGravity = true;
                    break;

                case AugmentType.Size:
                    transform.localScale = transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
                    break;

                case AugmentType.Explosive:
                    b_explosive = true;
                    break;

                case AugmentType.Gooey:
                    b_gooey = true;
                    break;

                case AugmentType.Soaked:
                    b_soaked = true;
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Hittable")
            collision.transform.GetComponent<Enemy>().TakeDamage(i_damage);

        go_hitEffect.transform.parent = null;
        go_hitEffect.transform.position = transform.position;
        go_hitEffect.SetActive(true);
        transform.LookAt(collision.transform);

        if (b_explosive)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, transform.forward, out _hit))
            {
                GameObject _goo = Instantiate(LocationController.x.go_explosionPrefab, transform.position, Quaternion.identity);
                _goo.transform.up = _hit.normal;
            }
        }
        if (b_gooey)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, transform.forward, out _hit))
            {
                GameObject _goo = Instantiate(LocationController.x.go_gooPatchPrefab, transform.position, Quaternion.identity);
                _goo.transform.up = _hit.normal;
            }
        }
        if (b_soaked)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position, transform.forward, out _hit))
            {
                GameObject _goo = Instantiate(LocationController.x.go_waterPuddlePrefab, transform.position, Quaternion.identity);
                _goo.transform.up = _hit.normal;
            }
        }

        gameObject.SetActive(false);
    }
}