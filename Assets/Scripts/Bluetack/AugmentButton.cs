using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentButton : MonoBehaviour, IPoolable
{
    public int i_buttonIndex;

    public void Clicked(/*GameObject wb*/)
    {
        //wb?.GetComponent<Workbench>()?.ClickAugment(i_buttonIndex);
        FindObjectOfType<Workbench>()?.AugPropertyDisplay.ClickAugment(i_buttonIndex);
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
        return "\\Augments\\";
    }
}
