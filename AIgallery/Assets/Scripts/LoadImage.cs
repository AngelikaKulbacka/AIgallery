using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LoadImage : MonoBehaviour
{
    public Transform InteractionSource;
    public float InteractionRange;
    public int Iterations = 5;
    public GameObject IterationsPanel;
    public Texture2D LoadingTexture;
    public static string CondaPath = null;
    public static bool UseGpu = false;
    private BlankPainting _filePickerPainting = null;
    private string _chosenFile = null;
    private ConcurrentQueue<(GameObject, string)> _generatedImages = new ConcurrentQueue<(GameObject, string)>();

    [SerializeField] private GameObject uiPanel;


    void Update()
    {
        if (_generatedImages.Count > 0)
        {
            Debug.Log($"Generated images is {_generatedImages.Count}");
            if (_generatedImages.TryDequeue(out var element))
            {
                var target = element.Item1;
                var path = element.Item2;
                Debug.Log($"Loading image as material");
                LoadImageAsMaterial(target, path);
                return;
            }
        }

        var ray = new Ray(InteractionSource.position, InteractionSource.forward);
        var hits = Physics.RaycastAll(ray, InteractionRange);
        Debug.Log($"Hit {hits.Count()} elements");

        Debug.DrawRay(InteractionSource.position, InteractionSource.forward, Color.green, 1000);
        uiPanel.SetActive(false);
        foreach (var hitInfo in hits)
        {
            Debug.Log(hitInfo.distance);
            Debug.Log(hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.TryGetComponent<BlankPainting>(out var _))
            {
                uiPanel.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ChooseFile(hitInfo.collider.gameObject);
                    return;
                }
            }
        }
    }

    public void RunNeuralTransferStyle()
    {
        Debug.Log(_filePickerPainting.GetStylePath());
        var absolutePath = Directory.GetParent(Application.dataPath).FullName + "\\";
        Debug.Log(absolutePath + _filePickerPainting.GetStylePath());

        RunNeuralTransferStyle(_filePickerPainting.gameObject, _chosenFile, absolutePath + _filePickerPainting.GetStylePath());
    }

    private void OnFilePicked(string[] filepaths)
    {
        Time.timeScale = 1;
        _chosenFile = filepaths[0];


        var pattern = $@"Assets\\Media\\{_filePickerPainting.DirectoryName}\\.*_?generated_at_iteration_\d+.png$";
        Debug.Log($"Pattern: {pattern}");
        Debug.Log($"Filename: {_chosenFile}");
        Debug.Log($"Regex match: {Regex.Match(_chosenFile, pattern).Success}");
        if (Regex.Match(_chosenFile, pattern).Success) 
        {
            LoadImageAsMaterial(_filePickerPainting.gameObject, _chosenFile);
        }
        else
        {
            IterationsPanel.SetActive(true);
        }
    }

    private void OnCancel()
    {
        Time.timeScale = 1;
    }

    private void ChooseFile(GameObject target)
    {
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _filePickerPainting = target.GetComponent<BlankPainting>();

        FileBrowser.ShowLoadDialog(OnFilePicked, OnCancel, FileBrowser.PickMode.Files);
    }

    private void RunNeuralTransferStyle(GameObject targetObject, string baseImagePath,  string styleImagePath)
    {
        Debug.Log("It's working!");
        string outputImagePath = Path.GetTempFileName() + ".jpg";
        string programPath = null;
        string args = null;
        if (CondaPath is not null)
        {
            programPath = $"{CondaPath}\\_conda.exe";
            string condaArgs = $"run --no-capture-output -p  {CondaPath}  python Assets\\Scripts\\algorithm.py";
            args = $"{condaArgs} {baseImagePath} {styleImagePath} {Iterations} {outputImagePath}";
        }
        else
        {
            programPath = "python";
            args = $"Assets\\Scripts\\algorithm.py {baseImagePath} {styleImagePath} {Iterations} {outputImagePath}";
        }

        Debug.Log($"Running: {programPath} {args}");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = programPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = args,
                CreateNoWindow = true
            },
        };

        Debug.Log("It's working 2!");
        new Thread(() =>
        {
            Debug.Log("It's starting!");
            process.Start();
            while (!process.StandardOutput.EndOfStream || !process.StandardError.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line is not null)
                {
                    Debug.Log(line);
                }
                string errLine = process.StandardError.ReadLine();
                if (errLine is not null)
                {
                    Debug.Log($"Error: {errLine}");
                }
            }
            Debug.Log("It's ending!");
            _generatedImages.Enqueue((targetObject, outputImagePath));

        }).Start();
    }

    public void ShowLoading()
    {
        LoadMaterial(_filePickerPainting.gameObject, LoadingTexture);
    }

    private void LoadImageAsMaterial(GameObject targetObject, string outputImagePath)
    {

        Texture2D texture = LoadTextureFromPath(outputImagePath);
        LoadMaterial(targetObject, texture);
    }

    private void LoadMaterial(GameObject targetObject, Texture2D texture)
    {
        if (texture != null)
        {
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.mainTexture = texture;

            Renderer renderer = targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                var materials = renderer.materials;
                materials[1] = newMaterial;
                renderer.materials = materials;
            }
            else
            {
                Debug.LogError("Target object does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogError("Failed to load image texture");
        }
    }

    private Texture2D LoadTextureFromPath(string imagePath)
    {
        byte[] imageData = System.IO.File.ReadAllBytes(imagePath);

        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            return texture;
        }
        else
        {
            Debug.LogError("Failed to load image texture from file: " + imagePath);
            return null;
        }
    }
}
