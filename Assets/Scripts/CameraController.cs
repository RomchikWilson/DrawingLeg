using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraTargetSpeed = 10f;
    [SerializeField] private float cameraTargetSpeedRotation = 10f;

    private Transform target = default;

    public static Action<Transform> SetTargetAction = default;

    private void OnEnable()
    {
        SetTargetAction += SetCameraTarget;
    }

    private void OnDisable()
    {
        SetTargetAction -= SetCameraTarget;
    }

    private void Update()
    {
        if (target != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, cameraTargetSpeedRotation * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, target.position, cameraTargetSpeed * Time.deltaTime);
        }
    }

    private void SetCameraTarget(Transform _target)
    {
        target = _target;
    }
}