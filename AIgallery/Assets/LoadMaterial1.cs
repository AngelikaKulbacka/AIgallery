using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class LoadMaterial1 : MonoBehaviour
{
    public RawImage outputImage;
    public Texture2D baseImage;
    public Texture2D styleReferenceImage;
    public string resultPrefix;

    // Start is called before the first frame update
    void Start()    
    {
        // Przetwarzanie stylu przy uruchomieniu
        ProcessStyleTransfer();
    }

    public void ProcessStyleTransfer()
    {
        // Konwertowanie tekstur na pliki tymczasowe
        string baseImagePath = SaveTextureToTempFile(baseImage);
        string styleReferenceImagePath = SaveTextureToTempFile(styleReferenceImage);

        // Wywo³anie skryptu Pythona do przetwarzania stylu
        string scriptPath = Application.dataPath + "Assets/Scripts/algorithm.py";
        string arguments = $"{scriptPath} {baseImagePath} {styleReferenceImagePath} {resultPrefix}";
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = arguments,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            UseShellExecute = false
        };

        // Uruchomienie procesu Pythona
        Process process = new Process();
        process.StartInfo = startInfo;
        process.OutputDataReceived += (sender, e) => Debug.Log(e.Data);
        process.Start();

        // Odczytanie wygenerowanego obrazu i ustawienie go jako tekstury wynikowej
        string resultImagePath = Application.dataPath + "Assets/Scripts/Image.png";
        StartCoroutine(LoadAndSetOutputImage(resultImagePath));
    }

    private string SaveTextureToTempFile(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Application.temporaryCachePath + "/temp_image.png";
        System.IO.File.WriteAllBytes(filePath, bytes);
        return filePath;
    }

    private IEnumerator LoadAndSetOutputImage(string imagePath)
    {
        WWW www = new WWW("file://" + imagePath);
        yield return www;

        outputImage.texture = www.texture;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
