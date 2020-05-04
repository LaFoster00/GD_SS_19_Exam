using System;
using System.Net.Sockets;
using Player;
using UnityEngine;

namespace InventorySystem
{
    public abstract class Weapon : MonoBehaviour
    {
        public Transform muzzle;
        [SerializeField] private protected float coolDown = 1;
        [SerializeField] private protected AudioSource sound;
        [SerializeField] private protected GameObject bullet;
        [SerializeField] private protected GameObject muzzleFlash;
        [SerializeField] private protected int magazineSize = 10;
        [SerializeField] private protected int bullets;
        [SerializeField] private protected float reloadTime = 1;
        [SerializeField] private protected PlayerControllerTopdown owner;
        public float shakeStrength = 0.5f;
        public float shakeLength = 0.5f;
        private protected float lastShotTime;

        private protected bool _reloading;
        
        public abstract GameObject Pickup();
        public abstract void Drop();
        public abstract bool Use(Vector3 position, Quaternion rotation);
        public abstract void Reload(PlayerControllerTopdown player);
    }
}