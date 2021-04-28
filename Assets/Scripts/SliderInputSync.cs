using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SliderInputSync : MonoBehaviour
{
    private Slider slider;
    private InputField input;
    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        input = GetComponentInChildren<InputField>();
        input.text = slider.value.ToString();
        slider.onValueChanged.AddListener(f => input.text = f.ToString());
        input.onValueChanged.AddListener(s => {
            int f;
            if (s == "")
            {
                f = (int)slider.minValue;
                slider.value = f;
            }
            else
            {
                f = int.Parse(s);
                f = (int)Mathf.Clamp(f, slider.minValue, slider.maxValue);
                input.text = f.ToString();
                slider.value = f;
            }
        });
    }
}
