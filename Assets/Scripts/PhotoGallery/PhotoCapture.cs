using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    private Camera cam;
    private CameraController c_controller;
    [SerializeField] private Texture2D ib_photoStamp;
    [SerializeField, Range(0.0f, 1.0f)] private float f_alpha;

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
        if (!Directory.Exists(Application.dataPath + "/ScreenShots"))
            Directory.CreateDirectory(Application.dataPath + "/ScreenShots");
        return Application.dataPath + "/ScreenShots";
    }

    private Texture2D AlphaBlend(Texture2D _tex_bottom)
    {
        Texture2D tex_combine = new Texture2D(_tex_bottom.width, _tex_bottom.height);
        Color[] botColor = _tex_bottom.GetPixels();
        Debug.Log(string.Format("Screenshot: {0}px x {1}px. btm: {2} | Watermark: {3}px x {4}px, btm: {5}",
            tex_combine.width, tex_combine.height, tex_combine.width * tex_combine.height,
            128, 64, 128 * 64));
        Texture2D tempTex = Instantiate(ib_photoStamp);
        TextureScale.Bilinear(tempTex, 128, 64);
        int mWidth = tex_combine.width;
        int mHeight = tex_combine.height;

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
