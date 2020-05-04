using ExamShooter_Menu;
using Player;
using UnityEngine;

namespace GameEvents
{  

  //---------------------------------------------------------------------------------------------------
  // sample events without additional arguments

  public class GameEvent_Click : GameEvent {}
  public class GameEvent_CancelAction : GameEvent {}
  public class GameEvent_FinishLevel : GameEvent {}
  public class GameEvent_AllPlayersJoined : GameEvent {}

  //---------------------------------------------------------------------------------------------------

  // sample events with additional arguments. Make sure these implement Validate()

  /// <summary>
  /// A list of the possible simplified Game Engine base events
  /// </summary>
  public enum SimpleEventType
  {
    LevelStart,
    LevelComplete,
    LevelEnd,
    Pause,
    UnPause,
    PlayerDeath,
    Respawn,
    StarPicked,
    NewWave
  }
  
  public class SimpleEvent : GameEvent
  {
    public readonly SimpleEventType eventType;
    public SimpleEvent(SimpleEventType t)
    {
      eventType = t;
    }
    
    public override bool isValid() {
      return (eventType != null);
    }
  }
  
  public class GameEvent_OnScoreChange : GameEvent
  {
    public readonly float score;

    public GameEvent_OnScoreChange(float Score)
    {
      score = Score;
    }

    public override bool isValid()
    {
      return score != 0;
    }
  }

  public class GameEvent_OnMenu : GameEvent
  {
    public readonly MenuType menuType;

    public GameEvent_OnMenu(MenuType MenuType)
    {
      menuType = MenuType;
    }

    public override bool isValid()
    {
      return menuType != null;
    }
  }

  public class GameEvent_OnDifficultySelected : GameEvent
  {
    public Difficulty difficulty;

    public GameEvent_OnDifficultySelected(Difficulty Difficulty)
    {
      difficulty = Difficulty;
    }

    public override bool isValid()
    {
      return difficulty != null;
    }
  }

  public class GameEvent_CameraShake : GameEvent
  {
    public readonly float strength;
    public readonly float length;

    public GameEvent_CameraShake(float Strength, float Length)
    {
      strength = Strength;
      length = Length;
    }

    public override bool isValid()
    {
      return strength > 0 && length > 0;
    }
  }

  public class GameEvent_OnEnemyDeath : GameEvent
  {
    public readonly GameObject enemyGO;
    public readonly float points;

    public GameEvent_OnEnemyDeath(GameObject EnemyGO, float Points)
    {
      enemyGO = EnemyGO;
      points = Points;
    }

    public override bool isValid()
    {
      return points > 0;
    }
  }
  
  
  public class GameEvent_OnPlayerDeath : GameEvent
  {
    public readonly PlayerControllerTopdown player;

    public GameEvent_OnPlayerDeath(PlayerControllerTopdown Player)
    {
      player = Player;
    }

    public override bool isValid()
    {
      return player != null;
    }
  }

  public class GameEvent_PlayerSpawn : GameEvent
  {
    public readonly PlayerControllerTopdown player;

    public GameEvent_PlayerSpawn(PlayerControllerTopdown Player)
    {
      player = Player;
    }

    public override bool isValid()
    {
      return player != null;
    }
  }

  public class GameEvent_OnPlayerReceiveDamage : GameEvent
  {
    public readonly PlayerControllerTopdown player;
    public readonly float hp;

    public GameEvent_OnPlayerReceiveDamage(PlayerControllerTopdown Player,float HP)
    {
      player = Player;
      hp = HP;
    }

    public override bool isValid()
    {
      return player != null && hp != 0;
    }
  }
  
  public class GameEvent_OnReload : GameEvent
  {
    public readonly PlayerControllerTopdown player;
    public readonly float reloadTime;

    public GameEvent_OnReload(PlayerControllerTopdown Player,float ReloadTime)
    {
      player = Player;
      reloadTime = ReloadTime;
    }

    public override bool isValid()
    {
      return player != null && reloadTime != 0;
    }
  }
  
  public class GameEvent_OnShotFired : GameEvent
  {
    public readonly PlayerControllerTopdown player;

    public GameEvent_OnShotFired(PlayerControllerTopdown Player)
    {
      player = Player;
    }

    public override bool isValid()
    {
      return player != null;
    }
  }
  
  public class GameEvent_OnWeaponStart : GameEvent
  {
    public readonly PlayerControllerTopdown player;
    public readonly int magazineSize;

    public GameEvent_OnWeaponStart(PlayerControllerTopdown Player, int MagazineSize)
    {
      player = Player;
      magazineSize = MagazineSize;
    }

    public override bool isValid()
    {
      return player != null;
    }
  }
}