using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] private float targetHitpoint = 5.0f;
    public float rotationalSpeed = 180f;
    private float rotationalSpeed_X, rotationalSpeed_Y, rotationalSpeed_Z;

    public GameManager gameManager;

    public GameObject explosionEffect;

    public AudioClip spawn_sound;
    public AudioClip destroy_sound;
    //public AudioClip destroy_sound2;
    public GameObject destroy_sound2;

    bool destroy_bool = false;
    Rigidbody rb_target;

    private int rotationalDirection_X;
    private int rotationalDirection_Y;
    private int rotationalDirection_Z;

    private void Start()
    {
        rb_target = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        rb_target.isKinematic = true;

        rotationalDirection_X = Random.Range(1, 3);//ランダムで１から２を代入
        rotationalDirection_Y = Random.Range(1, 3);
        rotationalDirection_Z = Random.Range(1, 3);

        if (rotationalDirection_X == 2)
        {
            rotationalDirection_X = -1;
        }
        
        if (rotationalDirection_Y == 2)
        {
            rotationalDirection_Y = -1;
        }

        if (rotationalDirection_Z == 2)
        {
            rotationalDirection_Z = -1;
        }
        
        rotationalSpeed_X = rotationalSpeed * rotationalDirection_X;
        rotationalSpeed_Y = rotationalSpeed * rotationalDirection_Y;
        rotationalSpeed_Z = rotationalSpeed * rotationalDirection_Z;

        audioSource.PlayOneShot(spawn_sound);

    }

Vector3 hitPos;

    void OnCollisionEnter(Collision other)
    {
        targetHitpoint -= 1;
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(rotationalSpeed_X, rotationalSpeed_Y, rotationalSpeed_Z) * Time.deltaTime, Space.World);

        if (targetHitpoint <= 0  && !destroy_bool )
        {
            destroy_bool = true;
            AudioSource.PlayClipAtPoint(destroy_sound, transform.position);
            rb_target.isKinematic = false;

            if (explosionEffect)
            {
                GameObject explosion = (GameObject)Instantiate(explosionEffect, transform.position, Quaternion.identity);

                audioSource.PlayOneShot(destroy_sound);

                Destroy(explosion, 3.0f);
            }
            else
                print("Missing the explosionEffect prefab");

            gameManager.totalScore += 100; 
            Invoke("DelayMethod", 2.0f);
        }

        if (destroy_bool)
        {
            transform.Rotate(new Vector3(rotationalSpeed_X * 2, rotationalSpeed_Y * 2, 0) * Time.deltaTime, Space.World);
        }
    }

    void DelayMethod()
    {
        if (explosionEffect)
        {
            GameObject explosion = (GameObject)Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(4, 4, 4);
            
            Destroy(explosion, 3.0f);
        }
        else
            print("Missing the explosionEffect prefab");

        Instantiate(destroy_sound2, transform.position, transform.rotation);
        Destroy(this.gameObject, 0.0f);
    }
}
