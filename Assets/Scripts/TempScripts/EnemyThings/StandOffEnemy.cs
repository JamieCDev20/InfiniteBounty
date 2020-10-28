using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandOffEnemy : Enemy
{

    [SerializeField] private float f_walkSpeed;


    protected override void Start()
    {
        base.Start();
        transform.LookAt(GameObject.FindGameObjectWithTag("Stockpile").transform);

    }

    private void Update()
    {
        transform.position += transform.forward * f_walkSpeed * Time.deltaTime;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Stockpile"))
        {
            Death();
            collision.transform.GetComponent<StockPile>().TakeDamage();
        }
    }

}