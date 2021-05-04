using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheScriptThatFinishesTheGame : MonoBehaviour
{
    [SerializeField] private string s_sceneToLoad;
    private bool b_shouldSuck;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        b_shouldSuck = true;

        Collider[] _cA = Physics.OverlapSphere(transform.position, 1.5f);

        for (int i = 0; i < _cA.Length; i++)
        {
            if (_cA[i].gameObject.isStatic)
                continue;

            if (_cA[i].CompareTag("Player"))
                SceneManager.LoadScene(s_sceneToLoad);
            else
                StartCoroutine(InhaleObject(_cA[i]));
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SceneManager.LoadScene(s_sceneToLoad);
        else
            StartCoroutine(InhaleObject(other));
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(InhaleObject(other));
    }

    private IEnumerator InhaleObject(Collider other)
    {
        if (other.transform.root != transform.root)
            //if (!other.gameObject.isStatic)
            if (b_shouldSuck)
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return new WaitForSeconds(0.01f);
                    other.transform.position = Vector3.Lerp(other.transform.position, transform.position, 0.5f);
                    other.transform.localScale = Vector3.Lerp(other.transform.localScale, Vector3.zero, 0.3f);
                    other.transform.Rotate((transform.up * 10) + transform.forward * 10);
                }
                other.gameObject.SetActive(false);
            }
    }
}
