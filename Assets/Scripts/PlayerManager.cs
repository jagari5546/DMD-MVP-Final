using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset _inputAction;
    private InputActionMap _playerActionMap;
    private InputAction _moveAction;
    private InputAction _lookAction;

    [Header("Camera / Look")]
    [SerializeField] private Transform _head;
    [SerializeField] private bool _lookInputIsDelta = true;
    [SerializeField] private float _sensX = 0.35f;
    [SerializeField] private float _sensY = 0.35f;
    [SerializeField] private float _stickSensDegPerSec = 120f;
    [SerializeField] private float _minPitch = -89f;
    [SerializeField] private float _maxPitch =  89f;

    [Header("Cinemachine (opcional)")]
    [SerializeField] private CinemachineCamera _cinemachineCam;
    [SerializeField] private bool _disableCinemachineOnStart = true;

    [Header("Movement smoothing")]
    [Range(0f, 0.99f)] [SerializeField] private float _smoothing = 0.25f;
    [SerializeField] private float _accelLerp = 1f;

    private NavMeshAgent _agent;

    private Vector3 _lastDir, _targetDir;
    private float _lerpTime;

    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;

        _playerActionMap = _inputAction.FindActionMap("Player");
        _moveAction = _playerActionMap.FindAction("Move");
        _lookAction = _playerActionMap.FindAction("Look");
        _playerActionMap.Enable();
        _inputAction.Enable();

        if (_disableCinemachineOnStart && _cinemachineCam != null)
            _cinemachineCam.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _yaw = transform.eulerAngles.y;
        if (_head != null) _pitch = _head.localEulerAngles.x;
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Vector2 look = _lookAction.ReadValue<Vector2>();

        if (_lookInputIsDelta)
        {
            _yaw   += look.x * _sensX;
            _pitch -= look.y * _sensY;
        }
        else
        {
            _yaw   += look.x * _stickSensDegPerSec * Time.deltaTime;
            _pitch -= look.y * _stickSensDegPerSec * Time.deltaTime;
        }

        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        if (_head != null) _head.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        Vector2 move = _moveAction.ReadValue<Vector2>();
        Vector3 camFwd = (_head != null ? _head.forward : transform.forward);
        camFwd = Vector3.ProjectOnPlane(camFwd, Vector3.up).normalized;
        Vector3 camRight = Vector3.Cross(Vector3.up, camFwd);

        Vector3 desired = camFwd * move.y + camRight * move.x;
        if (desired.sqrMagnitude > 1f) desired.Normalize();

        if (desired != _lastDir) _lerpTime = 0f;
        _lastDir = desired;

        _targetDir = Vector3.Lerp(_targetDir, desired,
            Mathf.Clamp01(_lerpTime * _accelLerp * (1f - _smoothing)));

        _agent.Move(_targetDir * (_agent.speed * Time.deltaTime));
        _lerpTime += Time.deltaTime;
    }
}
