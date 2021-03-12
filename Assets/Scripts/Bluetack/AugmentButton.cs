using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentButton : MonoBehaviour, IPoolable
{
    public int i_buttonIndex;
    private GameObject go_parent;
    public GameObject Parent { set { go_parent = value; } }

    public void Clicked(/*GameObject wb*/)
    {
        Workbench wb = go_parent.GetComponentInChildren<Workbench>();
        Microwave mw = go_parent.GetComponentInChildren<Microwave>();
        if(wb != null)
            wb.AugPropertyDisplay.ClickAugment(i_buttonIndex);
        else if(mw != null)
        {
            mw.AugPropertyDisplay.ClickAugment(i_buttonIndex);
            mw.SetAugment();
        }
    }

    public void Die()
    {
        if (PoolManager.x != null)
        {
            PoolManager.x.ReturnObjectToPool(gameObject);
        }
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
