using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	[RequireComponent(typeof(Camera))]
	public class MultiCameraFocus : MonoBehaviour
	{
		public float Velocity;
		public float SmoothTime = 0.5f;
		public float MinHeight;
		public float MaxHeight;
		public Vector3 Offset;
		public List<Transform> Targets = new List<Transform>();

		public float HorizontalSize
		{
			set
			{
				var radAngle = camera.fieldOfView * Mathf.Deg2Rad;
				var hAngleTan = Mathf.Tan(radAngle / 2) * camera.aspect;
				MinHeight = hAngleTan / 2 * value;
			}
		}

		private Vector3 velocity;
		private Bounds bounds;
		private new Camera camera;
		private Vector3 zoomOffset;

		private void Awake		()
		{
			camera = this.GetComponent<Camera>();
		}

		private void LateUpdate()
		{
			if (Targets.Count == 0)
			{
				return;
			}

			CalculateBounds();
			Zoom();
			Move();
		}

		private void CalculateBounds()
		{
			bounds = new Bounds(Targets[0].position, Vector3.zero);

			if (Targets.Count == 1)
			{
				return;
			}

			for (int i = 1; i < Targets.Count; i++)
			{
				bounds.Encapsulate(Targets[i].position);
			}
		}

		private void Move()
		{
			var newPos = bounds.center + Offset + zoomOffset;
			newPos.x = 0;
			transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, SmoothTime);
		}

		private void Zoom()
		{
			var half = bounds.size.z / 2;
			var height = Mathf.Clamp(half * 2.6f, MinHeight, MaxHeight);
			var dist = height / 2.6f;
			this.zoomOffset = new Vector3(0, height, -dist);
		}
	}
}