using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private Texture2D ib_photoStamp;

    public void Start()
    {
        RecieveCamera(Camera.main);
    }

    public void RecieveCamera(Camera c_cam)
    {
        cam = c_cam;
    }

    public void RecieveInputs(bool _b_button)
    {
        if (_b_button)
            TakePhoto();
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

    private Texture2D AlphaBlend(Texture2D _tex_bottom)
    {
        Texture2D tex_combine = new Texture2D(_tex_bottom.width, _tex_bottom.height);
        Color[] botColor = _tex_bottom.GetPixels();
        Color[] stampColor = ib_photoStamp.GetPixels();
        for (int i = 0; i < stampColor.Length; i++)
        {
            // Alpha blend here
            float topAlpha = stampColor[i].a;
            float destAlpha = 1f - topAlpha;
            float alpha = topAlpha + destAlpha + botColor[botColor.Length / 2 - i].a;

        }
        return tex_combine;
    }
}
