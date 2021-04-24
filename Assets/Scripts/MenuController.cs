﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SubjectNerd.Utilities;

public class MenuController : MonoBehaviour
{
    [Serializable]
    public class WaveTypeMenuWithName
    {
        public string name;
        public GameObject waveTypeMenu;
    }
    [Reorderable]
    public WaveTypeMenuWithName[] waveTypeMenus;
    private void Start()
    {
        InitWaveTypeSelector();
    }
    private void InitWaveTypeSelector()
    {
        Dropdown waveTypeSelector = GameObject.Find("WaveTypeSelector").GetComponent<Dropdown>();
        waveTypeSelector.AddOptions(waveTypeMenus.Select(x => x.name).ToList());
        waveTypeSelector.onValueChanged.AddListener(SwitchToWaveTypeMenu);
        SwitchToWaveTypeMenu(waveTypeSelector.value);
    }
    public void SwitchToWaveTypeMenu(int index)
    {
        foreach (var obj in waveTypeMenus)
        {
            obj.waveTypeMenu.SetActive(false);
        }
        waveTypeMenus[index].waveTypeMenu.SetActive(true);
    }
}
