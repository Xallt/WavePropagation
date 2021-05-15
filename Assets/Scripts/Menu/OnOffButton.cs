using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffButton: MonoBehaviour
{
    Button onButton, offButton;
    private void Awake()
    {
        onButton = transform.Find("OnButton").GetComponent<Button>();
        offButton = transform.Find("OffButton").GetComponent<Button>();
    }
    private void Switch(bool on = true)
    {
        offButton.gameObject.SetActive(on);
        onButton.gameObject.SetActive(!on);
    }
    private void OnEnable()
    {
        Switch(false);
        onButton.onClick.AddListener(() => Switch(true));
        offButton.onClick.AddListener(() => Switch(false));
    }
}
