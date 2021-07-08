using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    private Transform linkedCam;
    private Rigidbody _rb;
    private bool isGrabbed = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (linkedCam) {
            transform.position = linkedCam.position + linkedCam.forward * 2.25f - linkedCam.up * 0.5f;
            transform.rotation = linkedCam.rotation;
        }
    }

    public void Grab(Transform camera) {
        linkedCam = camera;

        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _rb.velocity = Vector3.zero;

        isGrabbed = true;
    }

    public void Drop(Vector3 newVelocity) {
        linkedCam = null;

        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.None;
        _rb.velocity = newVelocity;

        Traveler t = GetComponent<Traveler>();
        if (t && t.IsTravelling()) 
            t.EndTravel();
        isGrabbed = false;
    }


    public bool IsGrabbed() { return isGrabbed; }
}
