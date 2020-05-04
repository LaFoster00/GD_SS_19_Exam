using System.Collections;
using GameEvents;
using InventorySystem;
using UnityEngine;
using ObjectPooling;
using Player;

public class Rifle : Weapon
{
    private void Start()
    {
        bullets = magazineSize;
        GameEventManager.Raise(new GameEvent_OnWeaponStart(owner, magazineSize));
    }

    public override GameObject Pickup()
    {
        throw new System.NotImplementedException();
    }

    public override void Drop()
    {
        throw new System.NotImplementedException();
    }

    public override bool Use(Vector3 position, Quaternion rotation)
    {
        if (bullets > 0 && !_reloading)
        {
            if (Time.time - lastShotTime >= coolDown)
            {
                ObjectPooler.Instance.Spawn(bullet, position, rotation);
                ObjectPooler.Instance.Spawn(muzzleFlash, position, rotation);
                bullets--;
                GameEventManager.Raise(new GameEvent_OnShotFired(owner));
                lastShotTime = Time.time;
                sound.Play();
                return true;
            }
        }
        return false;
    }

    public override void Reload(PlayerControllerTopdown player)
    {
        StartCoroutine(CoReload(player));
    }

    private IEnumerator CoReload(PlayerControllerTopdown player)
    {
        _reloading = true;
        GameEventManager.Raise(new GameEvent_OnReload(player, reloadTime));
        yield return new WaitForSeconds(reloadTime);
        bullets = magazineSize;
        _reloading = false;
    }
}
