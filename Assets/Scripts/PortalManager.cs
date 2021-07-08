using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalManager : MonoBehaviour
{
    public class ObjectTravel {
        public Traveler traveler;
        public Portal from;
        public Portal to;

        public ObjectTravel(Traveler t, Portal from, Portal to) {
            this.traveler = t;
            this.from = from;
            this.to = to;
        }
    }

    public static PortalManager instance;

    #region Objects travels
    private List<ObjectTravel> objectTravels;
    private List<GameObject> travelers;
    #endregion

    #region Player travel
    private bool playerTraveling = false;
    public bool IsPlayerTraveling { get { return playerTraveling; } }

    private bool justChangedLayer = false;
    public bool JustChangedLayer { get { return justChangedLayer; } }
    private Portal from;
    private Portal to;
    #endregion

    private void Awake()
    {
        #region Singleton
        if (instance) {
            Destroy(this.gameObject);
        }
        instance = this;
        #endregion

        objectTravels = new List<ObjectTravel>();
        travelers = new List<GameObject>();

    }

    public void AddTraveler(GameObject traveler, Portal from, Portal to) {
        int index = travelers.BinarySearch(traveler);
        if (index < 0)
        {
            traveler.GetComponent<Collider>().isTrigger = true;
            traveler.layer = LayerMask.NameToLayer("ObjectTravelling");

            travelers.Add(traveler);
            ObjectTravel ot = new ObjectTravel(traveler.GetComponent<Traveler>(), from, to);
            objectTravels.Add(ot);
        }
    }

    public void DeleteTraveler(GameObject traveler) {
        int index = travelers.BinarySearch(traveler);
        if (index >= 0)
        {
            Traveler t = travelers[index].GetComponent<Traveler>();
            GrabbableObject gbo = travelers[index].GetComponent<GrabbableObject>();
            if (t && gbo && !gbo.IsGrabbed())
            {
                t.EndTravel();
            }

            travelers.RemoveAt(index);
            objectTravels.RemoveAt(index);
        }
    }

    void LateUpdate()
    {
        if (playerTraveling) {
            Vector3 portalToCameraFlat = (Vector3.Scale(Player.instance.mainCamera.position, new Vector3(1, 0, 1)) + Vector3.up * from.transform.position.y) - from.transform.position;
            if (Mathf.Abs(Vector3.SignedAngle(from.transform.up, portalToCameraFlat, Vector3.up)) > 90)
            {
                TransportatePlayer();
                SwapPlayerPortals();
            }
        }

        foreach (ObjectTravel ot in objectTravels)
        {
            if(!ot.traveler.IsTravelling())
                ot.traveler.StartTravelThroughPortal(ot.from.transform, ot.to.transform);
        }
    }

    public void TransportatePlayer()
    {
        Vector3 playerDirection = transform.position - Player.instance.transform.position;

        // Vector3 newPosition = linkedPortal.transform.position + playerDirection;
        Vector3 newPosition = from.portalCamera.position + from.transform.up * 0.05f;

        Player.instance.transform.position = newPosition - Vector3.Scale(Player.instance.mainCamera.localPosition, Vector3.up);
        float rotationAngle = Vector3.SignedAngle(transform.eulerAngles, -to.transform.eulerAngles, Vector3.up);
        //player.eulerAngles += new Vector3(0, rotationAngle, 0);
        Player.instance.transform.eulerAngles = new Vector3(0, from.portalCamera.eulerAngles.y, 0);

        Debug.Log("Transportating " + Player.instance.name + " from " + transform.name + " to " + to.transform.name + " in position " + newPosition + " and rotation of " + rotationAngle + " degrees.");

        //Player.instance.gameObject.layer = LayerMask.NameToLayer("Player");

        // This last thing is for bug fixing
        Player.instance.GetComponent<FirstPersonController>().m_MouseLook.m_CharacterTargetRot = Player.instance.transform.localRotation;
        Player.instance.GetComponent<FirstPersonController>().enabled = false;
        StartCoroutine(EnablePlayerControlAfterTravel());
    }

    private IEnumerator EnablePlayerControlAfterTravel()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(0.015f);
        Player.instance.GetComponent<FirstPersonController>().enabled = true;
    }


    private void SwapPlayerPortals() {
        Portal tmp = to;
        to = from;
        from = tmp;
    }

    public void StartPlayerTravel(Portal fromPortal, Portal toPortal) {
        from = fromPortal;
        to = toPortal;
        playerTraveling = true;
        Player.instance.gameObject.layer = LayerMask.NameToLayer("PlayerTravelling");
        justChangedLayer = true;
    }

    public void EndPlayerTravel() {
        playerTraveling = false;
        from = null;
        to = null;
        Player.instance.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void SetLayerFlag(bool value) { justChangedLayer = value; }
}
