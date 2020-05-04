using System.Collections;
using System.Collections.Generic;
using ExamShooter_Menu;
using GameEvents;
using Player;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2,
    Impossible = 3
}

public class GameManagement : NetworkBehaviour
{
    [HideInInspector] public static GameManagement Instance;

    public Difficulty difficulty;

    [SerializeField] private Score _score;
    [SerializeField] private GameObject enemy;
    [SerializeField] private int firstWaveSize;
    [SerializeField] private int waveIncrement;


    private float _waveBonus;
    private int _enemyWave;
    private PlayerControllerTopdown[] _players = new PlayerControllerTopdown[2];
    private Transform[] _enemySpawnPositions;

    private int enemiesAllive;
    private int _nextWaveSize;

    void OnEnable()
    {
        Instance = this;

        GameEventManager.AddListener<GameEvent_PlayerSpawn>(OnPlayerSpawn);
        GameEventManager.AddListener<GameEvent_OnDifficultySelected>(OnDifficultySelected);
        GameEventManager.AddListener<GameEvent_OnEnemyDeath>(OnEnemyDeath);
        GameEventManager.AddListener<GameEvent_OnPlayerDeath>(OnPlayerDeath);

        GameManagement[] objs = GameObject.FindObjectsOfType<GameManagement>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDisable()
    {
        GameEventManager.RemoveListener<GameEvent_PlayerSpawn>(OnPlayerSpawn);
        GameEventManager.RemoveListener<GameEvent_OnDifficultySelected>(OnDifficultySelected);
        GameEventManager.RemoveListener<GameEvent_OnEnemyDeath>(OnEnemyDeath);
        GameEventManager.RemoveListener<GameEvent_OnPlayerDeath>(OnPlayerDeath);
    }

    private void Start()
    {
        _score.lastScore = 0;
        if (isServer)
        {
            GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawnPosition");
            List<Transform> enemySpawnTrans = new List<Transform>();
            foreach (GameObject e in enemySpawns)
            {
                enemySpawnTrans.Add(e.transform);
            }

            _enemySpawnPositions = enemySpawnTrans.ToArray();
            _nextWaveSize = firstWaveSize;
        }
    }

    private void Update()
    {
        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                foreach (PlayerControllerTopdown p in _players)
                {
                    Debug.Log(p);
                }
            }
        }

        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameEventManager.Raise(new GameEvent_OnMenu(MenuType.Esc));
        }
    }

    #region CharacterHandling

    private void OnPlayerSpawn(GameEvent_PlayerSpawn playerSpawn)
    {
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i] == null)
            {
                _players[i] = playerSpawn.player;
                Debug.Log("A player joined the Server!");
                if (i == 1)
                {
                    Debug.Log("The Second Player Joined!");
                }
                else
                {
                    Debug.Log("Waiting for Second Player To Join!");
                    return;
                }
            }
        }

        if (isServer)
        {
            OpenDifficultyMenu();
        }
    }

    private void OnEnemyDeath(GameEvent_OnEnemyDeath deadEnemy)
    {
        float bonus = _enemyWave * 2;
        Debug.Log("An Enemy Died! You gained " + (deadEnemy.points + _waveBonus) + " points!");
        UpdateScore(deadEnemy.points + _waveBonus);
        CmdDestroyEnemy(deadEnemy.enemyGO);
        enemiesAllive--;
        if (enemiesAllive == 0)
        {
            StartCoroutine(WaveFinished());
        }
    }

    private void OnPlayerDeath(GameEvent_OnPlayerDeath onPlayerDeath)
    {
        if (_players[0].isDead && _players[1].isDead)
        {
            SceneManager.LoadScene(2);
        }
    }

    #endregion

    #region Match

    private void OpenDifficultyMenu()
    {
        //inform client that the menu has to be opened
        GameEventManager.Raise(new GameEvent_OnMenu(MenuType.DifficultyServer));
        RpcDifficultyMenu();
    }

    private void OnDifficultySelected(GameEvent_OnDifficultySelected difficultySelected)
    {
        //inform the client that the difficulty is selected
        difficulty = difficultySelected.difficulty;
        GameEventManager.Raise(new GameEvent_OnMenu(MenuType.DifficultyServer));
        RpcDifficultyMenu();
        RpcStartMatch(difficultySelected.difficulty);
        StartMatch(difficultySelected.difficulty);
    }

    private void StartMatch(Difficulty difficulty)
    {
        StartCoroutine(SpawnWave(_nextWaveSize));
    }

    private void UpdateScore(float points)
    {
        RpcUpdateScore(points);
        _score.lastScore += points;
        GameEventManager.Raise(new GameEvent_OnScoreChange(_score.lastScore));
        if (points > _score.HighScore)
        {
            _score.HighScore = _score.lastScore;
        }
    }

    #endregion

    #region EnemyWaves

    private IEnumerator SpawnWave(int waveSize)
    {
        enemiesAllive = waveSize;
        _enemyWave++;
        _waveBonus = Mathf.Pow(_enemyWave, 3);
        //spawn enemy wave on all clients
        int spawnedEnemies = 0;
        while (spawnedEnemies < waveSize)
        {
            int spawnPos = UnityEngine.Random.Range(0, 3);
            CmdSpawnEnemy(_enemySpawnPositions[spawnPos].position);
            spawnedEnemies++;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.7f));
        }

        _nextWaveSize += waveIncrement;

        Debug.Log("All enemies Spawned!");
    }

    private IEnumerator WaveFinished()
    {
        UpdateScore(_waveBonus * ((int) difficulty + 1));
        Debug.Log("Next wave starts in 5 Seconds!");
        yield return new WaitForSeconds(5);
        GameEventManager.Raise(new SimpleEvent(SimpleEventType.NewWave));
        StartCoroutine(SpawnWave(_nextWaveSize));
    }

    #endregion

    #region NetworkFunctions

    [ClientRpc]
    private void RpcDifficultyMenu()
    {
        if (!isServer)
        {
            GameEventManager.Raise(new GameEvent_OnMenu(MenuType.DifficultyClient));
        }
    }

    [ClientRpc]
    private void RpcStartMatch(Difficulty difficulty)
    {

    }

    [ClientRpc]
    private void RpcUpdateScore(float points)
    {
        if (!isServer)
        {
            _score.lastScore += points;
            Debug.Log("Score is: " + _score.lastScore);
            GameEventManager.Raise(new GameEvent_OnScoreChange(_score.lastScore));
            if (points > _score.HighScore)
            {
                _score.HighScore = _score.lastScore;
            }
        }
    }
    
    [ClientRpc]
    private void RpcOnPlayerDeath()
    {
        if (_players[0].isDead && _players[1].isDead)
        {
            SceneManager.LoadScene(2);
        }
    }

    [Command]
    void CmdSpawnEnemy(Vector3 position)
    {
        GameObject enemyGo = Instantiate(this.enemy, position, Quaternion.Euler(0, 0, 0));
        Enemy e = enemyGo.GetComponent<Enemy>();
        e.EnemyInit(_players);
        NetworkServer.Spawn(enemyGo);
    }

    [Command]
    void CmdDestroyEnemy(GameObject enemy)
    {
        NetworkServer.Destroy(enemy);
    }

    #endregion
}