using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JoinServer_Button : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInput;

    public delegate void OnJoinServer(string ipAddress);

    public static OnJoinServer OnServerJoin;

    public void OnButtonClick()
    {
        OnServerJoin.Invoke(ipInput.text);
    }
}
