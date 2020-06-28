using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region inspector properties    

    [Header("Target,CameraPosition")]
    public Transform target;//カメラターゲット
    public float distance = 2.5f;// カメラ距離
    public float height = 1.4f;//カメラ高さ
    public float rightOffset = 0f;//カメラ右位置補正

    [Header("AngleLimit")]
    [Tooltip("Top camera angle.")]
    public float topAngleView = 60;
    [Tooltip("Minimum camera angle.")]
    public float bottomAngleView = -45;

    [Header("CameraSensitivity")]
    public float xMouseSensitivity = 3f;//x軸マウス感度
    public float yMouseSensitivity = 3f;//y軸マウス感度

    [Header("CameraFollowTime")]
    [Tooltip("Speed that determines how much camera rotation will lag behind mouse movement.")]
    public float yCameraSpeed;//カメラ追従速度(Y軸)
    public float xCameraSpeed;//カメラ追従速度(X軸)

    #endregion

    #region hide properties    

    private float rotationYVelocity, cameraXVelocity;

    private float wantedCameraYRotation;
    private float currentYRotation;
    private float wantedCameraXRotation;
    private float currentCameraXRotation;

    private Transform targetPos;//カメラ基準点
    private Transform targetLookAt;//カメラ注視点
    private Camera _camera;//カメラコンポーネント取得

    private float forward = -1f;

    #endregion

    void Start()
    {
        if (target == null)//target nullなら処理終了
            return;

        Cursor.lockState = CursorLockMode.Locked;//カーソルロック

        _camera = GetComponent<Camera>();

        targetPos = new GameObject("targetPos").transform;//カメラ基準点オブジェクト生成
        targetLookAt = new GameObject("targetLookAt").transform;//カメラ注視点オブジェクト生成

        targetPos.rotation = target.rotation;//カメラ基準点回転初期化
    }

    void FixedUpdate()
    {
        CameraMovement();
    }

    void CameraMovement()
    {
        if (target == null)//target nullなら処理終了
            return;

        targetPos.position = target.position + new Vector3(0, height, 0);

        targetLookAt.parent = targetPos;

        targetLookAt.localPosition = new Vector3(rightOffset, 0, 0);

        wantedCameraYRotation += Input.GetAxis("Mouse X") * xMouseSensitivity;
        wantedCameraXRotation -= Input.GetAxis("Mouse Y") * yMouseSensitivity;

        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedCameraYRotation, ref rotationYVelocity, yCameraSpeed);
        currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

        wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);

        targetPos.rotation = Quaternion.Euler(currentCameraXRotation, currentYRotation,0);

        var camDir = (forward * targetLookAt.forward);
        camDir = camDir.normalized;

        transform.position = targetLookAt.position + (camDir * (distance));

        var camRotation = Quaternion.LookRotation(targetLookAt.position - transform.position);

        transform.rotation = camRotation;
    }
}
