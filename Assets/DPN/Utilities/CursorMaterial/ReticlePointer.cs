/************************************************************************************

Copyright   :   Copyright 2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;

namespace dpn
{
	[RequireComponent(typeof(Renderer))]
    /// <summary>
    /// Draws a circular reticle in front of any object that the user points at.
    /// The circle dilates if the object is clickable.
    /// </summary>
    public class ReticlePointer : DpnBasePointer
	{
        /// <summary>
        /// Number of segments making the reticle circle.
        /// </summary>
        public int reticleSegments = 20;

        /// <summary>
        /// Growth speed multiplier for the reticle/
        /// </summary>
        public float reticleGrowthSpeed = 8.0f;

        /// <summary>
        /// Private members
        /// </summary>
        private Material materialComp;

        /// <summary>
        /// Current inner angle of the reticle (in degrees).
        /// </summary>
        private float reticleInnerAngle = 0.0f;
        /// <summary>
        /// Current outer angle of the reticle (in degrees).
        /// </summary>
        private float reticleOuterAngle = 0.5f;
        /// <summary>
        /// Current distance of the reticle (in meters).
        /// </summary>
        private Vector3 reticleLocalPosition = new Vector3(0, 0, 0);

        /// <summary>
        /// Minimum inner angle of the reticle (in degrees).
        /// </summary>
        private const float kReticleMinInnerAngle = 0.0f;
        /// <summary>
        /// Minimum outer angle of the reticle (in degrees).
        /// </summary>
        private const float kReticleMinOuterAngle = 0.5f;
        /// <summary>
        /// Angle at which to expand the reticle when intersecting with an object
		/// (in degrees).
        /// </summary>
        public float kReticleGrowthAngle = 0.5f;

        /// <summary>
        /// Minimum distance of the reticle (in meters).
        /// </summary>
        private const float kReticleDistanceMin = 0.45f;
        /// <summary>
        /// Maximum distance of the reticle (in meters).
        /// </summary>
        private float kReticleDistanceMax = 100.0f;

        /// <summary>
        /// Current inner and outer diameters of the reticle,
		/// before distance multiplication.
        /// </summary>
        private float reticleInnerDiameter = 0.0f;
		private float reticleOuterDiameter = 0.0f;

		void Awake()
		{
			kReticleDistanceMax = DpnCameraRig._instance._center_eye.farClipPlane - 1.0f;
			SetTitledAngle(0);
		}

		protected void Start()
		{
			CreateReticleVertices();

			materialComp = gameObject.GetComponent<Renderer>().material;
		}

		void Update()
		{
			UpdateDiameters();
		}

        /// <summary>
        /// This is called when the 'BaseInputModule' system should be enabled.
        /// </summary>
        public override void OnInputModuleEnabled() { }

        /// <summary>
        /// This is called when the 'BaseInputModule' system should be disabled.
        /// </summary>
        public override void OnInputModuleDisabled() { }

        /// <summary>
        /// Called when the user is pointing at valid GameObject. This can be a 3D
		/// or UI element.
		///
		/// The targetObject is the object the user is pointing at.
		/// The intersectionPosition is where the ray intersected with the targetObject.
		/// The intersectionRay is the ray that was cast to determine the intersection.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="intersectionPosition">The intersection position.</param>
        /// <param name="intersectionRay">The intersection ray.</param>
        /// <param name="isInteractive">if set to <c>true</c> [is interactive].</param>
        public override void OnPointerEnter(GameObject targetObject, Vector3 intersectionPosition,
		   Ray intersectionRay, bool isInteractive)
		{
			SetPointerTarget(intersectionPosition, isInteractive);
		}

        /// <summary>
        /// Called every frame the user is still pointing at a valid GameObject. This
		/// can be a 3D or UI element.
		///
		/// The targetObject is the object the user is pointing at.
		/// The intersectionPosition is where the ray intersected with the targetObject.
		/// The intersectionRay is the ray that was cast to determine the intersection.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="intersectionPosition">The intersection position.</param>
        /// <param name="intersectionRay">The intersection ray.</param>
        /// <param name="isInteractive">if set to <c>true</c> [is interactive].</param>
        public override void OnPointerHover(GameObject targetObject, Vector3 intersectionPosition,
			Ray intersectionRay, bool isInteractive)
		{
			SetPointerTarget(intersectionPosition, isInteractive);
		}

        /// <summary>
        /// Called when the user's look no longer intersects an object previously
		/// intersected with a ray projected from the camera.
		/// This is also called just before **OnInputModuleDisabled** and may have have any of
		/// the values set as **null**.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        public override void OnPointerExit(GameObject targetObject)
		{
			reticleLocalPosition = new Vector3(0, 0, 0);
			reticleInnerAngle = kReticleMinInnerAngle;
			reticleOuterAngle = kReticleMinOuterAngle;
			transform.localPosition = new Vector3(0, offsetY, distanceMax);
		}

		override public void SetTitledAngle(float degree)
		{
			base.SetTitledAngle(degree);

			if (float.Equals(tiltedAngle % 180.0f, 0.0f))
			{
				distanceMax = kReticleDistanceMax;
				offsetY = 0.0f;
			}
			else
			{
				distanceMax = kReticleDistanceMax * 2.0f - kReticleDistanceMax / Mathf.Cos(Mathf.Deg2Rad * tiltedAngle);
				offsetY = Mathf.Tan(Mathf.Deg2Rad * tiltedAngle) * distanceMax;
			}
			gameObject.transform.Rotate(new Vector3(-tiltedAngle, 0,0));
		}

		float distanceMax = 0.0f;
		float offsetY = 0.0f;

        /// <summary>
        /// Called when a trigger event is initiated. This is practically when
		/// the user begins pressing the trigger.
        /// </summary>
        public override void OnPointerClickDown() { }

        /// <summary>
        /// Called when a trigger event is finished. This is practically when
		/// the user releases the trigger.
        /// </summary>
        public override void OnPointerClickUp() { }

		public override float GetMaxPointerDistance()
		{
			return kReticleDistanceMax;
		}

		public override void GetPointerRadius(out float innerRadius, out float outerRadius)
		{
			float min_inner_angle_radians = Mathf.Deg2Rad * kReticleMinInnerAngle;
			float max_inner_angle_radians = Mathf.Deg2Rad * (kReticleMinInnerAngle + kReticleGrowthAngle);

			innerRadius = 2.0f * Mathf.Tan(min_inner_angle_radians);
			outerRadius = 2.0f * Mathf.Tan(max_inner_angle_radians);
		}

		public override Transform GetPointerTransform()
		{
			return this.transform.parent.transform;
		}

		private void CreateReticleVertices()
		{
			Mesh mesh = new Mesh();
			gameObject.AddComponent<MeshFilter>();
			GetComponent<MeshFilter>().mesh = mesh;

			int segments_count = reticleSegments;
			int vertex_count = (segments_count + 1) * 2;

			#region Vertices

			Vector3[] vertices = new Vector3[vertex_count];

			const float kTwoPi = Mathf.PI * 2.0f;
			int vi = 0;
			for (int si = 0; si <= segments_count; ++si)
			{
				// Add two vertices for every circle segment: one at the beginning of the
				// prism, and one at the end of the prism.
				float angle = (float)si / (float)(segments_count) * kTwoPi;

				float x = Mathf.Sin(angle) * 0.5f;
				float y = Mathf.Cos(angle) * 0.5f;

				vertices[vi++] = new Vector3(x, y, 0.0f); // Outer vertex.
				vertices[vi++] = new Vector3(x, y, 1.0f); // Inner vertex.
			}
			#endregion

			#region Triangles
			int indices_count = (segments_count + 1) * 3 * 2;
			int[] indices = new int[indices_count];

			int vert = 0;
			int idx = 0;
			for (int si = 0; si < segments_count; ++si)
			{
				indices[idx++] = vert + 1;
				indices[idx++] = vert;
				indices[idx++] = vert + 2;

				indices[idx++] = vert + 1;
				indices[idx++] = vert + 2;
				indices[idx++] = vert + 3;

				vert += 2;
			}
			#endregion

			mesh.vertices = vertices;
			mesh.triangles = indices;
			mesh.RecalculateBounds();
			;
		}

		private void UpdateDiameters()
		{
			if (reticleInnerAngle < kReticleMinInnerAngle)
			{
				reticleInnerAngle = kReticleMinInnerAngle;
			}

			if (reticleOuterAngle < kReticleMinOuterAngle)
			{
				reticleOuterAngle = kReticleMinOuterAngle;
			}

			float inner_half_angle_radians = Mathf.Deg2Rad * reticleInnerAngle * 0.5f;
			float outer_half_angle_radians = Mathf.Deg2Rad * reticleOuterAngle * 0.5f;

			float inner_diameter = 2.0f * Mathf.Tan(inner_half_angle_radians);
			float outer_diameter = 2.0f * Mathf.Tan(outer_half_angle_radians);

			reticleInnerDiameter =
				Mathf.Lerp(reticleInnerDiameter, inner_diameter, Time.deltaTime * reticleGrowthSpeed);
			reticleOuterDiameter =
				Mathf.Lerp(reticleOuterDiameter, outer_diameter, Time.deltaTime * reticleGrowthSpeed);

			materialComp.SetFloat("_InnerDiameter", reticleInnerDiameter * transform.localPosition.z);
			materialComp.SetFloat("_OuterDiameter", reticleOuterDiameter * transform.localPosition.z);
		}

		private void SetPointerTarget(Vector3 target, bool interactive)
		{
			transform.position = target;

			if (interactive)
			{
				reticleInnerAngle = kReticleMinInnerAngle + kReticleGrowthAngle;
				reticleOuterAngle = kReticleMinOuterAngle + kReticleGrowthAngle;
			}
			else
			{
				reticleInnerAngle = kReticleMinInnerAngle;
				reticleOuterAngle = kReticleMinOuterAngle;
			}
		}
        protected override void OnDisable()
        {
            OnPointerExit(null);
            base.OnDisable();
        }
	}
}
