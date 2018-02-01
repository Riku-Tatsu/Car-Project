using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPhysics : MonoBehaviour {

	[System.Serializable]
	public class WheelData
	{
		public Transform wheelPivot;
		public Transform wheelCastPoint;
		public float wheelRadius;

		public bool isLeft;
		public bool isFront;
		public bool canSteer;

		[HideInInspector]
		public RaycastHit rHit;
		[HideInInspector]
		public float gripMult;

		[HideInInspector]
		public bool isGrounded;
		[HideInInspector]
		public float prevSuspDist;
		[HideInInspector]
		public Vector3[] lastWorldPoses;
		[HideInInspector]
		public Vector3 stopPosWorld;
		[HideInInspector]
		public Vector3 stopPosVector;
		[HideInInspector]
		public Vector3 posVelocityWorld;
		[HideInInspector]
		public float lastStopPosDist;

		[HideInInspector]
		public Vector3 lastWorldPos;
		[HideInInspector]
		public float rotation;
		[HideInInspector]
		public float rpm;
		[HideInInspector]
		public bool isLocked;
		[HideInInspector]
		public float spinAmount;
	}

	[Header("---Inputs---")]
	public string steer = "Steering";
	public string forward = "Forward";
	public string reverse = "Reverse";
	public string handbrake = "Handbrake";
	[Space]
	public bool isDriving = false;
	public bool usedByPlayer = false;

	[Header("---Physics Info---")]
	public WheelData[] wheels;
	public Collider chassisCollider;
	public Vector3 centreOfMass = Vector3.zero;
	public Vector3 inertiaMod = Vector3.one;

	[Header("---Suspension---")]
	public float sMaxDistance = 0.25f;
	public float sRestDistance = 0.1f;
	public float sSpring = 2.0f;
	public float sCompressionDamping = 1.0f;
	public float sReboundDamping = 2.0f;

	[Space]
	public float sRollHeightFront = 0.3f;
	public float sRollHeightRear = 0.4f;

	[Header("---Tyres---")]
	public float tMaxGrip = 1.5f;
	public float tBiasFront = 0.495f;
	public float tDriftAbility = 1.0f;

	public float tPeakAngle = 10.0f;
	public float tSlipTrail = 0.1f;

	public float tStopRadius = 0.02f;
	public float tStopDamping = 1.0f;

	[Header("---Engine & Transmission---")]
	public float mAcceleration = 2.0f;
	public float mBiasFront = 0.5f;

	[Space]
	public float mTopSpeedKmh = 100.0f;
	public float mReverseKmh = 50.0f;

	[Header("")]
	public float bDeceleration = 1.4f;
	public float bBiasFront = 0.55f;

	[Header("---Controls---")]
	public float cSteerMax = 35.0f;
	public float cSteerSpeed = 8.0f;
	public float cSteerWeight = 1.0f;
	public float cSteerFrontAmount = 1.0f;
	public float cSteerRearAmount = 0.0f;
	public bool cRearWheelsCanCounter = true;
	[Space]
	public float cFreeVelocity = 3.0f;

	[HideInInspector]
	public bool isBraking;
	[HideInInspector]
	public bool isReversing;
	[HideInInspector]
	public bool headlightsOn = false;

	Vector4 velocity;

	bool isGrounded;
	Rigidbody rBody;
	Vector2 wheelbase = Vector2.zero;
	Vector3 steerAngleLocationFront;
	Vector3 steerAngleLocationRear;
	float stationaryMult = 1;
	Vector3 lastChassisPos;
	Quaternion lastChassisRot;
	float counterAmount;
	float steerAngleFront = 0;
	float steerAngleRear = 0;
	float steerFactor = 0;
	float steerFactorReciprocated = 0;

	float steerInput;
	float forwardInput;
	float reverseInput;
	bool handbrakeInput;

	float motorInput;
	float brakeInput;

	void Start ()
	{
		rBody = transform.root.GetComponent<Rigidbody>();
		rBody.inertiaTensor = Vector3.Scale(rBody.inertiaTensor, inertiaMod);
		rBody.centerOfMass = centreOfMass;

		lastChassisPos = transform.position;
		lastChassisRot = transform.rotation;

		int frontWheels = 0;
		int rearWheels = 0;
		for(int i = 0; i < wheels.Length; i++)
		{
			wheels[i].lastWorldPoses = new Vector3[4];
			wheels[i].lastWorldPoses[0] = wheels[i].wheelPivot.position - wheels[i].wheelPivot.up * wheels[i].wheelRadius;
			wheels[i].lastWorldPoses[1] = wheels[i].wheelPivot.position - wheels[i].wheelPivot.up * wheels[i].wheelRadius;
			wheels[i].lastWorldPoses[2] = wheels[i].wheelPivot.position - wheels[i].wheelPivot.up * wheels[i].wheelRadius;
			wheels[i].lastWorldPoses[3] = wheels[i].wheelPivot.position - wheels[i].wheelPivot.up * wheels[i].wheelRadius;

			wheels[i].stopPosWorld = wheels[i].wheelPivot.position;

			if (wheels[i].isFront) { frontWheels++; steerAngleLocationRear += wheels[i].wheelPivot.localPosition; } else { rearWheels++; steerAngleLocationFront += wheels[i].wheelPivot.localPosition; }

			if(wheels[i].canSteer)
			{
				steerFactor += (wheels[i].isFront ? cSteerFrontAmount : cSteerRearAmount);
			}

			wheelbase += new Vector2(wheels[i].wheelPivot.localPosition.x, wheels[i].wheelPivot.localPosition.z);
		}

		steerAngleLocationFront = new Vector3(0, steerAngleLocationFront.y / rearWheels, steerAngleLocationFront.z / rearWheels);
		steerAngleLocationRear = new Vector3(0, steerAngleLocationRear.y / frontWheels, steerAngleLocationRear.z / frontWheels);

		//steerFactor = 1f / steerFactor;
		steerFactorReciprocated = 1f / steerFactor;

		wheelbase *= 1f / wheels.Length;
	}
	
	void PollInputs()
	{
		if(isDriving)
		{
			if(usedByPlayer)
			{
				steerInput = Input.GetAxisRaw(steer);
				forwardInput = Input.GetAxisRaw(forward);
				reverseInput = Input.GetAxisRaw(reverse);
				handbrakeInput = Input.GetButton(handbrake);
			}
			else
			{
				steerInput = 0;
				forwardInput = 0;
				reverseInput = 0;
				handbrakeInput = true;
			}
		}
		else
		{
			steerInput = 0;
			forwardInput = 0;
			reverseInput = 0;
			handbrakeInput = true;
		}
		
		if(velocity.z > cFreeVelocity)
		{
			motorInput = forwardInput;
			brakeInput = reverseInput;
		}
		else if(velocity.z < -cFreeVelocity)
		{
			motorInput = -reverseInput;
			brakeInput = forwardInput;
		}
		else
		{
			if(forwardInput != 0 || reverseInput != 0 || !isGrounded)
			{
				motorInput = (forwardInput - reverseInput);
				brakeInput = 0;
			}
			else
			{
				motorInput = 0;
				brakeInput = bDeceleration;
			}

		}

		isBraking = brakeInput > 0;
		isReversing = motorInput < 0;

		////HARDCODED DEADZONE FOR NOW////

		float sign = Mathf.Sign(steerInput);

		steerInput = Mathf.Clamp01((Mathf.Abs(steerInput) - 0.1f) / 0.9f);
		steerInput = Mathf.Pow(steerInput, 2) * sign;
		

		//Debug.DrawRay(transform.position, transform.right * steerInput, Color.white, 0, false);
		Debug.DrawRay(transform.position, transform.forward * forwardInput, Color.green, 0, false);
		Debug.DrawRay(transform.position, -transform.forward * reverseInput, Color.red, 0, false);
	}

	void Update()
	{
		PollInputs();

		////VISUALS INFO////

		
		for (int i = 0; i < wheels.Length; i++)
		{
			
			float motorBiasMult = 0;
			float brakeBiasMult = 0;
			if(wheels[i].isFront)
			{
				motorBiasMult = mBiasFront;
				brakeBiasMult = bBiasFront;
			}
			else
			{
				motorBiasMult = 1 - mBiasFront;
				brakeBiasMult = 1 - bBiasFront;
			}
			float motorAmountAbs = Mathf.Abs(motorInput * mAcceleration * motorBiasMult);
			//float motorAmount = motorInput * mAcceleration * motorBiasMult;
			float brakeAmount = brakeInput * bDeceleration * brakeBiasMult;


			if (wheels[i].isLocked)
			{
				wheels[i].rpm = 0;
			}
			else if (wheels[i].isGrounded)
			{
				float dirVel = -wheels[i].wheelCastPoint.InverseTransformPoint(wheels[i].lastWorldPos).z;
				float spinVel = (wheels[i].spinAmount >= 0 ? mTopSpeedKmh / 3.6f : -mReverseKmh / 3.6f) * Time.deltaTime;

				float wheelVel = Mathf.Lerp(dirVel, spinVel, Mathf.Abs(wheels[i].spinAmount));
				//float rotation = 360 * dirVel * wheels[i].wheelRadius * Mathf.PI;
				float rotation = (wheelVel / (wheels[i].wheelRadius * Mathf.PI * 2)) * 360;

				wheels[i].rpm = rotation;

				wheels[i].rotation += rotation;
			}
			else
			{

				if(motorAmountAbs >= brakeAmount)
				{
					float rpm = (((motorInput > 0 ? mTopSpeedKmh / 3.6f : mReverseKmh / 3.6f) / (wheels[i].wheelRadius * Mathf.PI * 2)) * 360 * motorInput);
					//wheels[i].rpm = (mReverseKmh / (wheels[i].wheelRadius * Mathf.PI * 2)) * 360 * (motorAmountAbs / mAcceleration);
					
					if(Mathf.Abs(rpm) < Mathf.Abs(wheels[i].rpm))
					{
						wheels[i].rpm *= 0.98f;
					}
					else
					{
						wheels[i].rpm = Mathf.Clamp(wheels[i].rpm + rpm, -mReverseKmh, mTopSpeedKmh);
					}
					//if(rpm <= 0)
					//{
					//	if(rpm < wheels[i].rpm)
					//	{
					//		wheels[i].rpm = Mathf.Min(wheels[i].rpm, rpm);
					//	}
					//	else
					//	{
					//		wheels[i].rpm *= 0.98f;
					//	}
						
					//}
					//else if(rpm > 0)
					//{
					//	if (rpm < wheels[i].rpm)
					//	{
					//		wheels[i].rpm = Mathf.Max(wheels[i].rpm, rpm);
					//	}
					//	else
					//	{
					//		wheels[i].rpm *= 0.98f;
					//	}
					//}
				}
				else if(motorAmountAbs < brakeAmount)
				{
					wheels[i].rpm = 0;
				}
				//else
				//{
				//	wheels[i].rpm *= 0.98f;
				//}

				wheels[i].rotation += wheels[i].rpm;
			}
			wheels[i].lastWorldPos = wheels[i].wheelCastPoint.position;
		}

		////DEBUGGING////

		//Points of interest
		HelperMath.DebugDrawCross(transform, steerAngleLocationFront, 0.1f, Color.white, 0);
		HelperMath.DebugDrawCross(transform, rBody.centerOfMass, 0.1f, Color.magenta, 0);

	
		for(int i = 0; i < wheels.Length; i++)
		{
			//Wheel trail
			HelperMath.DebugDrawCross(wheels[i].lastWorldPoses[1], 0.03f, Color.Lerp(Color.yellow, Color.red, 0.333f), 0);
			HelperMath.DebugDrawCross(wheels[i].lastWorldPoses[2], 0.03f, Color.Lerp(Color.yellow, Color.red, 0.666f), 0);
			HelperMath.DebugDrawCross(wheels[i].lastWorldPoses[3], 0.03f, Color.red, 0);

			//Suspension info
			Color gripColor = Color.magenta;
			if (wheels[i].gripMult >= 1)
			{
				gripColor = Color.Lerp(Color.black, Color.white, Mathf.Clamp01(wheels[i].gripMult - 1));
			}
			else
			{
				gripColor = Color.Lerp(Color.red, Color.black, Mathf.Clamp01(wheels[i].gripMult));
			}
			Debug.DrawRay(wheels[i].wheelPivot.position - transform.up * wheels[i].wheelRadius, wheels[i].rHit.normal * wheels[i].gripMult, gripColor, 0, false);
		}

	}



	void FixedUpdate()
	{
		Vector3 vel = transform.InverseTransformVector(rBody.velocity);
		velocity = new Vector4(vel.x, vel.y, vel.z, vel.magnitude);
		stationaryMult = 1 - Mathf.Clamp01((velocity.w - 0.5f) * 2);
		if(forwardInput > 0.1f || reverseInput > 0.1f)
		{
			stationaryMult = 0;
		}

		CalculateChassisStop();

		CalculateSteering();

		RaycastWheels();

		float velKmh = velocity.w * 3.6f;
		float speedMult = velKmh / mTopSpeedKmh;
		speedMult = Mathf.Pow(1 - speedMult, 2);
		float accelForce = motorInput * mAcceleration * speedMult;
		float brakeForce = brakeInput * bDeceleration;
		//if(vel.z > cFreeVelocity)
		//{
		//	float speedMult = velKmh / mTopSpeedKmh;
		//	speedMult = Mathf.Pow(1 - speedMult, 2);

		//	accelForce = speedMult * forwardInput * mAcceleration;

		//	brakeForce = bDeceleration * reverseInput;
		//}
		//else if(vel.z < -cFreeVelocity)
		//{
		//	float speedMult = velKmh / mReverseKmh;
		//	speedMult = Mathf.Pow(1 - speedMult, 2);

		//	accelForce = speedMult * reverseInput * -mAcceleration;

		//	brakeForce = bDeceleration * forwardInput;
		//}
		//else
		//{
		//	accelForce = motorInput;
		//	brakeForce = 0;
		//}

		if((velocity.w < 1 && stationaryMult > 0) || !isDriving)
		{
			accelForce = 0;
			brakeForce = bDeceleration;
		}

		for (int i = 0; i < wheels.Length; i++)
		{
			////PREP DATA////

			ShiftWorldPoses(i, wheels[i].wheelPivot.position);
			CalculateFrictionVectors(i);

			////SIMULATION EXECUTION////

			if (wheels[i].isGrounded)
			{
				float acc = 0;
				float brk = 0;
				float rollHeight = 0;
				float gripBias = 1.0f;
				if(wheels[i].isFront)
				{
					gripBias = tBiasFront * 2;

					acc = accelForce * mBiasFront;
					brk = brakeForce * bBiasFront;

					rollHeight = sRollHeightFront;
				}
				else
				{
					gripBias = (1 - tBiasFront) * 2;

					acc = accelForce * (1 - mBiasFront);

					if(handbrakeInput)
					{
						brk = tMaxGrip * 10;
					}
					else
					{
						brk = brakeForce * (1 - bBiasFront);
					}
					

					rollHeight = sRollHeightRear;
				}

				RunSuspension(i, gripBias);
				RunFriction(i, acc, brk, rollHeight);
			}

			////SET INFO FOR NEXT FRAME////

			wheels[i].prevSuspDist = wheels[i].rHit.distance;

		}
	}

	void CalculateSteering()
	{
		Vector3 angularVelocity = rBody.angularVelocity * Mathf.Rad2Deg * steerFactorReciprocated;
		//angularVelocity = transform.InverseTransformVector(angularVelocity);

		Vector3 localVelocityFront = transform.InverseTransformDirection(rBody.GetPointVelocity(transform.TransformPoint(steerAngleLocationFront) ));
		float chassisSteerAngleFront = Mathf.Atan2(localVelocityFront.x, localVelocityFront.z) * Mathf.Rad2Deg * inertiaMod.y + -angularVelocity.y * inertiaMod.y;

		Vector3 localVelocityRear = transform.InverseTransformDirection(rBody.GetPointVelocity(transform.TransformPoint(steerAngleLocationRear)));
		float chassisSteerAngleRear = Mathf.Atan2(localVelocityRear.x, localVelocityRear.z) * Mathf.Rad2Deg * inertiaMod.y + -angularVelocity.y * inertiaMod.y;

		if(handbrakeInput)
		{
			chassisSteerAngleFront = 0;
			chassisSteerAngleRear = 0;
		}

		float steeringTangentFactor = tPeakAngle * Mathf.Deg2Rad;
		steeringTangentFactor = steeringTangentFactor / Mathf.Tan(steeringTangentFactor);

		float avgForwardVelocity = (localVelocityFront.z + localVelocityRear.z) * 0.5f;
		float maxSteering = (steeringTangentFactor / Mathf.Clamp(avgForwardVelocity, steeringTangentFactor, Mathf.Infinity)) * 90;
		maxSteering = Mathf.Clamp(maxSteering, tPeakAngle, cSteerMax);

		counterAmount = 0;
		if(Mathf.Sign(steerInput) == Mathf.Sign(chassisSteerAngleFront))
		{
			if(Mathf.Abs(chassisSteerAngleFront) > (tPeakAngle * steerFactor))
			{
				maxSteering = cSteerMax;
			}
			
			if(localVelocityFront.z > 1)
			{
				counterAmount = Mathf.Abs(steerInput);
			}
			
		}

		//steerAngle = Mathf.Lerp(steerAngle, steerInput * maxSteering, Time.deltaTime * cSteerSpeed);

		float assistSteeringFront = localVelocityFront.z > 1 ? Mathf.Clamp(chassisSteerAngleFront * steerFactorReciprocated, -tPeakAngle, tPeakAngle) : 0;
		float finalSteeringFront = Mathf.Clamp(assistSteeringFront * (/*(1 - Mathf.Abs(steerInput)) */ (1 - cSteerWeight)) + ((maxSteering) * steerInput), -maxSteering, maxSteering);
		//finalSteering = Mathf.Clamp(assistSteering * (1 - Mathf.Abs(steerInput)) + steerAngle, -maxSteering, maxSteering);
		//finalSteering = Mathf.Lerp(assistSteering, maxSteering * Mathf.Sign(steerInput), Mathf.Abs(steerInput));

		float assistSteeringRear = localVelocityRear.z > 1 ? Mathf.Clamp(chassisSteerAngleRear * steerFactorReciprocated, -tPeakAngle, tPeakAngle) : 0;
		float rearAssistAngle = assistSteeringRear * (/*(1 - Mathf.Abs(steerInput)) */ (1 - cSteerWeight));
		float rearSteeringAngle = ((maxSteering) * steerInput);
		float finalSteeringRear = 0;//Mathf.Clamp((maxSteering * steerInput) - (Mathf.Abs(rearAssistAngle) * Mathf.Sign(steerInput)), -maxSteering, maxSteering);
		if (cRearWheelsCanCounter)
		{
			finalSteeringRear = rearSteeringAngle - rearAssistAngle * (1 - cSteerWeight);
			finalSteeringRear = Mathf.Clamp(finalSteeringRear, -maxSteering, maxSteering);
		}
		else
		{
			if (Mathf.Abs(rearAssistAngle) > Mathf.Abs(rearSteeringAngle))
			{
				finalSteeringRear = 0;
			}
			else
			{
				finalSteeringRear = Mathf.Max(0, Mathf.Abs(rearSteeringAngle) - Mathf.Abs(rearAssistAngle)) * Mathf.Sign(rearSteeringAngle);
				//finalSteeringRear = Mathf.Lerp(rearSteeringAngle, (Mathf.Abs(rearSteeringAngle) - Mathf.Abs(rearAssistAngle)) * Mathf.Sign(rearSteeringAngle), Mathf.Abs(rearAssistAngle) / tPeakAngle);
				finalSteeringRear = Mathf.Clamp(finalSteeringRear, -maxSteering, maxSteering);
			}

			finalSteeringRear = Mathf.Lerp(finalSteeringRear, 0, counterAmount);
		}
		



		steerAngleFront = Mathf.Lerp(steerAngleFront, finalSteeringFront, Time.deltaTime * cSteerSpeed);
		steerAngleRear = Mathf.Lerp(steerAngleRear, finalSteeringRear, Time.deltaTime * cSteerSpeed);
		for (int i = 0; i < wheels.Length; i++)
		{
			if (wheels[i].canSteer)
			{
				if (wheels[i].isFront)
				{
					wheels[i].wheelPivot.localEulerAngles = new Vector3(0, steerAngleFront * cSteerFrontAmount, 0);
				}
				else
				{
					wheels[i].wheelPivot.localEulerAngles = new Vector3(0, -steerAngleRear * cSteerRearAmount, 0);
				}
			}
		}
	}

	void CalculateChassisStop()
	{
		bool recalc = false;

		float chassisDistDelta = Vector3.Distance(rBody.worldCenterOfMass, lastChassisPos);
		chassisDistDelta = Vector3.Distance(rBody.position, lastChassisPos);
		if (chassisDistDelta > tStopRadius)
		{
			//Vector3 chassisDir = -(rBody.worldCenterOfMass - lastChassisPos).normalized * tStopRadius;
			//lastChassisPos = rBody.worldCenterOfMass + chassisDir;

			Vector3 chassisDir = -(rBody.position - lastChassisPos).normalized * tStopRadius;
			lastChassisPos = rBody.position + chassisDir;

			recalc = true;
		}

		if(Quaternion.Angle(lastChassisRot, rBody.rotation) > tPeakAngle || recalc)
		{
			lastChassisRot = rBody.rotation;

			recalc = true;
		}

		if (recalc)
		{
			Vector3 side = lastChassisRot * Vector3.right;
			Vector3 up = lastChassisRot * Vector3.up;
			Vector3 fwd = lastChassisRot * Vector3.forward;
			for (int i = 0; i < wheels.Length; i++)
			{
				Vector3 newPos = wheels[i].wheelPivot.localPosition.x * side + wheels[i].wheelPivot.localPosition.y * up + wheels[i].wheelPivot.localPosition.z * fwd;
				wheels[i].stopPosWorld = FlattenLocalYPosition(wheels[i].wheelPivot, lastChassisPos + newPos);

				//HelperMath.DebugDrawCross(rBody.position + newPos, 0.25f, Color.magenta, Time.fixedDeltaTime);
			}
		}
	}

	void RaycastWheels()
	{
		float numGroundedWheels = 0;
		for (int i = 0; i < wheels.Length; i++)
		{
			RaycastHit[] rHits = Physics.RaycastAll(wheels[i].wheelCastPoint.position, -rBody.transform.up, sMaxDistance);

			if(rHits.Length == 0)
			{
				//Debug.DrawRay(wheels[i].wheelCollider.position, -wheels[i].wheelCollider.up * sMaxDistance, Color.red, 0, false);

				wheels[i].isGrounded = false;
				wheels[i].rHit.distance = sMaxDistance;
				continue;
			}

			wheels[i].rHit.distance = -1;
			for (int j = 0; j < rHits.Length; j++)
			{
				if (rHits[j].collider != chassisCollider && !rHits[j].collider.isTrigger)
				{
					if(wheels[i].rHit.distance == -1)
					{
						wheels[i].rHit = rHits[j];
					}
					else if (rHits[j].distance < wheels[i].rHit.distance)
					{
						wheels[i].rHit = rHits[j];
					}
				}
			}

			wheels[i].isGrounded = wheels[i].rHit.distance != -1;

			if (wheels[i].isGrounded)
			{
				numGroundedWheels++;
				//Debug.DrawLine(wheels[i].rHit.point, wheels[i].wheelCollider.position, Color.green, 0, false);
			}

		}

		isGrounded = numGroundedWheels > Mathf.CeilToInt(wheels.Length * 0.5f);
	}

	void ShiftWorldPoses(int i, Vector3 newPos)
	{
		wheels[i].lastWorldPoses[0] = newPos;

		Vector3 dir01 = (wheels[i].lastWorldPoses[0] - wheels[i].lastWorldPoses[1]).normalized;
		dir01 = Vector3.Slerp(dir01, -transform.up, stationaryMult > 0 ? Time.fixedDeltaTime * 5 : 0);
		Vector3 pos1 = wheels[i].lastWorldPoses[0] + -dir01 * tSlipTrail * 0.33f;
		wheels[i].lastWorldPoses[1] = pos1;

		Vector3 dir12 = (wheels[i].lastWorldPoses[1] - wheels[i].lastWorldPoses[2]).normalized;
		dir12 = Vector3.Slerp(dir12, -transform.up, stationaryMult > 0 ? Time.fixedDeltaTime * 5 : 0);
		Vector3 pos2 = wheels[i].lastWorldPoses[1] + -dir12 * tSlipTrail * 0.33f;
		wheels[i].lastWorldPoses[2] = pos2;

		Vector3 dir23 = (wheels[i].lastWorldPoses[2] - wheels[i].lastWorldPoses[3]).normalized;
		dir23 = Vector3.Slerp(dir23, -transform.up, stationaryMult > 0 ? Time.fixedDeltaTime * 5 : 0);
		Vector3 pos3 = wheels[i].lastWorldPoses[2] + -dir23 * tSlipTrail * 0.33f;
		wheels[i].lastWorldPoses[3] = pos3;


		//wheels[i].lastWorldPoses[0] = FlattenLocalYPosition(wheels[i].wheelCollider, wheels[i].lastWorldPoses[0]);
		//wheels[i].lastWorldPoses[1] = FlattenLocalYPosition(wheels[i].wheelCollider, wheels[i].lastWorldPoses[1]);
		//wheels[i].lastWorldPoses[2] = FlattenLocalYPosition(wheels[i].wheelCollider, wheels[i].lastWorldPoses[2]);
		//wheels[i].lastWorldPoses[3] = FlattenLocalYPosition(wheels[i].wheelCollider, wheels[i].lastWorldPoses[3]);


		//HelperMath.DebugDrawCross(wheels[i].lastWorldPoses[0], 0.03f, Color.yellow, 0);
		//HelperMath.DebugDrawCross(wheels[i].lastWorldPoses[1], 0.03f, Color.Lerp(Color.yellow, Color.red, 0.333f), 0);
		//HelperMath.DebugDrawCross(wheels[i].lastWorldPoses[2], 0.03f, Color.Lerp(Color.yellow, Color.red, 0.666f), 0);
		//HelperMath.DebugDrawCross(wheels[i].lastWorldPoses[3], 0.03f, Color.red, 0);
	}

	void CalculateFrictionVectors(int i)
	{
		////WHEEL TRAIL////

		Vector3 vel01 = wheels[i].lastWorldPoses[0] - wheels[i].lastWorldPoses[1];
		Vector3 vel12 = wheels[i].lastWorldPoses[1] - wheels[i].lastWorldPoses[2];
		Vector3 vel23 = wheels[i].lastWorldPoses[2] - wheels[i].lastWorldPoses[3];

		Vector3 avgVel = (vel01 + vel12 + vel23);

		////TRAILING STOP POSITION////

		//Vector3 avgPos = (wheels[i].lastWorldPoses[1] + wheels[i].lastWorldPoses[2] + wheels[i].lastWorldPoses[3]) * 0.33333f;
		Vector3 flatStopPos = FlattenLocalYPosition(wheels[i].wheelPivot, wheels[i].stopPosWorld);
		Vector3 stopDir = Vector3.ProjectOnPlane((wheels[i].wheelPivot.position - flatStopPos), wheels[i].rHit.normal).normalized;

		float stopPosDist = Vector3.Distance(wheels[i].wheelPivot.position, flatStopPos);//Vector3.Distance(Vector3.zero, wheels[i].wheelCollider.InverseTransformPoint(flatStopPos));
		float springDamp = -((wheels[i].lastStopPosDist / tStopRadius) - (stopPosDist / tStopRadius));
		springDamp = Mathf.Clamp(springDamp * tStopDamping, -1, 1);
		wheels[i].lastStopPosDist = stopPosDist;

		float avgStopMult = Mathf.Clamp01(stopPosDist / (tStopRadius));
		Vector3 finalStopVector = stopDir * Mathf.Clamp(avgStopMult + springDamp, -1, 1);


		wheels[i].stopPosVector = finalStopVector;
		wheels[i].posVelocityWorld = FlattenLocalYPosition(wheels[i].wheelPivot, avgVel);


		//HelperMath.DebugDrawCross(avgPos, 0.1f, Color.blue, 0);

		HelperMath.DebugDrawCross(wheels[i].stopPosWorld, 0.1f, Color.blue, 0);
		HelperMath.DebugDrawCross(wheels[i].wheelPivot.position, 0.1f, Color.magenta, 0);
		//Debug.DrawRay(wheels[i].rHit.point, avgVel, Color.green, 0, false);
		//Debug.DrawRay(wheels[i].rHit.point, wheels[i].stopPosVector, Color.yellow, 0, false);
	}

	void RunSuspension(int i, float gripBias)
	{
		////USEFUL VALUES////
		float springLerp02 = 0;
		if (wheels[i].rHit.distance > sRestDistance)
		{
			springLerp02 = Mathf.InverseLerp(sMaxDistance, sRestDistance, wheels[i].rHit.distance);
		}
		else
		{
			springLerp02 = 1 + Mathf.InverseLerp(sRestDistance, 0, wheels[i].rHit.distance);
		}
		Debug.DrawRay(wheels[i].rHit.point + Vector3.forward * 0.1f, Vector3.up * springLerp02, Color.green, 0, false);

		////CALCULATE SPRING FORCE////

		//float springMult = Mathf.Clamp01(wheels[i].rHit.distance / sMaxDistance);
		float springForce = 0;// springMult * 10 * sSpring;
		if (springLerp02 >= 1)
		{
			springForce = Mathf.Lerp(1, sSpring, springLerp02 - 1);
		}
		else
		{
			springForce = Mathf.Lerp(0, 1, springLerp02);
		}
		springForce = (Mathf.Clamp(springForce, 0, sSpring) * 10) / wheels.Length;

		float wheelVelocity = -((wheels[i].rHit.distance - wheels[i].prevSuspDist) / sMaxDistance) / Time.fixedDeltaTime;
		float springDamping = wheelVelocity * (wheelVelocity < 0 ? sReboundDamping : sCompressionDamping);

		float springResult = Mathf.Max(0, springForce + springDamping);

		//Debug.DrawRay(wheels[i].rHit.point, wheels[i].rHit.normal * springResult, Color.blue, 0, false);
		rBody.AddForceAtPosition(wheels[i].rHit.normal * springResult, wheels[i].rHit.point, ForceMode.Acceleration);

		////CALCULATE GRIP MODIFIER////
		//float restDist = 1 - (1f / (sSpring * wheels.Length));
		//restDist *= sMaxDistance;

		float gripMult = 0;
		//if(wheels[i].rHit.distance > sRestDistance)
		//{
		//	gripMult = Mathf.InverseLerp(sMaxDistance, sRestDistance, wheels[i].rHit.distance);
		//}
		//else
		//{
		//	gripMult = 1 + Mathf.InverseLerp(sRestDistance, 0, wheels[i].rHit.distance);
		//}

		gripMult = springLerp02;
		wheels[i].gripMult = gripMult * gripBias;

		////DEBUGGING////

		//Color gripColor = Color.magenta;
		//if (gripMult >= 1)
		//{
		//	gripColor = Color.Lerp(Color.black, Color.white, Mathf.Clamp01(gripMult - 1));
		//}
		//else
		//{
		//	gripColor = Color.Lerp(Color.red, Color.black, Mathf.Clamp01(gripMult));
		//}
		//Debug.DrawRay(wheels[i].rHit.point, wheels[i].rHit.normal * gripMult, gripColor, 0, false);
	}

	float CalculateDriftAmount(int index, float driftAngle, float wheelForce, float friction)
	{
		/*
		float usePredict = Mathf.Clamp01((longForce / friction));
		float driftAngleTarget = Mathf.Max(0, driftAngle, tPeakAngle * (tStaticFriction / mAcceleration));
		driftAngleTarget = Mathf.Lerp(90, (90 - tPeakAngle) * 0.5f, usePredict);
		driftAngle = Mathf.Lerp(90 - tPeakAngle, 0, usePredict);

		driftUse = (Mathf.Abs(gripSlipAngle) - driftAngle) / Mathf.Clamp(driftAngleTarget, 0, 90);
		driftUse = Mathf.Clamp01(Mathf.InverseLerp(driftAngle, driftAngleTarget, Mathf.Abs(gripSlipAngle)));
		*/

		float usePredict = Mathf.Clamp01((wheelForce / friction));
		//usePredict = Mathf.Clamp(usePredict, 0, tDriftAbility);
		float driftAngleOuter = Mathf.Lerp(90, (90 - tPeakAngle) * 0.5f, usePredict);
		float driftAngleInner = Mathf.Lerp((90 - tPeakAngle) * 0.5f, 0, usePredict);

		driftAngleOuter = Mathf.Lerp(90, tPeakAngle, usePredict);
		driftAngleInner = Mathf.Lerp(tPeakAngle, 0, usePredict);

		//driftUse = (Mathf.Abs(driftAngle) - driftAngleInner) / Mathf.Clamp(driftAngleOuter, 0, 90);
		float driftUse = Mathf.Clamp01(Mathf.InverseLerp(driftAngleInner, driftAngleOuter, Mathf.Abs(driftAngle)));
		driftUse = Mathf.Pow(driftUse, 1) * usePredict;
		driftUse = Mathf.Clamp(driftUse, 0, tDriftAbility * usePredict);
		driftUse = Mathf.Clamp01(Mathf.Max(driftUse, (wheelForce / friction) - 1));

		Color invis = new Color(1, 0, 0, 0.5f);
		HelperMath.DebugDrawCone(wheels[index].wheelPivot, Vector3.zero, driftAngleInner, 0.25f, Color.Lerp(invis, Color.red, usePredict), 0);
		HelperMath.DebugDrawCone(wheels[index].wheelPivot, Vector3.zero, driftAngleOuter, 0.35f, Color.Lerp(invis, Color.red, driftUse * usePredict), 0);

		return driftUse;
	}

	void RunFriction(int i, float accelForce, float brakeForce, float rollHeight)
	{
		//Vector3 velocity = rBody.GetPointVelocity(wheels[i].rHit.point);
		//Vector3 localVelocity = wheels[i].wheelCollider.InverseTransformVector(velocity).normalized;

		Vector3 posVelocityLocal = wheels[i].wheelPivot.InverseTransformVector(wheels[i].posVelocityWorld);
		Vector3 vel = wheels[i].wheelPivot.InverseTransformVector(rBody.GetPointVelocity(wheels[i].rHit.point));

		float friction = wheels[i].gripMult * tMaxGrip;

		float totLongForce = Mathf.Abs(accelForce) - brakeForce;
		float wheelForce = 0;
		float driftForce = 0;
		if(totLongForce <= 0)
		{
			wheelForce = Mathf.Abs(totLongForce) * -Mathf.Sign(vel.z);
		}
		else
		{
			wheelForce = totLongForce * Mathf.Sign(accelForce);
			driftForce = totLongForce;
		}
		bool isLocked = brakeForce > friction;


		float slipAngle = Mathf.Atan2(-posVelocityLocal.x, Mathf.Abs(posVelocityLocal.z)) * Mathf.Rad2Deg;
		float steerForce = (Mathf.Clamp(slipAngle / tPeakAngle, -1, 1));

		float driftUse = CalculateDriftAmount(i, slipAngle, driftForce, friction);
		wheelForce = Mathf.Lerp(wheelForce, Mathf.Sign(wheelForce) * mAcceleration, driftUse);

		wheels[i].spinAmount = driftUse * Mathf.Sign(accelForce);
		wheels[i].isLocked = isLocked;

		float lateralMult = 1 - driftUse * 0.5f;

		float counterSteerMult = wheels[i].canSteer && wheels[i].isFront ? Mathf.Lerp(1, 0.5f, counterAmount) : 1;

		Vector3 steerForceVector = Vector3.ProjectOnPlane(wheels[i].wheelPivot.right, wheels[i].rHit.normal).normalized;
		Vector3 finalSteerForce = steerForceVector * steerForce * friction * lateralMult * counterSteerMult;

		float longSlip = Mathf.Clamp01((Mathf.Abs(wheelForce) / friction) - 1);
		wheelForce *= 1 - ( (Mathf.Abs(slipAngle) / 90) * (1 - driftUse) );

		Vector3 longitudinalForce = Vector3.ProjectOnPlane(wheels[i].wheelPivot.forward, wheels[i].rHit.normal).normalized;
		longitudinalForce *= Mathf.Clamp(wheelForce, -friction, friction);

		//finalForce = Vector3.ClampMagnitude(lateralForce * driftMod + longitudinalForce, friction * (2 - driftUse));


		Vector3 finalStopForce = -wheels[i].stopPosVector * friction;
		Vector3 finalForce = finalSteerForce + longitudinalForce;
		if(isLocked)
		{
			//Vector3.LerpUnclamped(finalSteerForce, finalStopForce, stationaryMult)
			if(handbrakeInput && !wheels[i].isFront && stationaryMult == 0)
			{
				finalStopForce = -rBody.GetPointVelocity(wheels[i].wheelCastPoint.position);
				finalStopForce = Vector3.ProjectOnPlane(finalStopForce, wheels[i].rHit.normal).normalized * friction;
			}
			finalForce = finalStopForce;
			driftUse = 0;
		}
		else
		{
			finalForce = Vector3.LerpUnclamped(finalForce, finalStopForce, stationaryMult);
		}

		//finalForce *= (tMaxGrip * wheels[i].gripMult * 10) / wheels.Length;
		finalForce *= 10f / wheels.Length;
		finalForce = Vector3.ClampMagnitude(finalForce, friction * (2 - driftUse));

		Color debugFrictionColor = Color.Lerp(new Color(1, 1, 1, 0.25f), new Color(1, 0, 0, 0.25f), Mathf.Ceil(driftUse));
		Debug.DrawRay(wheels[i].rHit.point, finalForce, debugFrictionColor, 0, false);
		rBody.AddForceAtPosition(finalForce, wheels[i].rHit.point + transform.up * rollHeight, ForceMode.Acceleration);


		if(wheels[i].rHit.rigidbody)
		{
			float forceFric = 1;
			if (wheels[i].rHit.collider.sharedMaterial)
			{
				forceFric = Mathf.Lerp(wheels[i].rHit.collider.sharedMaterial.staticFriction, wheels[i].rHit.collider.sharedMaterial.dynamicFriction, driftUse);
			}

			Vector3 reactForce = wheels[i].wheelPivot.InverseTransformVector(finalForce);
			reactForce.x = 0;
			reactForce = wheels[i].wheelPivot.TransformVector(reactForce);

			//wheels[i].rHit.rigidbody.AddForceAtPosition(-reactForce * (wheels[i].rHit.rigidbody.mass / rBody.mass), wheels[i].rHit.point, ForceMode.Acceleration)
			wheels[i].rHit.rigidbody.AddForceAtPosition(Vector3.ClampMagnitude(-finalForce, 1) * forceFric * Mathf.Min(10, rBody.mass / wheels[i].rHit.rigidbody.mass), wheels[i].rHit.point, ForceMode.Acceleration);
		}
		//rBody.AddForce(transform.forward * (forwardInput - reverseInput) * 10, ForceMode.Acceleration);
	}

	Vector3 FlattenLocalYPosition(Transform trans, Vector3 pos)
	{
		Vector3 local = trans.InverseTransformPoint(pos);
		local.y = 0;

		Vector3 result = trans.TransformPoint(local);

		return result;
	}

}
