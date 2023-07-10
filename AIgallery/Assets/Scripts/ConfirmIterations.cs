using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmIterations : MonoBehaviour
{
    public GameObject panel;
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void Confirm()
    {
        Time.timeScale = 1;
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var loadImage = FindObjectOfType<LoadImage>();
        loadImage.ShowLoading();
        loadImage.RunNeuralTransferStyle();
    }
}
