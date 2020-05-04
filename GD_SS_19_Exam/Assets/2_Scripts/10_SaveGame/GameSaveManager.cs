using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager instance;

    [SerializeField]
    private Score score;

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
        
    }

    public bool isSaveFile()
    {
        return Directory.Exists(Application.persistentDataPath + "/game_save");
    }

    public void SaveGame()
    {
        if (!isSaveFile())
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/game_save");
        }

        if (!Directory.Exists(Application.persistentDataPath + "/game_save/score_data"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/game_save/score_data");
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game_save/score_data/score_save.txt");
        string json = JsonUtility.ToJson(score);
        bf.Serialize(file, json);
        file.Close();
    }

    public void LoadGame()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/game_save/score_data"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/game_save/score_data");
        }
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "/game_save/score_data/score_save.txt"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/game_save/score_data/score_save.txt",
                FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), score);
            file.Close();
        }
    }
}
