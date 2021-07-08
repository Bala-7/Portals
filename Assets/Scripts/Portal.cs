using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public Transform portalCamera;
    private Transform player;
    private Transform playerCamera;
    private Transform gunCamera;

    private float transportationDistance = 0.4f;
    private Animator ac;

    #region Portal elements
    private GameObject closedTexture;
    private GameObject portalGO;
    private GameObject borderGO;
    
    [SerializeField]
    private Material openPortalMaterial;
    [SerializeField]
    private Material closedPortalMaterial;
    #endregion
    
    private bool isOpened = false;
    public bool IsOpened { get { return isOpened; } }


    private void Awake()
    {
        player = FindObjectOfType<FirstPersonController>().transform;
        playerCamera = player.Find("FirstPersonCharacter");
        gunCamera = player.Find("GunCamera");
        ac = GetComponent<Animator>();
        closedTexture = transform.Find("PortalClosed").gameObject;
        portalGO = transform.Find("Portal").gameObject;
        borderGO = transform.Find("PortalRing").gameObject;
    }

    private void Start()
    {
        portalGO.SetActive(false);
        borderGO.SetActive(false);
        closedTexture.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraPositionAndRotation();
    }

    private void UpdateCameraPositionAndRotation() {

        // First I calculate the angle between portals normals
        float portalsFwdDelta = Vector3.SignedAngle(transform.forward, linkedPortal.transform.forward, Vector3.up);

        // I use rotated vector to calculate the final position of the portal camera
        Vector3 playerToPortal = transform.position - playerCamera.position;
        Vector3 rotatedPlayerToPortal = Quaternion.Euler(0, portalsFwdDelta, 0) * playerToPortal;
        Vector3 linkedPortalPlayer = linkedPortal.transform.position - rotatedPlayerToPortal;
        portalCamera.position = linkedPortalPlayer;
        portalCamera.RotateAround(linkedPortal.transform.position, Vector3.up, 180);

        // Fix the portal camera rotation
        float playerToPortalFwdDelta = Vector3.SignedAngle(transform.forward, playerCamera.forward, Vector3.up);
        float angleToRotateViewY = linkedPortal.transform.eulerAngles.y - 180;
        portalCamera.eulerAngles = linkedPortal.transform.forward + new Vector3(playerCamera.eulerAngles.x, playerToPortalFwdDelta + angleToRotateViewY, 0);

    }

    public void OpenPortal() {
        if (!isOpened)
        {
            isOpened = true;
            portalGO.SetActive(true);
            borderGO.SetActive(true);

            if (linkedPortal.IsOpened) linkedPortal.OnLinkedPortalOpened();
            //else closedTexture.SetActive(true);
        }

        ac.Play("PortalOpen");
        portalGO.GetComponent<MeshRenderer>().material = (linkedPortal.IsOpened) ? openPortalMaterial : closedPortalMaterial;
        //closedTexture.SetActive(!linkedPortal.IsOpened);
    }

    public void OnLinkedPortalOpened() {
        //closedTexture.SetActive(false);
        portalGO.GetComponent<MeshRenderer>().material = openPortalMaterial;
    }

    #region Triggers

    void OnTriggerEnter(Collider other)
    {
        // Object collides with this portal
        Traveler t = other.GetComponent<Traveler>();
        if ((t && !t.IsTravelling()))
        {
            PortalManager.instance.AddTraveler(other.gameObject, this, linkedPortal);
        }

        // Player collides with this portal
        if (other.CompareTag("Player") && !PortalManager.instance.IsPlayerTraveling) {
            PortalManager.instance.StartPlayerTravel(this, linkedPortal);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Traveler t = other.GetComponent<Traveler>();
        if ((t && t.IsTravelling() && t.gameObject.layer == LayerMask.NameToLayer("ObjectTravelling")))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Traveler");
            PortalManager.instance.DeleteTraveler(other.gameObject);
        }


        if (PortalManager.instance.JustChangedLayer)
            PortalManager.instance.SetLayerFlag(false);
        else if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTravelling"))
        {
            PortalManager.instance.EndPlayerTravel();
        }
    }

    #endregion
}