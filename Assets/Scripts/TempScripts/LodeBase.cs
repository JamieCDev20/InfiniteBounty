using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LodeBase : Enemy, IPoolable
{
    [Header("Lode Base")]
    [SerializeField] private GameObject go_nuggetPrefab;
    [SerializeField] private int i_nuggetToSpawn;
    [SerializeField] private int i_nuggetsPerBurst;
    [SerializeField] private float f_nuggetForce;
    [SerializeField] private int[] iA_healthIntervals = new int[3];
    [SerializeField] private GameObject[] goA_damageMeshes = new GameObject[3];
    [Space, SerializeField] private bool b_isNetworkedObject;
    [SerializeField] private string s_path;

    protected override void Start()
    {
        base.Start();
    }

    protected void OnEnable()
    {
        transform.position += Vector3.one;
        transform.position -= Vector3.one;
    }

    internal override void TakeDamage(int _i_damage)
    {
        i_currentHealth -= _i_damage;
        for (int i = 0; i < iA_healthIntervals.Length; i++)
        {
            if (i_currentHealth <= iA_healthIntervals[i])
            {
                NuggetBurst();
                goA_damageMeshes[i].SetActive(false);
                iA_healthIntervals[i] = 0;
            }
        }

        if (i_currentHealth <= 0) Death();
    }

    internal override void Death()
    {
        gameObject.SetActive(false);
        NuggetBurst();
    }

    private void NuggetBurst()
    {
        for (int i = 0; i < i_nuggetsPerBurst; i++)
        {
            GameObject _go_nugget = PoolManager.x.SpawnObject(go_nuggetPrefab, transform.position, transform.rotation);
            _go_nugget.SetActive(true);
            _go_nugget.transform.parent = null;
            _go_nugget.transform.position = transform.position + transform.localScale * (-1 + Random.value * 2);
            _go_nugget.transform.localScale = Vector3.one;
            Rigidbody _rb = _go_nugget.GetComponent<Rigidbody>();
            _rb.AddForce(new Vector3(-1 + Random.value * 2, Random.value * 2, -1 + Random.value * 2) * f_nuggetForce, ForceMode.Impulse);
            _go_nugget.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Die()
    {
        Death();
    }

    public bool IsNetworkedObject()
    {
        return b_isNetworkedObject;
    }

    public string ResourcePath()
    {
        return s_path;
    }
}