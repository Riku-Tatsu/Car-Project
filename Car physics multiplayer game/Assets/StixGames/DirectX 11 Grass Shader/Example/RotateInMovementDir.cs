using UnityEngine;

namespace StixGames.GrassShader
{
	public class RotateInMovementDir : MonoBehaviour
	{
		public float movementThreshold = 0.01f;

		private Vector3 lastPosition;

		void Start()
		{
			lastPosition = transform.position;
		}

		void LateUpdate()
		{
			Vector3 movementDir = transform.position - lastPosition;
			movementDir.y = 0;

			if (movementDir.magnitude > movementThreshold)
			{
				transform.rotation = Quaternion.LookRotation(movementDir.normalized);
			}

			lastPosition = transform.position;
		}
	}
}
