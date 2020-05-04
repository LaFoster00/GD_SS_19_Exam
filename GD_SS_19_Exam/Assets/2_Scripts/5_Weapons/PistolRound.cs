using System;
using UnityEngine;
using ObjectPooling;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PistolRound : MonoBehaviour, IPooledObject
{
    [SerializeField] private float bulletSpeed = 1;

    [SerializeField] private GameObject impactVFX;

    [SerializeField] private int damage;

    [SerializeField] private ParticleSystem beam;
    [SerializeField] private ParticleSystem bullet;

    private float _spawnTime;

    private Vector3 forward;

    private void FixedUpdate()
    {
        transform.position += forward * bulletSpeed * Time.fixedDeltaTime;
        if (Time.time - _spawnTime > 5)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnObjectSpawn()
    {
        _spawnTime = Time.time;
        forward = transform.forward;
        beam.Clear();
        bullet.Clear();
        beam.Play();
        bullet.Play();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != 9)
        {
            ContactPoint contact = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if (impactVFX != null)
            {
                ObjectPooler.Instance.Spawn(impactVFX, pos, rot);
            }

            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(damage);
            }
            
            ObjectPooler.Instance.Destroy(this.gameObject);
        }
    }
}
