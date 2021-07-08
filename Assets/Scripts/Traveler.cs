using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveler : MonoBehaviour
{
    public GameObject clonePrefab;
    
    private GameObject cloneInstance;
    private Transform portalA;
    private Transform portalB;

    private bool isTraveling = false;

    private Vector3 prevClonePosition;

    private float travelRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTraveling) {
            HandlePortalTravel();
        }
    }

    public void StartTravelThroughPortal(Transform p1, Transform p2) {
        portalA = p1;
        portalB = p2;
        isTraveling = true;

        
        Vector3 objectToPortalAFwd = p1.position - transform.position;
        travelRotation = Vector3.SignedAngle(p1.forward, p2.forward, Vector3.up);
        Vector3 rotatedObjectToPortal = Quaternion.Euler(0, travelRotation, 0) * objectToPortalAFwd;

        Vector3 clonePosition = p2.position - rotatedObjectToPortal;

        cloneInstance = Instantiate(clonePrefab, clonePosition, transform.rotation);
        cloneInstance.layer = LayerMask.NameToLayer("Clone");
    }

    private void HandlePortalTravel() {
        prevClonePosition = cloneInstance.transform.position;

        Vector3 objectToPortalAFwd = portalA.position - transform.position;
        travelRotation = Vector3.SignedAngle(portalA.forward, -portalB.forward, Vector3.up);
        Vector3 rotatedObjectToPortal = Quaternion.Euler(0, travelRotation, 0) * objectToPortalAFwd;
        Vector3 clonePosition = portalB.position - rotatedObjectToPortal;
        cloneInstance.transform.position = clonePosition;
        cloneInstance.transform.eulerAngles = Quaternion.Euler(0, travelRotation, 0) * transform.eulerAngles;
    }

    public void EndTravel() {
        isTraveling = false;
        GrabbableObject gbo = GetComponent<GrabbableObject>();
        GetComponent<Collider>().isTrigger = false;
        if (gbo) {
            if (gbo.IsGrabbed()) { 
                
            }
            else{
                transform.position = cloneInstance.transform.position;
                transform.rotation = cloneInstance.transform.rotation;
                
                Rigidbody rb = GetComponent<Rigidbody>();
                Vector3 newVelocity = Quaternion.Euler(0, travelRotation, 0) * rb.velocity;
                rb.velocity = newVelocity;
            }
        }
        Destroy(cloneInstance);
    }

    public bool IsTravelling() { return isTraveling; }
}
