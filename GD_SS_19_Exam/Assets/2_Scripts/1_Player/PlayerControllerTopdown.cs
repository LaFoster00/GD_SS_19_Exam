using System;
using System.Runtime.InteropServices;
using GameEvents;
using InventorySystem;
using UnityEngine;
using UnityEngine.Networking;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerControllerTopdown : NetworkBehaviour
    {
        [HideInInspector] public bool isDead;
        
        [SerializeField] private int _hp = 100;
        [SerializeField] private GameObject weapon;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private Transform playerObject;

        [SerializeField] private GameObject HUD;

        [Header("PlayerMovementAxis")] [SerializeField]
        private Transform playerMovementAxis;

        [Header("Movement Parameters")] [SerializeField]
        private float speed = 1;

        private Weapon _weapon;
        private Camera _playerCamera;
        private GameObject _playerCameraInstance;

        private void Start()
        {
            GameEventManager.Raise(new GameEvent_PlayerSpawn(this));

            _weapon = weapon.GetComponent<Weapon>();
            //instantiate camera
            if (isLocalPlayer)
            {
                Debug.Log("Instantiating Player Camera!");
                _playerCameraInstance = Instantiate(playerCamera);
                _playerCamera = _playerCameraInstance.GetComponentInChildren<Camera>();
                Transform targetGroup = _playerCameraInstance.transform.Find("Target_Group");
                targetGroup.position = playerObject.position;
                targetGroup.parent = playerObject;
            }
            else
            {
                GameObject.Destroy(HUD);
            }
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                #region WeaponHandling

                if (Input.GetMouseButton(0))
                {
                    UseWeapon();
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    ReloadWeapon();
                }

                #endregion
            }
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                if (!isDead)
                {
                    //update player movement
                    transform.position += GetMovementDirection() * speed * Time.fixedDeltaTime;
                    UpdateOrientation();
                }
            }
        }

        #region ServerSide

        public void ReceiveDamage(int damage)
        {
            RpcReceiveDamage(damage);
            _hp -= damage;
            Mathf.Clamp(_hp, 0, 100);
            GameEventManager.Raise(new GameEvent_OnPlayerReceiveDamage(this, _hp));
            if (_hp <= 0)
            {
                isDead = true;
                GameEventManager.Raise(new GameEvent_OnPlayerDeath(this));
                playerObject.gameObject.SetActive(false);
            }
        }
        
        [ClientRpc]
        private void RpcReceiveDamage(int damage)
        {
            if (!isServer)
            {
                _hp -= damage;
                Mathf.Clamp(_hp, 0, 100);
                GameEventManager.Raise(new GameEvent_OnPlayerReceiveDamage(this, _hp));
                if (_hp <= 0)
                {
                    isDead = true;
                    GameEventManager.Raise(new GameEvent_OnPlayerDeath(this));
                    playerObject.gameObject.SetActive(false);
                }
            }
        }

        [Command]
        private void CmdJoinServer()
        {
            GameEventManager.Raise(new GameEvent_PlayerSpawn(this));
        }

        #endregion

        #region WeaponControll

        #region Reload

        void ReloadWeapon()
        {
            _weapon.Reload(this);
            if (isServer)
            {
                RpcReloadWeapon();
            }
            else
            {
                CmdReloadWeapon();
            }
        }

        #region Network

        [Command]
        public void CmdReloadWeapon()
        {
            //reload weapon in hand d
            _weapon.Reload(this);
        }

        [ClientRpc]
        public void RpcReloadWeapon()
        {
            //reload weapon in hand
            _weapon.Reload(this);
        }

        #endregion

        #endregion

        #region fireWeapon

        void UseWeapon()
        {
            bool hasShot = _weapon.Use(_weapon.muzzle.position, _weapon.muzzle.rotation);
            if (hasShot == true) //its not redundant!
            {
                GameEventManager.Raise(new GameEvent_CameraShake(_weapon.shakeStrength, _weapon.shakeLength));

            }

            if (isServer)
            {
                RpcUseWeapon(_weapon.muzzle.position, _weapon.muzzle.rotation);
            }
            else
            {
                CmdUseWeapon(_weapon.muzzle.position, _weapon.muzzle.rotation);
            }
        }

        #region Network

        [Command]
        public void CmdUseWeapon(Vector3 localMuzzlePos, Quaternion localMuzzleRot)
        {
            //fire weapon in hand
            _weapon.Use(localMuzzlePos, localMuzzleRot);
        }

        [ClientRpc]
        public void RpcUseWeapon(Vector3 localMuzzlePos, Quaternion localMuzzleRot)
        {
            //fire weapon in hand
            _weapon.Use(localMuzzlePos, localMuzzleRot);
        }

        #endregion


        #endregion

        #endregion

        #region PlayerMovement

        private Vector3 GetMovementDirection()
        {
            Vector3 direction = Vector3.zero;

            float movementForward = 0;
            float movementRight = 0;

            if (Input.GetKey(KeyCode.W))
            {
                movementForward++;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movementForward--;
            }

            if (Input.GetKey(KeyCode.D))
            {
                movementRight++;
            }

            if (Input.GetKey(KeyCode.A))
            {
                movementRight--;
            }

            if (movementRight != 0 || movementForward != 0)
            {
                Vector3 forward = _playerCamera.transform.forward;
                forward.y = 0.0f;
                forward.Normalize();
                direction += forward * movementForward;
                direction += _playerCamera.transform.right * movementRight;

                direction = direction.normalized;
            }

            return direction;
        }

        private void UpdateOrientation()
        {
            Ray cameraRay = _playerCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            float rayLenght;

            if (groundPlane.Raycast(cameraRay, out rayLenght))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLenght);

                transform.LookAt(pointToLook);
            }
        }

        #endregion
    }
}