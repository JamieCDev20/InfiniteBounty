using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour, ObserverBase
{
    public static InfoText x;
    GameObject[] A_allText;
    [SerializeField] float f_textDistance;
    [SerializeField] float f_textPadding;
    [SerializeField] private GameObject listMover;
    [SerializeField] private GameObject textObject;

    public void Start()
    {
        FindObjectOfType<SaveManager>().AddObserver(this);
        if(x != null)
        {
            if (x != this)
                Destroy(this);
        }
        else
            x = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnNotify(ObserverEvent oe_event)
    {
        switch (oe_event)
        {
            case InfoTextEvent ite:
                if (ite.TextToDisplay != string.Empty)
                    AddText(ite.TextToDisplay);
                DumpText();
                break;
            case DeleteInfoTextEvent dite:
                A_allText = Utils.OrderedRemove(A_allText, FindTextIndex(dite.ToRemove));
                break;
        }
    }

    private int FindTextIndex(GameObject _textRef)
    {
        for (int i = 0; i < A_allText.Length; i++)
        {
            if (A_allText[i] == _textRef)
                return i;
        }
        return 0;
    }

    public void AddText(string _textToAdd)
    {
        GameObject textRef = PoolManager.x.SpawnObject(textObject);
        if(textRef.activeSelf == false)
            textRef.SetActive(true);
        InfoTextObject tr = textRef.GetComponent<InfoTextObject>();
        Text actText = tr.TextObject;
        actText.color = new Color(actText.color.r, actText.color.g, actText.color.b, 1f);
        tr.Info = _textToAdd;
        tr.StartFadeRoutine(3f, 1f);
        A_allText = Utils.AddToArray(A_allText, textRef);
    }

    public void DumpText()
    {
        int rev = 0;
        if(A_allText.Length > 0)
            for(int i = A_allText.Length-1; i >= 0; i--)
            {
                if(A_allText[i] != null)
                {
                    A_allText[i].transform.parent = listMover.transform;
                    A_allText[i].transform.localScale = Vector3.one;
                    A_allText[i].transform.localPosition = new Vector3(0, (-rev * textObject.GetComponent<RectTransform>().rect.height) - f_textPadding, 0);
                    A_allText[i].SetActive(true);
                    rev++;
                }
            }
    }

    public void DisplayText()
    {
        A_allText[A_allText.Length - 1].transform.parent = listMover.transform;
        A_allText[A_allText.Length - 1].transform.localPosition = Vector3.zero;
    }

    public void Reset()
    {
        foreach(GameObject go_text in A_allText)
        {
            InfoTextObject ito = go_text.GetComponent<InfoTextObject>();
            ito.Die();
        }
        A_allText = new GameObject[0];
    }
}
