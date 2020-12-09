using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelSpawnController : MonoBehaviour
{

    private PlayerInputManager pim;
    [SerializeField] private GameObject go_impactEffects;
    [SerializeField] private float f_fireForce = 10;

    public void SetupPlayer(GameObject _go_playerToSetup)
    {
        pim = _go_playerToSetup.GetComponent<PlayerInputManager>();
        pim.b_shouldPassInputs = false;
        _go_playerToSetup.transform.position = transform.position + Vector3.up * 500;
        _go_playerToSetup.GetComponent<Rigidbody>().velocity = Vector3.down * f_fireForce;
        _go_playerToSetup.transform.forward = transform.forward;
        pim.GetCamera().transform.forward = transform.forward;

        Invoke("PlayerImpact", 1f);
    }

    private void PlayerImpact()
    {
        go_impactEffects.SetActive(true);
        pim.b_shouldPassInputs = true;
    }


}