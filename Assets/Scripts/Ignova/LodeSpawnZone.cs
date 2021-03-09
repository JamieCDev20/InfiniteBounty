using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodeSpawnZone : MonoBehaviour
{
    private float f_zoneRadius;
    [SerializeField] private Vector2Int vi_numberOfLodes;
    [SerializeField] private GameObject[] goA_lodesTypesToSpawn;

    [Header("Raycast Things")]
    [SerializeField] private bool b_doRayCastSpawning;
    [SerializeField] private LayerMask lm_lodeSpawnLayer;
    [SerializeField] private string s_namesToNotSpawnOn = "*";

    private void Awake()
    {
        f_zoneRadius = GetComponent<SphereCollider>().radius;
    }

    internal void SpawnLode(Randomness _random, int seed)
    {
        Random.InitState(seed);
        GameObject _go_lode;
        int _i_lodeCount = Random.Range(vi_numberOfLodes.x, vi_numberOfLodes.y);

        if (b_doRayCastSpawning)
        {
            RaycastHit hit;
            for (int x = 0; x < _i_lodeCount; x++)
            {
                transform.localEulerAngles = new Vector3(Random.Range(-85, 85), Random.Range(-85, 85), 0);

                if (Physics.Raycast(transform.position, -transform.forward, out hit, f_zoneRadius, lm_lodeSpawnLayer))
                {
                    if (hit.transform.gameObject.layer == lm_lodeSpawnLayer)
                    {
                        _go_lode = Instantiate(goA_lodesTypesToSpawn[Random.Range(0, goA_lodesTypesToSpawn.Length)]);
                        _go_lode.transform.position = hit.point;
                        _go_lode.transform.up = hit.normal;
                        _random.LodeSpawned(_go_lode);
                    }
                    else
                        x--;
                }
                else
                    x--;
            }
        }
        else
        {
            for (int x = 0; x < _i_lodeCount; x++)
            {
                _go_lode = Instantiate(goA_lodesTypesToSpawn[Random.Range(0, goA_lodesTypesToSpawn.Length)], transform);
                _random.LodeSpawned(_go_lode);
            }
        }
    }

}