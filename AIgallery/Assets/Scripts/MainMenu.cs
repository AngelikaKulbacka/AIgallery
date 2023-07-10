using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        if (LoadImage.UseGpu && LoadImage.CondaPath is not null)
        {
            SceneManager.LoadScene(1);
        } else if (!LoadImage.UseGpu)
        {
            SceneManager.LoadScene(1);
        } else
        {
            Debug.Log($"Cannot start UseGpu is {LoadImage.UseGpu} and CondaPath is {LoadImage.CondaPath}");
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
