using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperMath : MonoBehaviour {

	////MATH HELPERS////

	public static float ReturnGreatest(float a, float b)
	{
		float absA = Mathf.Abs(a);
		float absB = Mathf.Abs(b);

		return absA > absB ? a : b;
	}

	////DEBUGGING HELPERS////

	public static void DebugDrawCross(Vector3 pos, float size, Color color, float time)
	{
		Debug.DrawLine(pos - Vector3.forward * size, pos + Vector3.forward * size, color, time, false);
		Debug.DrawLine(pos - Vector3.right * size, pos + Vector3.right * size, color, time, false);
		Debug.DrawLine(pos - Vector3.up * size, pos + Vector3.up * size, color, time, false);
	}

	public static void DebugDrawCross(Transform trans, float size, Color color, float time)
	{
		Debug.DrawLine(trans.position - trans.forward * size, trans.position + trans.forward * size, color, time, false);
		Debug.DrawLine(trans.position - trans.right * size, trans.position + trans.right * size, color, time, false);
		Debug.DrawLine(trans.position - trans.up * size, trans.position + trans.up * size, color, time, false);
	}

	public static void DebugDrawCross(Transform trans, Vector3 offset, float size, Color color, float time)
	{
		Vector3 offsetPos = trans.position + trans.TransformVector(offset);
		Debug.DrawLine(offsetPos - trans.forward * size, offsetPos + trans.forward * size, color, time, false);
		Debug.DrawLine(offsetPos - trans.right * size, offsetPos + trans.right * size, color, time, false);
		Debug.DrawLine(offsetPos - trans.up * size, offsetPos + trans.up * size, color, time, false);
	}

	public static void DebugDrawCone(Transform trans, Vector3 offset, float angle, float size, Color color, float time)
	{
		Vector3 angleLeft = Quaternion.AngleAxis(-angle, trans.up) * trans.forward;
		Vector3 angleRight = Quaternion.AngleAxis(angle, trans.up) * trans.forward;

		Debug.DrawRay(trans.TransformPoint(Vector3.Scale(Vector3.one, offset)), angleLeft * size, color, time, false);
		Debug.DrawRay(trans.TransformPoint(Vector3.Scale(Vector3.one, offset)), angleRight * size, color, time, false);
	}

}
