
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public Transform mouthstart;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private bool doLockCursor = true;
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
        if (activeCamera)
        {
            Cursor.lockState = doLockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = Cursor.lockState == CursorLockMode.None;
    
            targetRotation.y += Input.GetAxis("Mouse X") * mouseSensitivity;
            targetRotation.x += -Input.GetAxis("Mouse Y") * mouseSensitivity;
            currentRotation = Vector2.Lerp(currentRotation, targetRotation, smoothness * Time.deltaTime);
    
            transform.eulerAngles = currentRotation;
            prepareCam.transform.eulerAngles = currentRotation;
        }
        else
        {
            
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
    }
    
}
