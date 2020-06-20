using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
	public enum GunStyles
	{
		automatic,nonautomatic
	}

	AudioSource audioSource1;

	public GunStyles currentStyle;
	[HideInInspector]public MouseLook mouseLook;
	[HideInInspector]public PlayerMovement playerMovement;

	public Transform gunPivot;
	public Transform cameraUnit;
	[HideInInspector] public Transform cameraPivot_gc;
	[HideInInspector] public Image reticle;

	//private Camera secondCamera;
	private Transform player;
	//private Camera cameraComponent;
	private Transform gunPlaceHolder;

	[Header("Bullet properties")]
	public float bulletsIHave = 120;//弾所持数
	public float bulletsInTheGun = 30;//マガジン残弾
	public float amountOfBulletsPerLoad = 30;//リロード

	[HideInInspector]public Vector3 currentGunPosition;
	[Header("Gun Positioning")]
	public Vector3 restPlacePosition;
	public Vector3 aimPlacePosition;
	[Header("Camera Positioning")]
	public Vector3 restCameraPosition;
	public Vector3 aimCameraPosition;
	public float gunAimTime = 0.1f;

	[HideInInspector]
	public bool reloading;

	public Text bulletLabel;

	private Vector2 gunFollowTimeVelocity;

	[Header("Sensitvity of the gun")]
	public float mouseSensitvity_notAiming = 10;
	public float mouseSensitvity_aiming = 5;
	public float mouseSensitvity_running = 4;

	[HideInInspector]
	public bool aiming;

	private Vector3 velV;


	void Awake()
	{
		mouseLook = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLook>();
		playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
		reticle = GameObject.Find("Reticle").GetComponent<Image>();
		player = mouseLook.transform;
		cameraPivot_gc = mouseLook.cameraPivot;
		//gunPivot = mouseLook.cameraPivot;
		//cameraComponent = GameObject.FindGameObjectWithTag("gunPivot").GetComponent<Camera>();//なぜか上手く取得できない
		//cameraComponent = gunPivot.GetComponent<Camera>();//カメラピボット設定するとカメラコンポネント取得できない
		//secondCamera = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();

		//hitMarker = transform.Find("hitMarkerSound").GetComponent<AudioSource>();

		//restPlacePosition = gunPivot.transform.localPosition;

		rotationLastY = mouseLook.currentYRotation;
		rotationLastX = mouseLook.currentCameraXRotation;

		bulletLabel.text = "残弾：" + bulletsIHave + "/" + bulletsInTheGun;

		audioSource1 = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
    {
		GiveCameraScriptMySensitvity();

		

		Shooting();

		bulletLabel.text = "残弾：" + bulletsIHave + "/" + bulletsInTheGun;
	}

	void FixedUpdate()
	{
		//RotationGun();

		GunPosition();

		GunPositionUpdate();//エラー出たのでupdateから移動
	}


	void GiveCameraScriptMySensitvity()
	{
		mouseLook.mouseSensitvity_notAiming = mouseSensitvity_notAiming;
		mouseLook.mouseSensitvity_aiming = mouseSensitvity_aiming;
	}


	private Vector3 gunPosVelocity;
	private Vector3 cameraPivotPosVelocity;
	private Vector3 cameraPosVelocity;
	private float cameraZoomVelocity;
	private float secondCameraZoomVelocity;

	public void GunPosition()
	{
		//if aiming
		if (Input.GetAxis("Fire2") != 0 && !reloading)
		{
			gunPrecision = gunPrecision_aiming;
			recoilAmount_x = recoilAmount_x_;
			recoilAmount_y = recoilAmount_y_;
			recoilAmount_z = recoilAmount_z_;
			cameraUnit.transform.localPosition = Vector3.SmoothDamp(cameraUnit.transform.localPosition, aimCameraPosition, ref cameraPosVelocity, gunAimTime);
			cameraPivot_gc.transform.localPosition = Vector3.SmoothDamp(cameraPivot_gc.transform.localPosition, aimPlacePosition, ref cameraPivotPosVelocity, gunAimTime);
			reticle.enabled = false;
			//cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
			//secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
		}
		//if not aiming
		else
		{
			gunPrecision = gunPrecision_notAiming;
			recoilAmount_x = recoilAmount_x_non;
			recoilAmount_y = recoilAmount_y_non;
			recoilAmount_z = recoilAmount_z_non;
			cameraUnit.transform.localPosition = Vector3.SmoothDamp(cameraUnit.transform.localPosition, restCameraPosition , ref cameraPosVelocity, gunAimTime);
			cameraPivot_gc.transform.localPosition = Vector3.SmoothDamp(cameraPivot_gc.transform.localPosition, restPlacePosition, ref cameraPivotPosVelocity, gunAimTime);
			reticle.enabled = true;
			//cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_notAiming, ref cameraZoomVelocity, gunAimTime);
			//secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_notAiming, ref secondCameraZoomVelocity, gunAimTime);
		}
	}

	void GunPositionUpdate()
	{

		//transform.localPosition = Vector3.SmoothDamp(transform.localPosition,
		//(transform.right * (currentGunPosition.x  + currentRecoilXPos  )) +
		//(transform.up *(currentGunPosition.y  + currentRecoilYPos  )) +
		//(transform.forward * (currentGunPosition.z  + currentRecoilZPos  )), ref velV, 0);

		gunPivot.transform.localPosition = Vector3.SmoothDamp(gunPivot.transform.localPosition,
		(gunPivot.transform.right * (currentGunPosition.x  + currentRecoilXPos  )) +
		(gunPivot.transform.up *(currentGunPosition.y  + currentRecoilYPos  )) +
		(gunPivot.transform.forward * (currentGunPosition.z  + currentRecoilZPos  )), ref velV, 0);


		//playerMovement.cameraPosition = new Vector3(currentRecoilXPos, currentRecoilYPos, 0);

		currentRecoilZPos = Mathf.SmoothDamp(currentRecoilZPos, 0, ref velocity_z_recoil, recoilOverTime_z);
		currentRecoilXPos = Mathf.SmoothDamp(currentRecoilXPos, 0, ref velocity_x_recoil, recoilOverTime_x);
		currentRecoilYPos = Mathf.SmoothDamp(currentRecoilYPos, 0, ref velocity_y_recoil, recoilOverTime_y);
	}


	[Header("Rotation")]
	[Tooltip("The time waepon will lag behind the camera view best set to '0'.")]
	public float rotationLagTime = 0f;
	[Tooltip("Value of forward rotation multiplier.")]
	public Vector2 forwardRotationAmount = Vector2.one;

	private Vector2 velocityGunRotate;
	private float gunWeightX, gunWeightY;
	private float rotationLastY;
	private float rotationDeltaY;
	private float angularVelocityY;
	private float rotationLastX;
	private float rotationDeltaX;
	private float angularVelocityX;


	void RotationGun()
	{
		//rotationDeltaY = mouseLook.currentYRotation - rotationLastY;
		//rotationDeltaX = mouseLook.currentCameraXRotation - rotationLastX;

		//rotationLastY= mouseLook.currentYRotation;
		//rotationLastX= mouseLook.currentCameraXRotation;

		//angularVelocityY = Mathf.Lerp (angularVelocityY, rotationDeltaY, Time.deltaTime * 5);
		//angularVelocityX = Mathf.Lerp (angularVelocityX, rotationDeltaX, Time.deltaTime * 5);

		//gunWeightX = Mathf.SmoothDamp (gunWeightX, mouseLook.currentCameraXRotation, ref velocityGunRotate.x, rotationLagTime);
		//gunWeightY = Mathf.SmoothDamp (gunWeightY, mouseLook.currentYRotation, ref velocityGunRotate.y, rotationLagTime);

		//transform.rotation = Quaternion.Euler (gunWeightX + (angularVelocityX*forwardRotationAmount.x), gunWeightY + (angularVelocityY*forwardRotationAmount.y), 0);
	}

	private float currentRecoilZPos;
	private float currentRecoilXPos;
	private float currentRecoilYPos;

	public void RecoilMath()
	{
		currentRecoilZPos -= recoilAmount_z;
		currentRecoilXPos -= (Random.value - 0.5f) * recoilAmount_x;
		currentRecoilYPos -= (Random.value - 0.5f) * recoilAmount_y;
		mouseLook.wantedCameraXRotation -= Mathf.Abs(currentRecoilYPos * gunPrecision);//リコイル（縦）適用
		mouseLook.wantedYRotation -= (currentRecoilXPos * gunPrecision);//リコイル（横）適用
	}

	[Header("Shooting setup - MustDo")]
	public GameObject bulletSpawnPlace;
	public GameObject bulletPrefab;
	public float roundsPerSecond = 10;//1秒当たり連射数
	public float shotSpeed = 5000;
	private float waitTillNextFire;

	void Shooting()
	{
		if (currentStyle == GunStyles.nonautomatic)
		{
			if (Input.GetButtonDown("Fire1"))
				ShootMethod();
		}

		if (currentStyle == GunStyles.automatic)
		{
			if (Input.GetButton("Fire1"))
				ShootMethod();
		}

		if (Input.GetKeyDown(KeyCode.R) && playerMovement.maxSpeed < playerMovement.runningSpeed && !reloading)
			StartCoroutine("Reload_Animation");

		waitTillNextFire -= roundsPerSecond * Time.deltaTime;
	}


	[Header("Recoil Not Aiming")]
	public float recoilAmount_z_non = 0.05f;//反動（奥）
	public float recoilAmount_x_non = 0.02f;//反動（横）
	public float recoilAmount_y_non = 0.02f;//反動（縦）

	[Header("Recoil Aiming")]
	public float recoilAmount_z_ = 0.03f;
	public float recoilAmount_x_ = 0.005f;
	public float recoilAmount_y_ = 0.005f;

	[HideInInspector]	public float recoilAmount_z = 0.5f;
	[HideInInspector]	public float recoilAmount_x = 0.5f;
	[HideInInspector]	public float recoilAmount_y = 0.5f;

	[HideInInspector]public float velocity_z_recoil,velocity_x_recoil,velocity_y_recoil;

	[Header("Recoil Time")]//リコイル復帰時間
	public float recoilOverTime_z = 0.1f;
	public float recoilOverTime_x = 0.1f;
	public float recoilOverTime_y = 0.1f;

	[Header("Gun Precision")]//射撃制度
	public float gunPrecision_notAiming = 300.0f;
	public float gunPrecision_aiming = 100.0f;

	[Header("Camera ZoomRatio")]//カメラ倍率
	public float cameraZoomRatio_notAiming = 60;
	public float cameraZoomRatio_aiming = 40;

	public float secondCameraZoomRatio_notAiming = 60;
	public float secondCameraZoomRatio_aiming = 40;

	[HideInInspector]public float gunPrecision;

	[Header("Sound Source")]//射撃音,着弾音
	public AudioClip shoot_sound, reloadSound;
	public static AudioSource hitMarker;

	[Header("Muzzel Flashes")]
	public GameObject[] muzzelFlash;

	private GameObject holdFlash;
	private GameObject holdSmoke;


	public static void HitMarkerSound()
	{
		hitMarker.Play();
	}


	private void ShootMethod(){
		if(waitTillNextFire <= 0 && !reloading && playerMovement.maxSpeed < playerMovement.runningSpeed)
		{
			if(bulletsInTheGun > 0)
			{
				int randomNumberForMuzzelFlash = Random.Range(0,5);

				if (bulletPrefab)
				{
					GameObject bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawnPlace.transform.position, Quaternion.identity);

					Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

					bulletRigidbody.AddForce(transform.forward * shotSpeed);

					Destroy(bullet, 3.0f);
				}
				else
					print("Missing the bullet prefab");

					holdFlash = Instantiate(muzzelFlash[randomNumberForMuzzelFlash], bulletSpawnPlace.transform.position /*- muzzelPosition*/, bulletSpawnPlace.transform.rotation * Quaternion.Euler(0,0,90) ) as GameObject;
					holdFlash.transform.parent = bulletSpawnPlace.transform;
					Destroy(holdFlash, 0.05f);

				if (shoot_sound)
					AudioSource.PlayClipAtPoint(shoot_sound, bulletSpawnPlace.transform.position);
					//audioSource1.PlayOneShot(shoot_sound);
				else
					print ("Missing 'Shoot Sound'.");

				RecoilMath();

				waitTillNextFire = 1;
				bulletsInTheGun -= 1;
			}
			else
			{
				StartCoroutine("Reload_Animation");
			}
		}
	}


	[Header("ReloadTime")]
	[Tooltip("Time that passes after reloading. Depends on your reload animation length, because reloading can be interrupted via meele attack or running. So any action before this finishes will interrupt reloading.")]
	public float reloadChangeBulletsTime;


	IEnumerator Reload_Animation(){
		if(bulletsIHave > 0 && bulletsInTheGun < amountOfBulletsPerLoad && !reloading/* && !aiming*/)
		{
			if (reloadSound)
				AudioSource.PlayClipAtPoint(reloadSound, transform.position);
			else
				print ("'Reload Sound Source' missing.");

			//handsAnimator.SetBool("reloading",true);//アニメータ用Bool値
			reloading = true;

			yield return new WaitForSeconds(0.5f);

			//handsAnimator.SetBool("reloading",false);//アニメータ用Bool値
			reloading = false;

			yield return new WaitForSeconds (reloadChangeBulletsTime - 0.5f);

			if (playerMovement.maxSpeed != playerMovement.runningSpeed)
			{
				if (bulletsIHave - amountOfBulletsPerLoad >= 0) 
				{
					bulletsIHave -= amountOfBulletsPerLoad - bulletsInTheGun;
					bulletsInTheGun = amountOfBulletsPerLoad;
				}
				else if (bulletsIHave - amountOfBulletsPerLoad < 0)
				{
					float valueForBoth = amountOfBulletsPerLoad - bulletsInTheGun;
					if (bulletsIHave - valueForBoth < 0)
					{
						bulletsInTheGun += bulletsIHave;
						bulletsIHave = 0;
					}
					else
					{
						bulletsIHave -= valueForBoth;
						bulletsInTheGun += valueForBoth;
					}
				}
			}
		}
	}

	[Tooltip("HUD bullets to display bullet count on screen. Will be find under name 'HUD_bullets' in scene.")]
	public TextMesh HUD_bullets;

	void OnGUI()
	{
		if(!HUD_bullets)
		{
			try
			{
				HUD_bullets = GameObject.Find("HUD_bullets").GetComponent<TextMesh>();
			}
			catch(System.Exception ex)
			{
				print("Couldnt find the HUD_Bullets ->" + ex.StackTrace.ToString());
			}
		}
		if(mouseLook && HUD_bullets)
			HUD_bullets.text = bulletsIHave.ToString() + " - " + bulletsInTheGun.ToString();
	}

	//#####		RETURN THE SIZE AND POSITION for GUI images ##################
	private float position_x(float var)
	{
		return Screen.width * var / 100;
	}
	private float position_y(float var)
	{
		return Screen.height * var / 100;
	}
	private float size_x(float var)
	{
		return Screen.width * var / 100;
	}
	private float size_y(float var)
	{
		return Screen.height * var / 100;
	}
	private Vector2 vec2(Vector2 _vec2)
	{
		return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
	}

}
