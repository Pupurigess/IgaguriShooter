using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject shellPrefab;
    public float shotSpeed;
    public int shotCount = 30;
    public AudioClip shotSound;
    public AudioClip reloadSound;
    private float shotInterval;

    void Update()
    {

        // もしも「Fire1」というボタンが押されたら（条件）
        //if (Input.GetButtonDown("Fire1"))
        if (Input.GetMouseButton(0) || Input.GetButton("Fire1"))
        {

            shotInterval += 1;

            if (shotInterval % 10 == 0 && shotCount > 0) 
            {
                shotCount -= 1;

                Shot();// ①Shotという名前の関数（命令ブロック）を実行する。

                // ②効果音を再生する。
                AudioSource.PlayClipAtPoint(shotSound, transform.position);
            }


        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Fire2"))
        {
            shotCount = 30;
            AudioSource.PlayClipAtPoint(reloadSound, transform.position);
        }

    }


    public void Shot()
    {

        // プレファブから砲弾(Shell)オブジェクトを作成し、それをshellという名前の箱に入れる。
        GameObject shell = (GameObject)Instantiate(shellPrefab, transform.position, Quaternion.identity);

        // Rigidbodyの情報を取得し、それをshellRigidbodyという名前の箱に入れる。
        Rigidbody shellRigidbody = shell.GetComponent<Rigidbody>();

        // shellRigidbodyにz軸方向の力を加える。
        shellRigidbody.AddForce(transform.forward * shotSpeed);

        Destroy(shell, 3.0f);

    }
}
