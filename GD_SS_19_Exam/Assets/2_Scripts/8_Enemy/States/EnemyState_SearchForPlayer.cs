using System;
using System.Runtime.InteropServices;
using Player;
using UnityEngine;
public class EnemyState_SearchForPlayer : IState
{
    private readonly PlayerControllerTopdown[] _allPlayers;
    private PlayerControllerTopdown _targetPlayer;
    private readonly Action<Vector3> _setDestination;
    private readonly Action<PlayerControllerTopdown> _playerReached;
    private GameObject enemy;

    private Vector3 _lastPlayerPosition;
    private Vector3[] _playerPositions = new Vector3[2];

    public EnemyState_SearchForPlayer(PlayerControllerTopdown[] players,Action<Vector3> SetDestination, Action<PlayerControllerTopdown> PlayerReached, GameObject Enemy)
    {
        _allPlayers = players;
        _setDestination = SetDestination;
        _playerReached = PlayerReached;
        enemy = Enemy;
    }

    public void Enter()
    {
        FindTarget();
        _lastPlayerPosition = _targetPlayer.transform.position;
        _setDestination(_lastPlayerPosition);
    }

    public void Execute()
    {
        if (_targetPlayer != null)
        {
            float magnitudeOldPos = (_lastPlayerPosition - _targetPlayer.transform.position).magnitude;
            if (magnitudeOldPos > 0.5f)
            {
                FindTarget();
                _lastPlayerPosition = _targetPlayer.transform.position;
                _setDestination(_lastPlayerPosition);
            }
            
            float magnitude = (_targetPlayer.transform.position - enemy.transform.position).magnitude;

            if (magnitude <= 1.5f)
            {
                _playerReached(_targetPlayer);
            }
        }
        else
        {
            FindTarget();
        }
    }

    public void Exit()
    {
        
    }

    private void FindTarget()
    {
        if (!_allPlayers[0].isDead && !_allPlayers[1].isDead)
        {
            if (_allPlayers[1] != null)
            {
                _playerPositions[0] = _allPlayers[0].transform.position;
                _playerPositions[1] = _allPlayers[1].transform.position;

                float[] distances = new float[2];
                for (int i = 0; i < _allPlayers.Length; i++)
                {
                    distances[i] = (enemy.transform.position - _playerPositions[i]).magnitude;
                }

                if (distances[0] > distances[1])
                {
                    _targetPlayer = _allPlayers[1];
                }
                else
                {
                    _targetPlayer = _allPlayers[0];
                }
            }
            else
            {
                _targetPlayer = _allPlayers[0];
            }
        }else if (_allPlayers[0].isDead)
        {
            _targetPlayer = _allPlayers[1];
        }
        else
        {
            _targetPlayer = _allPlayers[0];
        }
    }
}
