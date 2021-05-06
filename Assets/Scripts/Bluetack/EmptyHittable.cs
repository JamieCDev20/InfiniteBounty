using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyHittable : MonoBehaviour, IHitable
{

    private ElementalObject e;

    private void Start()
    {
        e = GetComponent<ElementalObject>();
    }

    public void Die()
    {
    }

    public bool IsDead()
    {
        return true;
    }

    public void TakeDamage(int damage, bool activatesThunder)
    {
        e.ActivateElement(activatesThunder);
    }

    public void TakeDamage(int damage, bool activatesThunder, float delay)
    {
        StartCoroutine(DelayDamage(damage, activatesThunder, delay));
    }

    IEnumerator DelayDamage(int damage, bool thunder, float delay)
    {
        yield return new WaitForSeconds(delay);
        TakeDamage(damage, thunder);
    }

}
