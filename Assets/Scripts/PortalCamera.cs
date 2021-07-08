using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform player;
    public Transform portalA;
    public Transform portalB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePositionAndRotation();
    }

    private void UpdatePositionAndRotation() {
        Vector3 playerToPortalDirection = portalA.position - player.position;

        transform.position = portalB.position - playerToPortalDirection;

        transform.rotation = player.GetChild(0).rotation;
    }
}
