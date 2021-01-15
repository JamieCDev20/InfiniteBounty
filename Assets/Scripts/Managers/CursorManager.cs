using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager x;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D clickCursor;

    private void Awake()
    {
        if (x != null)
            Destroy(gameObject);
        else
            x = this;
        DontDestroyOnLoad(gameObject);
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Cursor.SetCursor(clickCursor, Vector2.zero, CursorMode.Auto);
        if (Input.GetMouseButtonUp(0))
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);

    }

}
