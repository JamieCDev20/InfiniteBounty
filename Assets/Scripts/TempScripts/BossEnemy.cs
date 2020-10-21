using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{

    [Header("Boss Info")]
    [SerializeField] private List<GameObject> goL_playerObjects = new List<GameObject>();
    private int i_targetIndex;
    private GameObject go_looker;
    [SerializeField] private float f_walkSpeed;
    private Rigidbody rb_rigidbody;

    [Header("Boss-Lode Stats")]
    [SerializeField] private GameObject go_nuggetPrefab;
    private List<GameObject> goL_nuggetPool = new List<GameObject>();
    [SerializeField] private int i_nuggetsToSpawn;
    [SerializeField] private int i_nuggetsPerBurst;
    [SerializeField] private float f_nuggetForce;
    [SerializeField] private int[] iA_healthIntervals = new int[0];
    [SerializeField] private GameObject[] goA_damageMeshes = new GameObject[3];

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < i_nuggetsToSpawn; i++)
        {
            goL_nuggetPool.Add(Instantiate(go_nuggetPrefab, transform));
            goL_nuggetPool[i].SetActive(false);
        }
        go_looker = new GameObject("BossLooker");
        rb_rigidbody = GetComponent<Rigidbody>();
        InvokeRepeating("FindNewTarget", 0, 10);
    }

    internal override void TakeDamage(int _i_damage)
    {
        i_currentHealth -= _i_damage;

        if (i_currentHealth <= 0) Death();

        for (int i = 0; i < iA_healthIntervals.Length; i++)
        {
            if (i_currentHealth <= iA_healthIntervals[i])
            {
                NuggetBurst();
                goA_damageMeshes[i].SetActive(false);
                iA_healthIntervals[i] = 0;
            }
        }

    }

    private void NuggetBurst()
    {
        for (int i = 0; i < i_nuggetsPerBurst; i++)
        {
            GameObject _go_nugget = goL_nuggetPool[0];
            goL_nuggetPool.RemoveAt(0);
            _go_nugget.SetActive(true);
            _go_nugget.transform.parent = null;
            _go_nugget.transform.position = transform.position + transform.localScale * (-1 + Random.value * 2) + (Vector3.up * 2);
            _go_nugget.transform.localScale = Vector3.one;
            Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
            _rb.AddForce(new Vector3(-1 + Random.value * 2, Random.value * 2, -1 + Random.value * 2) * f_nuggetForce, ForceMode.Impulse);
            _go_nugget.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);
        }
    }

    private void Update()
    {
        go_looker.transform.LookAt(goL_playerObjects[i_targetIndex].transform.position);
        go_looker.transform.position = transform.position;

        transform.position += transform.forward * f_walkSpeed;

        transform.forward = Vector3.Scale(Vector3.Lerp(transform.forward, go_looker.transform.forward, 0.3f), new Vector3(1, 0, 1));
    }

    private void FindNewTarget()
    {
        i_targetIndex = Random.Range(0, goL_playerObjects.Count);
    }

}
