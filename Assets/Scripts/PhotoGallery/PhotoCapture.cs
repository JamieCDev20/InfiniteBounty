using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    private Camera cam;
    private CameraController c_controller;
    [SerializeField] private Texture2D ib_photoStamp;

    public void Start()
    {
        RecieveCamera(Camera.main);
    }

    public void RecieveCamera(Camera c_cam)
    {
        cam = c_cam;
    }

    public void RecieveCameraController(CameraController _c_controller)
    {
        c_controller = _c_controller;
        RecieveCamera(c_controller.GetComponentInChildren<Camera>());
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
        Texture2D screenshot = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);

        screenshot.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        screenshot.Apply();
        cam.targetTexture = null;
        RenderTexture.active = null;
        int fileCount = 0;
        screenshot = AlphaBlend(screenshot);

        FileInfo[] filesInDir = new DirectoryInfo(dir).GetFiles();
        foreach (FileInfo fi in filesInDir)
            if(fi.Name.Substring(fi.Name.Length - 4) != "meta")
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
        Debug.Log(string.Format("Screenshot: {0}px x {1}px.", tex_combine.width, tex_combine.height));
        int botPoint = (_tex_bottom.width / 2) + (_tex_bottom.height / 4);
        int topPoint = ib_photoStamp.width / 2;
        Texture2D tempTex = Instantiate(ib_photoStamp);
        TextureScale.Bilinear(tempTex, 128, 64);

        Color[] stampColor = tempTex.GetPixels();
        for(int i = 0; i < tempTex.width; i++)
        {
            for(int j = 0; j < tempTex.height; j++)
            {
                Color topCol = stampColor[i+j];
                Color botCol = botColor[i+j];

                float topAlpha = topCol.a*0.15f;
                float destAlpha = 1f - topAlpha;
                float alpha = topAlpha + destAlpha * botCol.a;

                Color result = (topCol * topAlpha + botCol * botCol.a * destAlpha) / alpha;
                botColor[(i+j)] = result;
            }
        }

        /*for (int i = 0; i < stampColor.Length; i++)
        {
            int texInd = 0;
            // Alpha blend here
            if (i <= stampColor.Length / 2)
                texInd = botPoint - (topPoint + i);
            else
                texInd = botPoint + (topPoint - i);
            Color topCol = stampColor[i];
            Color botCol = botColor[i];

            float topAlpha = topCol.a;
            float destAlpha = 1f - topAlpha;
            float alpha = topAlpha + destAlpha * botCol.a;

            Color result = (topCol * topAlpha + botCol * botCol.a * destAlpha) / alpha;
            botColor[i] = result;
        }*/
        tex_combine.SetPixels(botColor);
        tex_combine.Apply();
        return tex_combine;
    }
}
