using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform cameraPivot;
    public float playerAngle;//プレイヤーの向き（オイラー角）
    [Header("Z Rotation Camera")]
    [HideInInspector] public float timer;
    [HideInInspector] public int int_timer;
    [HideInInspector] public float zRotation;
    [HideInInspector] public float wantedZ;
    [HideInInspector] public float timeSpeed = 2;

    [HideInInspector] public float timerToRotateZ;

    [Tooltip("Current mouse sensivity, changes in the weapon properties")]
    [HideInInspector]public float mouseSensitvity = 0;
    public float mouseSensitvity_notAiming = 20;
    public float mouseSensitvity_aiming = 5;

    private float rotationYVelocity, cameraXVelocity;
    [Tooltip("Speed that determines how much camera rotation will lag behind mouse movement.")]
    public float yRotationSpeed, xCameraSpeed;

    [HideInInspector]public float wantedYRotation;
    [HideInInspector]public float currentYRotation;
    [HideInInspector]public float wantedCameraXRotation;
    [HideInInspector]public float currentCameraXRotation;

    [Tooltip("Top camera angle.")]
    public float topAngleView = 60;
    [Tooltip("Minimum camera angle.")]
    public float bottomAngleView = -45;

    [HideInInspector] float deltaTime = 0.0f;
    [HideInInspector] float deltaTime2 = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//カーソルロックしない
        //Cursor.visible = false;
        print(Cursor.lockState);
    }

    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;//カーソルロック
    }

    void Update()
    {
        MouseInputMovement();

        if (GetComponent<PlayerMovement>().currentSpeed > 1)
            HeadMovement();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        deltaTime2 += Time.deltaTime;
    }

    void FixedUpdate()
    {
        MouseSensitvityChenge();
        ApplyingStuff();
    }


    void HeadMovement()
    {
        timer += timeSpeed * Time.deltaTime;
        int_timer = Mathf.RoundToInt(timer);
        if (int_timer % 2 == 0)
        {
            wantedZ = -1;
        }
        else
        {
            wantedZ = 1;
        }

        zRotation = Mathf.Lerp(zRotation, wantedZ, Time.deltaTime * timerToRotateZ);
    }


    void MouseInputMovement()
    {

        wantedYRotation += Input.GetAxis("Mouse X") * mouseSensitvity;

        wantedCameraXRotation -= Input.GetAxis("Mouse Y") * mouseSensitvity;

        wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);

    }

    void MouseSensitvityChenge()
    {
        if (Input.GetAxis("Fire2") != 0)
        {
            mouseSensitvity = mouseSensitvity_aiming;
        }
        else if (GetComponent<PlayerMovement>().maxSpeed > 5)
        {
            mouseSensitvity = mouseSensitvity_notAiming;
        }
        else
        {
            mouseSensitvity = mouseSensitvity_notAiming;
        }
    }

    void ApplyingStuff()
    {

        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
        currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

        //WeaponRotation();

        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
        cameraPivot.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);//なんでこれで動くかわからない
        playerAngle = transform.rotation.eulerAngles.y;
        //cameraPivot.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);

    }

}
