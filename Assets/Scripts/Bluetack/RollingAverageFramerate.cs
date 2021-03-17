using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollingAverageFramerate : MonoBehaviour
{

    public static RollingAverageFramerate x;

    private List<float> lastFrames = new List<float>();

    [SerializeField] private Text frText;
    [SerializeField] private Text pingText;

    private Canvas can;

    private void Awake()
    {
        if (x != null)
        {
            if (x != this)
                Destroy(gameObject);
        }
        else
            x = this;
        DontDestroyOnLoad(gameObject);

        can = GetComponentInChildren<Canvas>();
        can.enabled = false;

    }

    private void Update()
    {
        lastFrames.Add(Time.deltaTime);
        if (lastFrames.Count >= 50)
            lastFrames.RemoveAt(0);

        float t = 0;

        for (int i = 0; i < lastFrames.Count; i++)
        {
            t += lastFrames[i];
        }
        t /= lastFrames.Count;

        t = 1 / t;

        if(Input.GetKeyDown(KeyCode.Tab))
            can.enabled = !can.enabled;

        frText.text = $"FPS: {t}";
        pingText.text = $"Ping: {PhotonNetwork.GetPing()}ms";
    }

}
