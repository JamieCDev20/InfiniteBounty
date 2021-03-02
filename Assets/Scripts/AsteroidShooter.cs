using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidShooter : MonoBehaviour
{
    [SerializeField] private GameObject go_asteroidPrefab = null;
    private List<GameObject> goL_asteroidPool = new List<GameObject>();
    [SerializeField] private Vector2 v_lifeTime;
    [Space]
    [SerializeField] private Vector3 v_shootDirMax;
    [SerializeField] private Vector3 v_shootDirMin;
    [Space]
    [SerializeField] private Vector3 v_rotSpeedMax;
    [SerializeField] private Vector3 v_rotSpeedMin;

    IEnumerator Start()
    {
        for (int i = 0; i < 100; i++)
        {
            goL_asteroidPool.Add(Instantiate(go_asteroidPrefab, transform));
            goL_asteroidPool[i].SetActive(false);
        }

        for (int i = 0; i < 100; i++)
        {
            StartCoroutine(ShootAsteroid());
            yield return new WaitForSeconds(Random.Range(3, 10));
        }
    }

    private IEnumerator ShootAsteroid()
    {
        GameObject _go = goL_asteroidPool[0];
        goL_asteroidPool.RemoveAt(0);
        _go.SetActive(true);
        _go.transform.position = transform.position + new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));

        Rigidbody rb = _go.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(Random.Range(v_shootDirMin.x, v_shootDirMax.x), Random.Range(v_shootDirMin.y, v_shootDirMax.y), Random.Range(v_shootDirMin.z, v_shootDirMax.z));
        rb.angularVelocity = new Vector3(Random.Range(v_rotSpeedMin.x, v_rotSpeedMax.x), Random.Range(v_rotSpeedMin.y, v_rotSpeedMax.y), Random.Range(v_rotSpeedMin.z, v_rotSpeedMax.z));
        float _f_targetSize = Random.Range(3, 20);

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            _go.transform.localScale = Vector3.Lerp(Vector3.zero, _f_targetSize * Vector3.one, 0.5f);
        }

        yield return new WaitForSeconds(Random.Range(v_lifeTime.x, v_lifeTime.y));

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            _go.transform.localScale = Vector3.Lerp(_go.transform.localScale, Vector3.zero, 0.5f);
        }

        _go.SetActive(false);
        goL_asteroidPool.Add(_go);

        yield return new WaitForSeconds(Random.Range(0, 5));
        StartCoroutine(ShootAsteroid());
    }

}
