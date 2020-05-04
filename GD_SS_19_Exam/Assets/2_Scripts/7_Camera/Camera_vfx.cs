using System.Collections;
using Cinemachine;
using GameEvents;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class Camera_vfx : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vfxCamera;

    private Vector3 _originalPos;
    private CinemachineTransposer _transposer;
    private IEnumerator _shakeRoutine;

    private void Start()
    {
        _transposer = vfxCamera.GetCinemachineComponent<CinemachineTransposer>();
        _originalPos = _transposer.m_FollowOffset;
    }

    #region Events

    void OnEnable()
    {
        GameEventManager.AddListener<GameEvent_CameraShake>(OnCameraShake);
    }

    void OnDisable()
    {
        GameEventManager.RemoveListener<GameEvent_CameraShake>(OnCameraShake);
    }

    #endregion

    void OnCameraShake(GameEvent_CameraShake cameraShake)
    {

        if (_shakeRoutine != null)
        {
            StopCoroutine(_shakeRoutine);
        }

        _shakeRoutine = Shake(cameraShake.strength, cameraShake.length);
        StartCoroutine(_shakeRoutine);
    }

    IEnumerator Shake(float strength, float length)
    {
        float elapsed = 0.0f;
        
        while (elapsed < length)
        {
            float sF = 1 - (elapsed / length);
            float x = Random.Range(-1f, 1f) * strength * sF;
            float y = Random.Range(-1f, 1f) * strength * sF;
            if (_transposer != null)
            {
                _transposer.m_FollowOffset = _originalPos + new Vector3(x, y, 0);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_transposer != null)
        {
            _transposer.m_FollowOffset = _originalPos;
        }
    }
}