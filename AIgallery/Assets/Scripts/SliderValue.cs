using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValue : MonoBehaviour
{
    private Slider slider;
    private TextMeshProUGUI textComp;

    void Awake()
    {
        slider = GetComponentInParent<Slider>();
        textComp = GetComponentInParent<TextMeshProUGUI>();
    }

    void Start()
    {
        UpdateText(slider.value);
        slider.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(float val)
    {
        textComp.text = slider.value.ToString();
        var loadImage = FindObjectOfType<LoadImage>();
        loadImage.Iterations = (int)val;
    }
}
