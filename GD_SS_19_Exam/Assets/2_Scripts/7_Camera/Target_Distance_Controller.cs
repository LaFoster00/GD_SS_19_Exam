using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Distance_Controller : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float maxDistance = 1;

    private void Update()
    {
        UpdateOrientation();
    }

    private void UpdateOrientation()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.x -= Screen.width / 2;
        screenPos.y -= Screen.height / 2;
        screenPos.x /= Screen.width / 2;
        screenPos.y /= Screen.height / 2;
        Mathf.Clamp(screenPos.y, -1, 1);
        Mathf.Clamp(screenPos.x, -1, 1);

        transform.localPosition = new Vector3(0,0,screenPos.magnitude * maxDistance);
    }
}
