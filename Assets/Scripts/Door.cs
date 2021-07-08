using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator ac;

    private enum DOOR_STATE { CLOSED = 0, OPENED };
    private DOOR_STATE state = DOOR_STATE.CLOSED;

    private bool IsOpened { get { return state == DOOR_STATE.OPENED; } }

    public bool openDefault = false;

    private void Awake()
    {
        ac = GetComponent<Animator>();
    }

    private void Start()
    {
        if (openDefault) Open();
    }

    public void Open() {
        ac.Play("DoorOpen");
        state = DOOR_STATE.OPENED;
    }

    public void Close() {
        ac.Play("DoorClose");
        state = DOOR_STATE.CLOSED;
    }
}
