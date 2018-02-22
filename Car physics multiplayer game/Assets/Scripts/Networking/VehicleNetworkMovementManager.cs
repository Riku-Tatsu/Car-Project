using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VehicleNetworkMovementManager : NetworkBehaviour
{
    private CarPhysics carController;

    [SyncVar]
    public PlayerInputs syncedPlayerInputs = new PlayerInputs();

    [SyncVar]
    public Quaternion syncRBRot;

    [SyncVar]
    public Vector3 syncRBPos;

    [SerializeField]
    float lerpRate = 15;

    private Rigidbody rb;

    [System.Serializable]
    public class PlayerInputs
    {
       public float steer;
       public float forward;
       public float reverse;
       public bool handBrake; 

        public PlayerInputs()
        {
            steer = 0;
            forward = 0;
            reverse = 0;
            handBrake = false;
        }
    }

	// Use this for initialization
	void Start ()
    {
        carController = GetComponent<CarPhysics>();
        carController.VehicleNetworkManager = this;
        rb = GetComponent<Rigidbody>();

        if (isLocalPlayer)
            carController.usedByPlayer = true;

        else
            carController.usedByPlayer = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!isLocalPlayer)
        {
            UpdateOtherCarInputs(syncedPlayerInputs);
        }
       
	}

    void FixedUpdate()
    {
        TransmitRBVal();
        LerpRBValues();
    }

    [Command]
    public void CmdUpdateInputs(PlayerInputs playerInputs)
    {
        syncedPlayerInputs = playerInputs;
    }

    public void GrabPlayerInputs(float steer, float forward, float reverse, bool handBrake)
    {
        syncedPlayerInputs.steer = steer;
        syncedPlayerInputs.forward = forward;
        syncedPlayerInputs.reverse = reverse;
        syncedPlayerInputs.handBrake = handBrake;

        CmdUpdateInputs(syncedPlayerInputs);


    }

    [Command]
    public void CmdUpdateSyncedRBPos(Vector3 pos, Quaternion rot)
    {
        syncRBPos = pos;
        syncRBRot = rot;
    }

    [ClientCallback]
    public void TransmitRBVal()
    {
        if (isLocalPlayer)
        {
            CmdUpdateSyncedRBPos(rb.position, rb.rotation);
        }
    }

    [ClientCallback]
    public void UpdateOtherCarInputs(PlayerInputs playerInput)
    {
        carController.steerInput = playerInput.steer;
        carController.forwardInput = playerInput.forward;
        carController.reverseInput = playerInput.reverse;
        carController.handbrakeInput = playerInput.handBrake;
    }

    [ClientCallback]
    void LerpRBValues()
    {
        if (!isLocalPlayer)
        {
            rb.position = Vector3.Lerp(rb.position, syncRBPos, Time.deltaTime * lerpRate);
            rb.rotation = Quaternion.Slerp(rb.rotation, syncRBRot, Time.deltaTime * lerpRate);
        }
    }



}
