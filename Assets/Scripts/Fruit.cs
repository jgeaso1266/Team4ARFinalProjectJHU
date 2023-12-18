using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Fruit : MonoBehaviour
{

    public Material newMaterial;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;

    private void onTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MeshRenderer myRenderer = GetComponent<MeshRenderer>();
            myRenderer.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
