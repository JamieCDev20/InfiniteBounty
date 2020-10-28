using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugPlayer : SubjectBase
{
    public int i_playerID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<NugGO>() != null)
        {
            Nug collectedNug = collision.gameObject.GetComponent<NugGO>().nug;
            //CurrencyEvent ce = new CurrencyEvent(collectedNug.i_worth, true);
            collision.gameObject.SetActive(false);
            //Notify(ce);
        }
        else Debug.Log("Fugma");
            
    }
}
