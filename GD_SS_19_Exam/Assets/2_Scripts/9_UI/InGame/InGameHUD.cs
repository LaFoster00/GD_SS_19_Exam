using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameEvents;
using Player;
using TMPro;
using UnityEngine;

public class InGameHUD : MonoBehaviour
{
    [SerializeField] private PlayerControllerTopdown player;
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text amo;

    [SerializeField] private GameObject deathScreen;

    private int _magazineSize;
    private int _bulletsInMagazine;

    private void OnEnable()
    {
        GameEventManager.AddListener<GameEvent_OnPlayerReceiveDamage>(OnPlayerReceiveDamage);
        GameEventManager.AddListener<GameEvent_OnScoreChange>(OnScoreChange);
        GameEventManager.AddListener<GameEvent_OnShotFired>(OnShotFired);
        GameEventManager.AddListener<GameEvent_OnReload>(OnReload);
        GameEventManager.AddListener<GameEvent_OnWeaponStart>(OnWeaponStart);
        GameEventManager.AddListener<GameEvent_OnPlayerDeath>(OnPlayerDeath);
    }

    private void OnDisable()
    {
        GameEventManager.RemoveListener<GameEvent_OnPlayerReceiveDamage>(OnPlayerReceiveDamage);
        GameEventManager.RemoveListener<GameEvent_OnScoreChange>(OnScoreChange);
        GameEventManager.RemoveListener<GameEvent_OnShotFired>(OnShotFired);
        GameEventManager.RemoveListener<GameEvent_OnReload>(OnReload);
        GameEventManager.RemoveListener<GameEvent_OnWeaponStart>(OnWeaponStart);
        GameEventManager.RemoveListener<GameEvent_OnPlayerDeath>(OnPlayerDeath);
    }

    private void OnPlayerReceiveDamage(GameEvent_OnPlayerReceiveDamage playerDamage)
    {
        if (playerDamage.player == player)
        {
            hp.text = "HP: " + playerDamage.hp;
        }
    }

    private void OnScoreChange(GameEvent_OnScoreChange scoreChange)
    {
        score.text = "Score: " + scoreChange.score;
    }

    private void OnShotFired(GameEvent_OnShotFired shotFired)
    {
        if (shotFired.player == player)
        {
            _bulletsInMagazine--;
            amo.text = _bulletsInMagazine.ToString();
        }
    }

    private void OnReload(GameEvent_OnReload reload)
    {
        StartCoroutine(Reload(reload));
    }

    private IEnumerator Reload(GameEvent_OnReload reload)
    {
        yield return new WaitForSeconds(reload.reloadTime);
        _bulletsInMagazine = _magazineSize;
        amo.text = _bulletsInMagazine.ToString();
    }

    private void OnWeaponStart(GameEvent_OnWeaponStart weaponStart)
    {
        _magazineSize = weaponStart.magazineSize;
        _bulletsInMagazine = _magazineSize;
        amo.text = _bulletsInMagazine.ToString();
    }

    private void OnPlayerDeath(GameEvent_OnPlayerDeath onPlayerDeath)
    {
        if (onPlayerDeath.player == player)
        {
            hp.gameObject.SetActive(false);
            amo.gameObject.SetActive(false);
            deathScreen.SetActive(true);
        }
    }
}
