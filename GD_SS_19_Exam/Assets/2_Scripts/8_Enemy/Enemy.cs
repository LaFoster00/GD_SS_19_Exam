using System;
using GameEvents;
using StateSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using Player;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NetworkTransform))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] private int hp = 100;
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private float coolDown = 1;
    [SerializeField] private float basePoints = 1;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody _rigidbody;
    
    private PlayerControllerTopdown[] _players;
    private StateMachine _stateMachine;
    private Difficulty _difficulty;

    private IState searching;

    #region Init and Constructor

    private void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        if (isServer)
        {
            _stateMachine = new StateMachine();
            _stateMachine.ChangeState(new EnemyState_SearchForPlayer(_players, SetDestination, PlayerReached, this.gameObject));
        }
    }

    public void EnemyInit(PlayerControllerTopdown[] Players)
    {
        _players = Players;
        searching = new EnemyState_SearchForPlayer(_players, SetDestination, PlayerReached, this.gameObject);
        _difficulty = GameManagement.Instance.difficulty;

        switch (_difficulty)
        {
            case Difficulty.Easy:
                //base damage stays the way it is
                break;
            case Difficulty.Normal:
                baseDamage *= 2;
                basePoints *= 2;
                break;
            case Difficulty.Hard:
                baseDamage *= 4;
                basePoints *= 6;
                break;
            case Difficulty.Impossible:
                baseDamage *= 8;
                basePoints *= 10;
                break;
        }
    }

    #endregion

    #region OnlyServerside
    
    private void PlayerReached(PlayerControllerTopdown targetPlayer)
    {
        agent.isStopped = true;

        _stateMachine.ChangeState(new EnemyState_Attack(targetPlayer, PlayerLost, this.gameObject, baseDamage, coolDown));
    }

    private void PlayerLost(bool x)
    {
        agent.isStopped = false;
        _stateMachine.ChangeState(new EnemyState_SearchForPlayer(_players, SetDestination, PlayerReached, this.gameObject));
    }

    private void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    private void OnPlayerDeath(GameEvent_OnPlayerDeath onPlayer)
    {
        agent.isStopped = false;
        _stateMachine.ChangeState(new EnemyState_SearchForPlayer(_players, SetDestination, PlayerReached, this.gameObject));
    }

    private void Update()
    {
        if (isServer)
        {
            _stateMachine.ExecuteState();
        }
    }

    public void ReceiveDamage(int weaponDamage)
    {
        if (isServer)
        {
            //RpcReceiveDamage(weaponDamage);
            hp -= weaponDamage;
            if (hp <= 0)
            {
                Vector3 lastPos = transform.position;

                agent.isStopped = true;
                
                _rigidbody.isKinematic = true;
                transform.position = lastPos;
                
                _stateMachine.ChangeState(new EnemyState_Die(gameObject, basePoints));
            }
        }
    }
    
    #endregion
}
