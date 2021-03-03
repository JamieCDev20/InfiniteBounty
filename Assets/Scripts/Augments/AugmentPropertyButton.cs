using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentPropertyButton : MonoBehaviour, IPoolable
{
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
        return "\\";
    }
}
