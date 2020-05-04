using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Menu : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Score score;
    [SerializeField] private TMP_Text highScore;

    private void Start()
    {
        GameSaveManager.instance.LoadGame();
        
        if (networkManager == null)
        {
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        }

        highScore.text = " HighScore: " + score.HighScore;
    }
    
    private void OnEnable()
    {
        JoinServer_Button.OnServerJoin += OnJoinServer;
    }
    
    private void OnDisable()
    {
        JoinServer_Button.OnServerJoin -= OnJoinServer;
    }

    private void OnJoinServer(string IP_Address)
    {
        networkManager.networkAddress = IP_Address;
        networkManager.StartClient();
    }

    public void OnHostServer()
    {
        networkManager.StartHost();
    }
    
    public void OnQuitGame()
    {
        Application.Quit();
    }
}
