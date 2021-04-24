using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPolyhedra : MonoBehaviour
{
    // Start is called before the first frame update
    private float depth = 2;
    void Start()
    {
        Canvas.ForceUpdateCanvases();
        CenterPolyhedraSpace();
    }

    // Update is called once per frame
    void CenterPolyhedraSpace()
    {
        GameObject polyspace = GameObject.Find("PolyhedraSpace");
        Camera camera = GameObject.Find("Camera").GetComponent<Camera>();
        Vector3 screenPos = GetComponent<RectTransform>().position;
        screenPos.z = depth;
        polyspace.transform.position = camera.ScreenToWorldPoint(screenPos);
    }
}
