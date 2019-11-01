using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [Header("Configurable Properties")] public float LookOffset;
    public float CameraAngle;
    public float DefaultZoom;
    public float ZoomMax;
    public float ZoomMin;
    public float RotationSpeed;


    private Camera actualCamera;
    private Vector3 cameraPositionTarget;

    private const float InternalMoveTargetSpeed = 8;
    private const float InternalMoveSpeed = 4;
    private Vector3 moveTarget;
    private Vector3 moveDirection;

    private float currentZoomAmount;

    public float CurrentZoom
    {
        get => currentZoomAmount;
        private set
        {
            currentZoomAmount = value;
            UpdateCameraTarget();
        }
    }

    private float internamZoomSpeed = 4;

    private bool isRightMouseDown = false;
    private const float InternalRotationSpeed = 4;
    private Quaternion rotationTarget;
    private Vector2 mouseDelta;

    private void Start()
    {
        actualCamera = GetComponentInChildren<Camera>();
        actualCamera.transform.rotation = Quaternion.AngleAxis(CameraAngle, Vector3.right);

        cameraPositionTarget = (Vector3.up * LookOffset) +
                               (Quaternion.AngleAxis(CameraAngle, Vector3.right) * Vector3.back) * DefaultZoom;
        CurrentZoom = DefaultZoom;
        actualCamera.transform.position = cameraPositionTarget;
        rotationTarget = transform.rotation;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        print(value);
        moveDirection = new Vector3(value.x, 0, value.y);
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
        {
            return;
        }

        CurrentZoom = Mathf.Clamp(currentZoomAmount - context.ReadValue<Vector2>().y, ZoomMax, ZoomMin);
    }

    public void OnRotateToggle(InputAction.CallbackContext context)
    {
        isRightMouseDown = context.ReadValue<float>() == 1;
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        mouseDelta = isRightMouseDown ? context.ReadValue<Vector2>() : Vector2.zero;
    }

    private void LateUpdate()
    {
        moveTarget += (transform.forward * moveDirection.z + transform.right * moveDirection.x) * Time.fixedDeltaTime *
                      InternalMoveTargetSpeed;
        rotationTarget *= Quaternion.AngleAxis(mouseDelta.x * Time.deltaTime * RotationSpeed, Vector3.up);
        transform.position = Vector3.Lerp(transform.position, moveTarget, Time.deltaTime * InternalMoveSpeed);
        actualCamera.transform.localPosition = Vector3.Lerp(actualCamera.transform.localPosition, cameraPositionTarget,
            Time.deltaTime * internamZoomSpeed);
        transform.rotation =
            Quaternion.Slerp(transform.rotation, rotationTarget, Time.deltaTime * InternalRotationSpeed);
    }

    private void UpdateCameraTarget()
    {
        cameraPositionTarget = (Vector3.up * LookOffset) +
                               Quaternion.AngleAxis(CameraAngle, Vector3.right) * Vector3.back * currentZoomAmount;
    }
}