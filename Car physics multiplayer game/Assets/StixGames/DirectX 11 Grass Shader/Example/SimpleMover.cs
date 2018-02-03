using UnityEngine;
using System.Collections;

namespace StixGames.GrassShader
{
	public class SimpleMover : MonoBehaviour
	{
		public float speed = 5;
		public float height = 0.5f;

		public LayerMask rayCastLayers;

		// Update is called once per frame
		void Update ()
		{
			transform.position += speed * Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime;
			transform.position += speed * Vector3.forward * Input.GetAxis("Vertical") * Time.deltaTime;

			RaycastHit hit;
			if (Physics.Raycast(new Ray(transform.position + Vector3.up * 1000, Vector3.down), out hit, Mathf.Infinity, rayCastLayers))
			{
				var pos = transform.position;
				pos.y = hit.point.y + height;
				transform.position = pos;
			}
			else
			{
				var pos = transform.position;
				pos.y = height;
				transform.position = pos;
			}
		}
	}
}
