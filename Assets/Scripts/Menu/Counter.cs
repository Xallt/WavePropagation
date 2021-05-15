using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    Text text;
    private void Start()
    {
        text = GetComponent<Text>();
    }
    public void Increment()
    {
        int cur = int.Parse(text.text);
        text.text = (cur + 1).ToString();
    }
}
