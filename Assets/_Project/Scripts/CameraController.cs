
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public Transform mouthstart;
    public bool isRightEye;
    public Vector2 xConstraint = new Vector2(-60, 60);
    public Vector2 yConstraint = new Vector2(-60, 60);
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private bool doLockCursor = true;
    public Canvas reticleCanvas;
    public bool activeCamera = false;
    public CinemachineVirtualCamera prepareCam; public CinemachineVirtualCamera steadyCam;
    public CinemachineVirtualCamera virtualCamera;
    private Vector2 targetRotation;
    private Vector2 currentRotation;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (!activeCamera) return;

        Cursor.lockState = doLockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = Cursor.lockState == CursorLockMode.None;

        targetRotation.y += Input.GetAxis("Mouse X") * mouseSensitivity;
        targetRotation.y = Mathf.Clamp(targetRotation.y, yConstraint.x, yConstraint.y);
        targetRotation.x += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        targetRotation.x = Mathf.Clamp(targetRotation.x, xConstraint.x, xConstraint.y);
        currentRotation = Vector2.Lerp(currentRotation, targetRotation, smoothness * Time.deltaTime);

        transform.eulerAngles = currentRotation;
        prepareCam.transform.eulerAngles = currentRotation;
        if (isRightEye)
        {
            if (targetRotation.y <= yConstraint.x)
            {
                CameraManager.instance.SwitchCamera();
            }
        }
        else
        {
            if (targetRotation.y >= yConstraint.y)
            {
                CameraManager.instance.SwitchCamera();
            }
        }

    }

    public void Activate()
    {
        activeCamera = true;
        prepareCam.enabled = false;
        virtualCamera.enabled = true;
        steadyCam.enabled = false;
    }

    public void Deactivate()
    {
        activeCamera = false;
        prepareCam.enabled = false;
        virtualCamera.enabled = false;
        steadyCam.enabled = true;
        reticleCanvas.enabled = false;
    }

}
