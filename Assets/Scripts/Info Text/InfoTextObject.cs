using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoTextObject : MonoBehaviour, IPoolable
{
    [SerializeField] string s_filePath;
    [SerializeField] private Text t_text;
    private bool b_setTime = false;
    private bool b_cRunning;
    private float f_startTime;
    private float f_startAlpha;

    public Text TextObject { get { return t_text; } }
    public string Info { set { t_text.text = value;} }

    private void Awake()
    {
        StopAllCoroutines();
        t_text.color = new Color(t_text.color.r, t_text.color.g, t_text.color.b, 1);
    }

    public void StartFadeRoutine(float _showTime, float _fadeTime)
    {
        StartCoroutine(FadeText(_showTime, _fadeTime));
    }

    public void Reset()
    {
        if (b_cRunning)
        {
            StopAllCoroutines();
            transform.parent = null;
            t_text.color = new Color(t_text.color.r, t_text.color.g, t_text.color.b, t_text.color.a);
        }
        InfoText.x.OnNotify(new DeleteInfoTextEvent(gameObject));
        transform.localPosition = Vector3.zero;
        transform.position = Vector3.zero;
    }

    public void Die()
    {
        PoolManager.x.ReturnObjectToPool(gameObject);
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
        return s_filePath;
    }

    public IEnumerator FadeText(float _showTime, float _fadeTime)
    {
        b_cRunning = true;
        yield return new WaitForSeconds(_showTime);
        if (!b_setTime)
        {
            f_startTime = Time.time;
            f_startAlpha = t_text.color.a;
            b_setTime = true;
        }
        while(t_text.color.a != 0)
        {
            float dist = (Time.time - f_startTime) * _fadeTime;
            float frag = dist / f_startAlpha;
            Color temp = t_text.color;
            temp.a = Mathf.Lerp(f_startAlpha, 0, frag);
            t_text.color = temp;
            yield return null;
        }
        gameObject.SetActive(false);
        b_cRunning = false;
        Reset();
    }
}
