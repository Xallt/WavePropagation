using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SliderFloatSync : MonoBehaviour
{

    private Slider slider;
    private Text text;
    private void OnEnable()
    {
        slider = GetComponentInChildren<Slider>();
        text = transform.Find("Text").GetComponent<Text>();
    }
    private void Start()
    {
        text.text = slider.value.ToString();
        slider.onValueChanged.AddListener(f => text.text = f.ToString("0.0"));
    }
}
