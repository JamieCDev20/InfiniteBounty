using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodeSpawnZone : MonoBehaviour
{
    [SerializeField] private float f_zoneRadius;
    [SerializeField] private Vector2Int vi_numberOfLodes;
    [SerializeField] private GameObject[] goA_lodesTypesToSpawn;

    [Header("Raycast Things")]
    [SerializeField] private bool b_doRayCastSpawning;
    [SerializeField] private LayerMask lm_lodeSpawnLayer;

    internal void SpawnLode(Randomness _random, int seed)
    {
        Random.InitState(seed);
        GameObject _go_lode;

        if (b_doRayCastSpawning)
        {
            RaycastHit hit;

            for (int x = 0; x < Random.Range(vi_numberOfLodes.x, vi_numberOfLodes.y); x++)
            {
                transform.forward = -transform.up;
                transform.Rotate(Random.Range(-85, 85), Random.Range(-85, 85), 0, Space.World);

                if (Physics.Raycast(transform.position, transform.forward, out hit, f_zoneRadius, lm_lodeSpawnLayer))
                {
                    _go_lode = Instantiate(goA_lodesTypesToSpawn[Random.Range(0, goA_lodesTypesToSpawn.Length)], transform);
                    _go_lode.transform.position = hit.point;
                    _go_lode.transform.up = hit.normal;
                    _random.LodeSpawned(_go_lode);
                }
            }
        }
        else
        {
            for (int x = 0; x < Random.Range(vi_numberOfLodes.x, vi_numberOfLodes.y); x++)
            {
                _go_lode = Instantiate(goA_lodesTypesToSpawn[Random.Range(0, goA_lodesTypesToSpawn.Length)], transform);
                _random.LodeSpawned(_go_lode);
            }
        }
    }
}
