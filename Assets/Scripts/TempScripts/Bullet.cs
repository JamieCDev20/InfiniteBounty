using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{

    [SerializeField] private int i_damage;
    [SerializeField] private GameObject go_hitEffect;
    [SerializeField] private LayerMask lm_placementLayer;

    [Header("TrailEffects")]
    [SerializeField] private TrailRenderer tr_bulletTrail;
    [SerializeField] private GameObject go_flameTrails;
    [SerializeField] private GameObject go_electricTrails;

    private bool b_explosive;
    private bool b_gooey;
    private bool b_soaked;
    [SerializeField] private Rigidbody rb;

    protected bool b_inPool;
    protected int i_poolIndex;

    public void Setup(AugmentType[] _atA_activeAugments)
    {
        RemoveAugments(_atA_activeAugments);

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

    private void RemoveAugments(AugmentType[] _atA_activeAugments)
    {
        if (!_atA_activeAugments.Contains(AugmentType.Flame))
            GetComponent<Systemic>().b_fire = false;

        if (!_atA_activeAugments.Contains(AugmentType.Electric))
            GetComponent<Systemic>().b_electric = false;

        if (!_atA_activeAugments.Contains(AugmentType.Heavy))
            GetComponent<Rigidbody>().useGravity = false;

        if (!_atA_activeAugments.Contains(AugmentType.Size))
            transform.localScale = Vector3.one;

        if (!_atA_activeAugments.Contains(AugmentType.Explosive))
            b_explosive = false;

        if (!_atA_activeAugments.Contains(AugmentType.Gooey))
            b_gooey = false;

        if (!_atA_activeAugments.Contains(AugmentType.Soaked))
            b_soaked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Hittable")
            collision.transform.GetComponent<Enemy>().TakeDamage(i_damage);

        go_hitEffect.SetActive(false);
        go_hitEffect.transform.parent = transform;
        go_hitEffect.transform.localPosition = Vector3.zero;
        go_hitEffect.transform.parent = null;        
        go_hitEffect.SetActive(true);

        transform.LookAt(collision.transform);

        if (b_explosive)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position - (transform.forward * 5), transform.forward, out _hit, 10, gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                GameObject _goo = Instantiate(LocationController.x.go_explosionPrefab, _hit.point, Quaternion.identity);
                _goo.transform.up = _hit.normal;
            }
        }
        if (b_gooey)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position - (transform.forward * 5), transform.forward, out _hit, 10, gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                GameObject _goo = Instantiate(LocationController.x.go_gooPatchPrefab, _hit.point, Quaternion.identity);
                _goo.transform.up = _hit.normal;
            }
        }
        if (b_soaked)
        {
            RaycastHit _hit;
            if (Physics.Raycast(transform.position - (transform.forward * 5), transform.forward, out _hit, 10, gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                GameObject _goo = Instantiate(LocationController.x.go_waterPuddlePrefab, _hit.point, Quaternion.identity);
                _goo.transform.up = _hit.normal;
            }
        }

        Die();
    }

    #region Pooling

    public void Die()
    {
        if (PoolManager.x != null) PoolManager.x.ReturnInactiveToPool(gameObject, i_poolIndex);
        SetInPool(true);
        tr_bulletTrail.Clear();
        rb.velocity = Vector3.zero;
    }

    public int GetIndex()
    {
        return i_poolIndex;
    }

    public bool GetInPool()
    {
        return b_inPool;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public void OnSpawn()
    {
        // Set position, play sounds, some other stuff maybe.
    }

    public void SetIndex(int _i_index)
    {
        i_poolIndex = _i_index;
    }

    public void SetInPool(bool _b_delta)
    {
        b_inPool = _b_delta;
    }
    #endregion
}