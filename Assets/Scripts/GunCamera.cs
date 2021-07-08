using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCamera : MonoBehaviour
{
    private Transform viewCamera;

    private void Awake()
    {
        viewCamera = transform.parent.Find("FirstPersonCharacter");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = viewCamera.position;
        transform.rotation = viewCamera.rotation;
    }
}
