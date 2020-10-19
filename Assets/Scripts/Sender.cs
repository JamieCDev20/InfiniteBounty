using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender : MonoBehaviour
{

    //Variables
    #region Serialised

    [SerializeField] private whichType sendType;
    [SerializeField] private int loops = 100000;
    [SerializeField] private GameObject reciever;

    #endregion

    #region Private

    private float t;
    private Reciever Rcv;

    #endregion

    //Methods
    #region Unity Standards

    private void Start()
    {
        Rcv = reciever.GetComponent<Reciever>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Send();
    }

    #endregion

    #region Private Voids

    private void Send()
    {

        t = Time.realtimeSinceStartup * 1000;
        switch (sendType)
        {
            case whichType.sendMessage:
                for (int i = 0; i < loops; i++)
                {
                    reciever.SendMessage("Recieve");
                }
                break;
            case whichType.getComponent:
                for (int i = 0; i < loops; i++)
                {
                    reciever.GetComponent<Reciever>().Recieve();
                }
                break;
            case whichType.directReferenceLoop:
                for (int i = 0; i < loops; i++)
                {
                    Rcv.Recieve();
                }
                break;
            case whichType.directReferenceRecursive:
                SendToReciever(Rcv, 0);
                break;
            default:
                break;
        }

        t = -(t - Time.realtimeSinceStartup * 1000);

        Debug.LogFormat("{1} Operation of {2} loops took: {0} ms", t, sendType.ToString(), loops);

    }

    private void SendToReciever(Reciever r, int i)
    {
        if (i >= loops)
            return;
        r.Recieve();
        SendToReciever(r, i + 1);
    }

    #endregion

    #region Public Voids


    #endregion

    #region Private Returns


    #endregion

    #region Public Returns


    #endregion

}

public enum whichType
{
    sendMessage,
    getComponent,
    directReferenceLoop,
    directReferenceRecursive
}