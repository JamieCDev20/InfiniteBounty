using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private CameraController c_controller;
    [SerializeField] private Texture2D ib_photoStamp;
    [SerializeField, Range(0.0f, 1.0f)] private float f_alpha;
    [SerializeField, Range(0.0f, 1.0f)] private float f_sizeOnScreen;

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
        RenderTexture rTex = new RenderTexture(cam.pixelWidth, cam.pixelHeight, (int)cam.depth);
        cam.targetTexture = rTex;
        cam.Render();
        RenderTexture.active = rTex;
        Texture2D screenshot = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);

        screenshot.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        screenshot.Apply();
        cam.targetTexture = null;
        RenderTexture.active = null;

        screenshot = AlphaBlend(screenshot);
        SavePicture(screenshot);

    }

    private string GetDirectory()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/ScreenShots"))
            Directory.CreateDirectory(Application.persistentDataPath + "/ScreenShots");
        Debug.Log(Application.persistentDataPath + "/ScreenShots");
        return Application.persistentDataPath + "/ScreenShots";
    }

    private Texture2D AlphaBlend(Texture2D _tex_bottom)
    {
        Texture2D tex_combine = new Texture2D(_tex_bottom.width, _tex_bottom.height);
        Color[] botColor = _tex_bottom.GetPixels();
        Texture2D tempTex = Instantiate(ib_photoStamp);

        // The resize zone
        Debug.LogError(string.Format("ScreenWidth: {0} | ScreenHeight: {1}, FullStampWidth: {2} | FullStampHeight: {3}",
            _tex_bottom.width, _tex_bottom.height, tempTex.width, tempTex.height));
        float ratio = tempTex.width / _tex_bottom.width;
        Debug.LogError(ratio.ToString());
        int nW = (int)(ratio * (_tex_bottom.width * f_sizeOnScreen));
        int nH = (int)(ratio * (_tex_bottom.height * f_sizeOnScreen));
        int newX = (int)((nW * ratio));
        int newY = (int)((nH * ratio));
        TextureScale.Bilinear(tempTex, newX, newY);
        int mWidth = tex_combine.width;
        int mHeight = tex_combine.height;
        Debug.LogError(string.Format("StampWidth: {0} | StampHeight: {1}", mWidth, mHeight));

        Color[] stampColor = tempTex.GetPixels();
        for(int i = tempTex.height - 1; i > -1; i--)
        {
            for (int j = tempTex.width - 1; j > -1; j--)
            {
                int stampPos = ((tempTex.width * i) + j);
                int imgPos = mWidth + ((mWidth * i) + j) - 1;
                Color topCol = stampColor[stampPos];
                Color botCol = botColor[imgPos];

                float topAlpha = topCol.a * f_alpha;
                float destAlpha = 1f - topAlpha;
                float alpha = topAlpha + destAlpha * botCol.a;

                Color result = (topCol * topAlpha + botCol * botCol.a * destAlpha) / alpha;
                botColor[imgPos] = result;
            }
        }

        tex_combine.SetPixels(botColor);
        tex_combine.Apply();
        return tex_combine;
    }

    private void SavePicture(Texture2D pic)
    {
        string dir = GetDirectory();
        FileInfo[] filesInDir = new DirectoryInfo(dir).GetFiles();
        int fileCount = 0;
        foreach (FileInfo fi in filesInDir)
            if (fi.Name.Substring(fi.Name.Length - 4) != "meta")
                fileCount++;
        File.WriteAllBytes(dir + "/ScreenShot" + fileCount + ".png", pic.EncodeToPNG());
    }
}
