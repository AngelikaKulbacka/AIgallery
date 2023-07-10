using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using SimpleFileBrowser;
using static SimpleFileBrowser.FileBrowser;

public class RadioButton : MonoBehaviour
{
    ToggleGroup toggleGroup;

    void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }


    public void SelectPath()
    {
        FileBrowser.ShowLoadDialog((path) =>
        {
            LoadImage.CondaPath = path[0];
        }, () => { }, FileBrowser.PickMode.Folders);
    }

    public void SetGpu(bool enabled)
    {
        LoadImage.UseGpu = enabled;
    }
}
