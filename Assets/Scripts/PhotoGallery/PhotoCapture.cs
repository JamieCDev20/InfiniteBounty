using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    private Camera cam;

    public void Start()
    {
        cam = Camera.main;
    }

    private void TakePhoto()
    {
        string dir = GetDirectory();
        Debug.Log(dir);
        RenderTexture rTex = new RenderTexture(cam.pixelWidth, cam.pixelHeight, (int)cam.depth);
        cam.targetTexture = rTex;
        cam.Render();
        RenderTexture.active = rTex;
        Texture2D screenshot = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);

        screenshot.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        screenshot.Apply();
        RenderTexture.active = null;
        int fileCount = 0;
        FileInfo[] filesInDir = new DirectoryInfo(dir).GetFiles();
        foreach (FileInfo fi in filesInDir)
            fileCount++;
        File.WriteAllBytes(dir + "/ScreenShot" + fileCount + ".png", screenshot.EncodeToPNG());
    }

    private string GetDirectory()
    {
        if (!Directory.Exists(Application.dataPath + "/ScreenShots"))
            Directory.CreateDirectory(Application.dataPath + "/ScreenShots");
        return Application.dataPath + "/ScreenShots";
    }
}
