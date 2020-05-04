using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Cinemachine;
using GameEvents;
using UnityEngine;

public class EnemyState_Die : IState
{
    private GameObject _enemy;
    private float _points;
    
    public EnemyState_Die(GameObject enemy, float points)
    {
        _enemy = enemy;
        _points = points;
    }
    
    public void Enter()
    {
        GameEventManager.Raise(new GameEvent_OnEnemyDeath(_enemy, _points));
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
}
