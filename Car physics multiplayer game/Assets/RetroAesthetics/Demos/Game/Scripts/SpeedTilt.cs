using UnityEngine;

namespace RetroAesthetics.Demos {
	public class SpeedTilt : MonoBehaviour {
		public float minimumLocalPositionY = 1f;
		public float minimumLocalRotationX = 0f;
		public float maximumFOV = 80f;
		public float minSpeed = 0.5f;
		public float maxSpeed = 1f;

		private float _maxPositionY;
		private float _maxRotationX;
		private Vector3 _lastPosition;
		private float _distance;
		private Vector3 _localPosition;
		private Vector2 _localRotationYZ;
		private Camera _camera;
		private float _minFOV;

		void Start () {
			_maxPositionY = transform.localPosition.y;
			_lastPosition = transform.position;
			_localPosition = transform.localPosition;
			_localRotationYZ = new Vector2(transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
			_maxRotationX = transform.localRotation.eulerAngles.x;
			
			_camera = gameObject.GetComponentInChildren<Camera>();
			if (_camera == null) {
				this.enabled = false;
				return;
			}
			_minFOV = _camera.fieldOfView;
		}
		
		void FixedUpdate () {
			Vector3 d = _lastPosition - transform.position;
			
			// Skip frame if falling down.
			if (Mathf.Abs(d.y) < Mathf.Max(Mathf.Abs(d.x), Mathf.Abs(d.y))) {
				_distance = d.magnitude;
				if (_distance > minSpeed && _distance < maxSpeed) {
					float t = Mathf.Clamp01((_distance - minSpeed) / (maxSpeed - minSpeed));
					_localPosition.y = Mathf.SmoothStep(_maxPositionY, minimumLocalPositionY, t);
					transform.localPosition = _localPosition;
					float rotX = Mathf.SmoothStep(_maxRotationX, minimumLocalRotationX, t);
					transform.localRotation = Quaternion.Euler(rotX, _localRotationYZ.x, _localRotationYZ.y);
					_camera.fieldOfView = Mathf.SmoothStep(_minFOV, maximumFOV, t);
				}
			}
			
			_lastPosition = transform.position;
		}
	}

}
