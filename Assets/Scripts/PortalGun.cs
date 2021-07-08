using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    private enum PORTAL_TYPE { BLUE = 0, ORANGE }
    [SerializeField]
    private Portal orangePortal;
    [SerializeField]
    private Portal bluePortal;

    private Vector3 basePortalEuler = new Vector3(0, 180, 90);

    private Animator ac;

    private void Awake()
    {
        ac = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            FirePortal(PORTAL_TYPE.BLUE);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            FirePortal(PORTAL_TYPE.ORANGE);
        }
    }

    private void FirePortal(PORTAL_TYPE type) {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 13;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        RaycastHit hit;

        
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Vector3 checkPosition = hit.point + hit.normal.normalized * 0.015f;
            if (DoesPortalHaveSpace(checkPosition, hit.normal)) {
                
                Portal portalToPlace = (type == PORTAL_TYPE.BLUE) ? bluePortal : orangePortal;
                portalToPlace.transform.position = checkPosition/* + hit.normal.normalized * 0.025f*/;
                //portalToPlace.transform.eulerAngles = CalculatePortalRotation(portalToPlace.transform, hit.normal);
                Vector3 newRotation = CalculatePortalRotation(portalToPlace.transform, hit.normal);
                portalToPlace.transform.rotation = Quaternion.Euler(newRotation);
                portalToPlace.OpenPortal();
                ac.Play("Fire");
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

    }

    private bool DoesPortalHaveSpace(Vector3 wallPosition, Vector3 wallNormal) {
        RaycastHit hit;
        Vector3 wallUp = Vector3.up;    // Change this to adapt to any surface
        Vector3 wallDown = Vector3.down;    // Change this to adapt to any surface
        Vector3 wallLeft = Quaternion.Euler(0, -90, 0) * wallNormal;
        Vector3 wallRight = Quaternion.Euler(0, 90, 0) * wallNormal;

        Debug.DrawRay(wallPosition, wallUp);
        Debug.DrawRay(wallPosition, wallDown);
        Debug.DrawRay(wallPosition, wallLeft);
        Debug.DrawRay(wallPosition, wallRight);
        // Debug.Break();

        bool obstacleUp = Physics.Raycast(wallPosition, wallUp, out hit, 1.95f);
        bool obstacleDown = Physics.Raycast(wallPosition, wallDown, out hit, 1.95f);
        bool obstacleLeft = Physics.Raycast(wallPosition, wallLeft, out hit, 1.25f);
        bool obstacleRight = Physics.Raycast(wallPosition, wallRight, out hit, 1.25f);
        
        return !obstacleUp && !obstacleDown && !obstacleLeft && !obstacleRight;
    }

    private Vector3 CalculatePortalRotation(Transform portal, Vector3 wallNormal) {
        float xDelta = Vector3.SignedAngle(wallNormal, portal.up, Vector3.forward);
        float yDelta = Vector3.SignedAngle(wallNormal, portal.up, Vector3.up);
        float zDelta = Vector3.SignedAngle(wallNormal, portal.up, Vector3.right);

        //Vector3 newRotation = (Quaternion.Euler(0, 0, -90*wallNormal.z) * portal.eulerAngles);
        //Vector3 newRotation = Quaternion.Euler(0, 180, 0) * wallNormal;
        // TODO: Resolver este problema porque nunca se queda con la rotación correcta -_-
        Vector3 newRotation = new Vector3(0, 180*((wallNormal.x>0)?wallNormal.x:0)+ 90*wallNormal.z, 90);

        Debug.Log("Rotating portal to: " + newRotation);
        Debug.Log("Deltas: " + xDelta + " | " + yDelta + " | " + zDelta);
        Debug.Log("Wall normal: " + wallNormal);
        return newRotation;
    }
}
