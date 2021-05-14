using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SubjectNerd.Utilities;

public class MenuController : MonoBehaviour
{
    private Dropdown waveTypeSelector;
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
        waveTypeSelector = GameObject.Find("WaveTypeSelector").GetComponent<Dropdown>();
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
    public WaveTypeMenuController ActiveMenu()
    {
        return waveTypeMenus[waveTypeSelector.value].waveTypeMenu.GetComponent<WaveTypeMenuController>();
    }
}
