using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VehicleNetworkMovementManager : NetworkBehaviour
{
    private CarPhysics carController;

    [SyncVar]
    public PlayerInputs playerInputs = new PlayerInputs();

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
            UpdateOtherCarInputs(playerInputs);
        }	
	}

    [Command]
    public void CmdUpdateInputs(float steer, float forward, float reverse, bool handBrake)
    {
        playerInputs.steer = steer;
        playerInputs.forward = forward;
        playerInputs.reverse = reverse;
        playerInputs.handBrake = handBrake;

    }

    
    public void UpdateOtherCarInputs(PlayerInputs playerInput)
    {
        carController.steerInput = playerInput.steer;
        carController.forwardInput = playerInput.forward;
        carController.reverseInput = playerInput.reverse;
        carController.handbrakeInput = playerInput.handBrake;
    }

}
