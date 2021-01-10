using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 v_unitsPerSecond;
    [SerializeField] private bool b_shouldSinBackToStart;
    [SerializeField] private int i_damageToDeal;
    private List<IHitable> hL_thingsWithinCloud = new List<IHitable>();
    private float f_time;
    [SerializeField] private float f_timeBetweenDamages;
    [SerializeField] private bool b_dealDamageOnEntry;

    private void Update()
    {
        if (b_shouldSinBackToStart)
            transform.position += (v_unitsPerSecond * Mathf.Sin(Time.realtimeSinceStartup));
        else
            transform.position += v_unitsPerSecond * Time.deltaTime;

        f_time += Time.deltaTime;
        if (f_time >= f_timeBetweenDamages)
            DealDamage();

    }

    private void OnTriggerEnter(Collider other)
    {
        IHitable _h = other.GetComponent<IHitable>();

        if (_h != null)
            hL_thingsWithinCloud.Add(_h);

        if (b_dealDamageOnEntry)
            _h.TakeDamage(i_damageToDeal);
    }

    private void OnTriggerExit(Collider other)
    {
        IHitable _h = other.GetComponent<IHitable>();

        if (hL_thingsWithinCloud.Contains(_h))
            hL_thingsWithinCloud.Remove(_h);
    }

    private void DealDamage()
    {
        for (int i = 0; i < hL_thingsWithinCloud.Count; i++)
        {
            if (hL_thingsWithinCloud[i].IsDead())
            {
                hL_thingsWithinCloud.RemoveAt(i);
                i -= 1;
                continue;
            }
            hL_thingsWithinCloud[i].TakeDamage(i_damageToDeal);
        }
        f_time = 0;
    }
}