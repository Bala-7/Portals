using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    private Animator ac;
    private enum BUTTON_STATE { NOT_PRESSED = 0, PRESSED };
    private BUTTON_STATE state = BUTTON_STATE.NOT_PRESSED;

    public Door linkedDoor;

    private bool IsPressed {get {return state == BUTTON_STATE.PRESSED;}}

    private void Awake()
    {
        ac = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPressed && IsObjectPressing())
        {
            Activate();
        }
        else if (IsPressed && !IsObjectPressing()) {
            Deactivate();
        }
    }

    private bool IsObjectPressing() {
        Vector3 fwd = transform.TransformDirection(Vector3.up);
        Vector3 from = transform.position + Vector3.up * .35f;
        Debug.DrawRay(from, Vector3.up, Color.red);
        return Physics.Raycast(from, fwd, 1);
    }

    private void Activate() {
        ac.Play("ButtonPress");
        state = BUTTON_STATE.PRESSED;
        linkedDoor.Open();
    }

    private void Deactivate() {
        ac.Play("ButtonUnpress");
        state = BUTTON_STATE.NOT_PRESSED;
        linkedDoor.Close();
    }

}
