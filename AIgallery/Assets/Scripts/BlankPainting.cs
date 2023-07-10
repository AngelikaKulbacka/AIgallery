using System;
using UnityEditor;
using UnityEngine;

public class BlankPainting: MonoBehaviour
{
    public Texture2D StyleTexture;
    public string RelativePath;
    public string DirectoryName;

    public string GetStylePath()
    {
        return RelativePath;
    }
}
