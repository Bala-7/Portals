using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player instance;
    public Transform mainCamera;
    public Transform gunCamera;

    private void Awake()
    {
        #region Singleton
        if (instance)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        #endregion

        mainCamera = transform.Find("FirstPersonCharacter");
        gunCamera = transform.Find("GunCamera");
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
