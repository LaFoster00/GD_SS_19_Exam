using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vfx_Impact : MonoBehaviour, IPooledObject
{
    [SerializeField] private ParticleSystem beam;
    [SerializeField] private ParticleSystem particles;

    public void OnObjectSpawn()
    {
        beam.Clear();
        particles.Clear();
        beam.Play();
        particles.Play();
    }
}
