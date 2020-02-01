using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager instance = null;

    public Camera leftEye;
    public Camera rightEye;
    public Camera currentControlledCamera;

    public CameraController leftCameraController;
    public CameraController rightCameraController;
    public CameraController currentCameraController;

    public Tongue tongue;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentCameraController = rightCameraController;
        currentControlledCamera = rightEye;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchCamera();
        }
    }
    
    void SwitchCamera()
    {
        currentCameraController.Deactivate();
        currentCameraController = currentControlledCamera != leftEye ? leftCameraController : rightCameraController;
        currentControlledCamera = currentControlledCamera != leftEye ? leftEye : rightEye;
        currentCameraController.Activate();
        
        tongue.mouthStart = currentCameraController.mouthstart;
        tongue.vcamNormal = currentCameraController.virtualCamera;
        tongue.vcamPreparing = currentCameraController.prepareCam;
    }
}
