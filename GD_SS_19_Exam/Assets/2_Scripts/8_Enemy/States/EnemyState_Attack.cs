using System;
using Player;
using UnityEngine;

public class EnemyState_Attack : IState
{
    private PlayerControllerTopdown _targetPlayer;
    private Action<bool> _playerLost;
    private int _damage;
    private GameObject _enemy;
    private float _lastAttackTime;
    private float _coolDown;

    private Vector3 _lastPlayerPosition;
    private Vector3 _lastOwnPos;

    public EnemyState_Attack(PlayerControllerTopdown TargetPlayer, Action<bool> PlayerLost, GameObject Enemy, int Damage, float CoolDown)
    {
        _targetPlayer = TargetPlayer;
        _playerLost = PlayerLost;
        _enemy = Enemy;
        _damage = Damage;
        _coolDown = CoolDown;

    }

    public void Enter()
    {
        _lastPlayerPosition = _targetPlayer.transform.position;
        _lastOwnPos = _enemy.transform.position;
    }

    public void Execute()
    {
        if (_targetPlayer != null)
        {
            float magnitudeOldPos = (_lastPlayerPosition - _targetPlayer.transform.position).magnitude;
            if (magnitudeOldPos > 1f)
            {
                _playerLost(true);
                return;
            }
        }

        if (Time.time - _lastAttackTime >= _coolDown)
        {
            AttackPlayer(_targetPlayer);
            _lastAttackTime = Time.time;
        }

        if ((_lastOwnPos - _enemy.transform.position).magnitude >= 0.5f)
        {
            _playerLost(true);
        }
    }

    public void Exit()
    {
        
    }
    
    private void AttackPlayer(PlayerControllerTopdown targetPlayer)
    {
        targetPlayer.ReceiveDamage(_damage);
    }
}
