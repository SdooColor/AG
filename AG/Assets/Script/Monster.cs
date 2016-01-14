using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {
    private Animator controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnCollisionEnter(Collision collision) {
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        if (other.gameObject.name == "WeaponMode")
        {
            Beta beta = (Beta)other.gameObject.GetComponentInParent<Beta>();
            Debug.Log(beta);
            if (beta.isAttack)
            {
                controller.SetTrigger("Stricken");
            }
        }
    }
}
