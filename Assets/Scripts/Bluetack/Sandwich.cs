using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandwich : MonoBehaviour, IPoolable
{
    [SerializeField] private int i_healthGain;
    [SerializeField] private GameObject go_pickedUpEffect;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<IHitable>().TakeDamage(-i_healthGain, false);

            go_pickedUpEffect.transform.parent = null;
            go_pickedUpEffect.transform.position = transform.position;
            go_pickedUpEffect.SetActive(false);
            go_pickedUpEffect.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool IsNetworkedObject()
    {
        return false;
    }

    public string ResourcePath()
    {
        return "NetworkedObjects\\PlayerMade";
    }
}