using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private bool doLockCursor = true;
    private CinemachineVirtualCamera virtualCamera;
    private Vector2 targetRotation;
    private Vector2 currentRotation;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        Cursor.lockState = doLockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = Cursor.lockState == CursorLockMode.None;

        targetRotation.y += Input.GetAxis("Mouse X") * mouseSensitivity;
        targetRotation.x += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentRotation = Vector2.Lerp(currentRotation, targetRotation, smoothness * Time.deltaTime);

        transform.eulerAngles = currentRotation;
    }
}
