using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgaguriController : MonoBehaviour
{
    public GameObject hitEffect;

    void OnCollisionEnter(Collision other)
    {
        
        GetComponent<Rigidbody>().isKinematic = true;

        foreach (ContactPoint contactPoint in other.contacts)
        {
            GameObject hiteffects = (GameObject)Instantiate(hitEffect, (Vector3)contactPoint.point, Quaternion.identity);
            hiteffects.transform.localScale = new Vector3(2, 2, 2);
            Destroy(hiteffects, 1.0f);
        }

        Destroy(this.gameObject);

    }
}
