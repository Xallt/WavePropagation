using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubjectNerd.Utilities;
using UnityEngine.UI;
public class MeshSelector : MonoBehaviour
{
 
    [Serializable]
    public class MeshWithName
    {
        public string name;
        public Mesh mesh;
    }
    [Reorderable]
    public MeshWithName[] meshes;
    public void Start()
    {
        Dropdown dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(meshes.Select(p => p.name).ToList());
        Polyhedra poly = GameObject.Find("Polyhedra").GetComponent<Polyhedra>();
        dropdown.onValueChanged.AddListener(value => poly.SetMesh(meshes[value].mesh));
    }

    public Mesh GetSelectedMesh()
    {
        Dropdown dropdown = GetComponent<Dropdown>();
        return meshes[dropdown.value].mesh;
    }
}
