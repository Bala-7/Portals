using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjects : MonoBehaviour
{
    private Transform _cam;
    private Transform _objectHold;
    private Transform _objectAim;

    private float _distance = 9.0f;
    private bool _holdingObject;
    private bool _holdEnabled = true;

    private KeyCode grabButton = KeyCode.E;
    private KeyCode releaseObjectButton = KeyCode.X;

    private void Awake()
    {
        _cam = transform.Find("FirstPersonCharacter");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        bool isAimingObject = IsAimingObject();         // Check if player is aiming at an interactable object
        

        if (isAimingObject)
        {
            if (GrabButtonHold())
            {                  // If player press Right Click
                if (!_holdingObject && _holdEnabled)
                {
                    float d = Vector3.Distance(_objectAim.position, transform.position);
                    if (d > 2.0f)
                    {
                        // Attracts the object
                        float force = 3.0f;
                        Vector3 dirToPlayer = _cam.position - _objectAim.position;
                        Rigidbody rb = _objectAim.GetComponent<Rigidbody>();
                        rb.AddForce(dirToPlayer * force, ForceMode.Force);
                    }
                    else
                    {
                        LockObject();
                        
                    }
                }

            }
        }

        if (_holdingObject)
        {
            if (ReleaseButtonDown())
            {
                ReleaseObject(Vector3.zero);

            }
            else if (GrabButtonDown())
            {
                ReleaseObject((transform.forward + Vector3.up * 0.2f).normalized * 10.0f);
            }



        }
    }

    private void ReleaseObject(Vector3 newVelocity)
    {
        _holdingObject = false;
        /*_objectHold.parent = null;

        Rigidbody objectRigidBody = _objectHold.GetComponent<Rigidbody>();
        objectRigidBody.useGravity = true;
        objectRigidBody.constraints = RigidbodyConstraints.None;
        objectRigidBody.velocity = newVelocity;
        */
        _objectHold.GetComponent<GrabbableObject>().Drop(newVelocity);

        _objectHold = null;
        _holdEnabled = false;

        Invoke("EnableHold", 1.0f);
    }

    private void LockObject()
    {
        _holdingObject = true;
        _objectHold = _objectAim;
        _objectAim = null;

        _objectHold.GetComponent<GrabbableObject>().Grab(_cam);

        // Sets new position and rotation
        /*_objectHold.position = transform.position + transform.forward * 1.25f;
        _objectHold.rotation = transform.rotation;

        // Sets the RigidBody parameters
        Rigidbody objectRigidBody = _objectHold.GetComponent<Rigidbody>();
        objectRigidBody.useGravity = false;
        objectRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        objectRigidBody.velocity = Vector3.zero;

        // Sets the new object as a child of the gravity gun
        _objectHold.transform.parent = transform;*/
    }

    private bool IsAimingObject()
    {
        // Bit shift the index of the layer (9 or 11) to get a bit mask
        int layerMask = (1 << 9) | (1 << 11);

        RaycastHit hit;
        bool isAimingObject = Physics.Raycast(_cam.position, _cam.forward, out hit, _distance, layerMask);
        if (isAimingObject)
            _objectAim = hit.transform;

        return isAimingObject;
    }

    private void EnableHold() { _holdEnabled = true; }

    private bool GrabButtonHold() { return Input.GetKey(grabButton); }

    private bool GrabButtonDown() { return Input.GetKeyDown(grabButton); }

    private bool ReleaseButtonDown() { return Input.GetKeyDown(releaseObjectButton); }


}
