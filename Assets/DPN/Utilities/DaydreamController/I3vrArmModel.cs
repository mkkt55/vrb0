/*
 * Copyright (C) 2017 3ivr. All rights reserved.
 *
 * Author: Lucas(Wu Pengcheng)
 * Date  : 2017/06/19 08:08
 */

using UnityEngine;
using dpn;

namespace i3vr
{
    /// <summary>
    /// The I3vrArmModel is a standard interface to interact with a scene with the controller.
    /// It is responsible for:
    /// -  Determining the orientation and location of the controller.
    /// -  Predict the location of the shoulder, elbow, wrist, and pointer.
    ///
    /// There should only be one instance in the scene, and it should be attached
    /// to the I3vrController.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(DpnDaydreamController))]
    public class I3vrArmModel : MonoBehaviour
    {
        /// <summary>
        /// Initial relative location of the shoulder (meters).
        /// </summary>
        private static readonly Vector3 DEFAULT_SHOULDER_RIGHT = new Vector3(0.19f, -0.19f, -0.03f);
        private static readonly Vector3 DEFAULT_SHOULDER_LEFT = new Vector3(-0.19f, -0.19f, -0.03f);

        /// <summary>
        /// The range of movement from the elbow position due to accelerometer (meters).
        /// </summary>
        private static readonly Vector3 ELBOW_MIN_RANGE = new Vector3(-0.05f, -0.1f, 0.0f);
        private static readonly Vector3 ELBOW_MAX_RANGE = new Vector3(0.05f, 0.1f, 0.2f);

        /// <summary>
        /// Offset of the laser pointer origin relative to the wrist (meters)
        /// </summary>
        private static readonly Vector3 POINTER_OFFSET = new Vector3(0.0f, -0.009f, 0.099f);

        /// <summary>
        /// Rest position parameters for arm model (meters).
        /// </summary>
        private static readonly Vector3 ELBOW_POSITION = new Vector3(0.195f, -0.5f, -0.075f);
        private static readonly Vector3 ELBOW_POSITION_LEFT = new Vector3(-0.195f, -0.5f, -0.075f);
        private static readonly Vector3 WRIST_POSITION = new Vector3(0.0f, 0.0f, 0.25f);
        private static readonly Vector3 ARM_EXTENSION_OFFSET = new Vector3(-0.13f, 0.14f, 0.08f);
        private static readonly Vector3 ARM_EXTENSION_OFFSET_LEFT = new Vector3(0.13f, 0.14f, 0.08f);

        /// <summary>
        /// Strength of the acceleration filter (unitless).
        /// </summary>
        private const float GRAVITY_CALIB_STRENGTH = 0.999f;

        /// <summary>
        /// Strength of the velocity suppression (unitless).
        /// </summary>
        private const float VELOCITY_FILTER_SUPPRESS = 0.99f;

        /// <summary>
        /// Strength of the velocity suppression during low acceleration (unitless).
        /// </summary>
        private const float LOW_ACCEL_VELOCITY_SUPPRESS = 0.9f;

        /// <summary>
        /// Strength of the acceleration suppression during low velocity (unitless).
        /// </summary>
        private const float LOW_VELOCITY_ACCEL_SUPPRESS = 0.5f;

        /// <summary>
        /// The minimum allowable accelerometer reading before zeroing (m/s^2).
        /// </summary>
        private const float MIN_ACCEL = 1.0f;

        /// <summary>
        /// The expected force of gravity (m/s^2).
        /// </summary>
        private const float GRAVITY_FORCE = 9.807f;

        /// <summary>
        /// Amount of normalized alpha transparency to change per second.
        /// </summary>
        private const float DELTA_ALPHA = 4.0f;

        /// <summary>
        /// Angle ranges the for arm extension offset to start and end (degrees).
        /// </summary>
        private const float MIN_EXTENSION_ANGLE = 7.0f;
        private const float MAX_EXTENSION_ANGLE = 60.0f;

        /// <summary>
        /// Increases elbow bending as the controller moves up (unitless).
        /// </summary>
        private const float EXTENSION_WEIGHT = 0.4f;

        /// <summary>
        /// Offset of the elbow due to the accelerometer.
        /// </summary>
        private Vector3 elbowOffset;

        /// <summary>
        /// Forward direction of the arm model.
        /// </summary>
        private Vector3 torsoDirection;

        /// <summary>
        /// Filtered velocity of the controller.
        /// </summary>
        private Vector3 filteredVelocity;

        /// <summary>
        /// Filtered acceleration of the controller.
        /// </summary>
        private Vector3 filteredAccel;

        /// <summary>
        /// Used to calibrate the ambient gravitational force.
        /// </summary>
        private Vector3 zeroAccel;

        /// <summary>
        /// Indicates if this is the first frame to receive new IMU measurements.
        /// </summary>
        private bool firstUpdate;

        /// <summary>
        /// Multiplier for handedness such that 1 = Right, 0 = Center, -1 = left.
        /// </summary>
        private Vector3 handedMultiplier;

        private static I3vrArmModel instance = null;

        /// <summary>
        /// Use the I3vrController singleton to obtain a singleton for this class.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static I3vrArmModel Instance
        {
            get
            {
                return instance != null && instance.isActiveAndEnabled ? instance : null;
            }
        }

        /// <summary>
        /// Represents when gaze-following behavior should occur.
        /// </summary>
        public enum GazeBehavior
        {
            Never,        /// The shoulder will never follow the gaze.
            DuringMotion, /// The shoulder will follow the gaze during controller motion.
            Always        /// The shoulder will always follow the gaze.
        }

        private DpnDaydreamController controller;

        /// <summary>
        /// Height of the elbow  (m).
        /// </summary>
        [Range(0.0f, 0.2f)]
        public float addedElbowHeight = 0.0f;

        /// <summary>
        /// Depth of the elbow  (m).
        /// </summary>
        [Range(0.0f, 0.2f)]
        public float addedElbowDepth = 0.0f;

        /// <summary>
        /// Downward tilt of the laser pointer relative to the controller (degrees).
        /// </summary>
        [Range(0.0f, 30.0f)]
        public float pointerTiltAngle = 15.0f;

        /// <summary>
        /// Controller distance from the face after which the alpha value decreases (meters).
        /// </summary>
        [Range(0.0f, 0.4f)]
        public float fadeDistanceFromFace = 0.32f;

        /// <summary>
        /// Determines if the shoulder should follow the gaze
        /// </summary>
        public GazeBehavior followGaze = GazeBehavior.Never;

        /// <summary>
        /// Determines if the accelerometer should be used.
        /// </summary>
        public bool useAccelerometer = false;

        /// <summary>
        /// Vector to represent the pointer's location.
        /// NOTE: This is in meatspace coordinates.
        /// </summary>
        /// <value>
        /// The pointer position.
        /// </value>
        public Vector3 pointerPosition { get; private set; }

        /// <summary>
        /// Quaternion to represent the pointer's rotation.
        /// NOTE: This is in meatspace coordinates.
        /// </summary>
        /// <value>
        /// The pointer rotation.
        /// </value>
        public Quaternion pointerRotation { get; private set; }

        /// <summary>
        /// Vector to represent the wrist's location.
        /// NOTE: This is in meatspace coordinates.
        /// </summary>
        /// <value>
        /// The wrist position.
        /// </value>
        public Vector3 wristPosition { get; private set; }

        /// <summary>
        /// Quaternion to represent the wrist's rotation.
        /// NOTE: This is in meatspace coordinates.
        /// </summary>
        /// <value>
        /// The wrist rotation.
        /// </value>
        public Quaternion wristRotation { get; private set; }

        /// <summary>
        /// Vector to represent the elbow's location.
        /// NOTE: This is in meatspace coordinates.
        /// </summary>
        /// <value>
        /// The elbow position.
        /// </value>
        public Vector3 elbowPosition { get; private set; }

        /// <summary>
        /// Quaternion to represent the elbow's rotation.
        /// NOTE: This is in meatspace coordinates.
        /// </summary>
        /// <value>
        /// The elbow rotation.
        /// </value>
        public Quaternion elbowRotation { get; private set; }

        /// <summary>
        /// Vector to represent the shoulder's location.
        /// NOTE: This is in meatspace coordinates.
        /// </summary>
        /// <value>
        /// The shoulder position.
        /// </value>
        public Vector3 shoulderPosition { get; private set; }

        /// <summary>
        /// Vector to represent the shoulder's location.
        /// NOTE: This is in meatspace coordinates.
        /// </summary>
        /// <value>
        /// The shoulder rotation.
        /// </value>
        public Quaternion shoulderRotation { get; private set; }

        /// <summary>
        /// The suggested rendering alpha value of the controller.
        /// This is to prevent the controller from intersecting face.
        /// </summary>
        /// <value>
        /// The alpha value.
        /// </value>
        public float alphaValue { get; private set; }

        private void Awake()
        {
            controller = GetComponent<DpnDaydreamController>();
        }
        void Start()
        {
            instance = this;
            UpdateHandedness();
            // Reset other relevant state.
            firstUpdate = true;
            elbowOffset = Vector3.zero;
            alphaValue = 1.0f;
            zeroAccel.Set(0, GRAVITY_FORCE, 0);
        }

        void OnDestroy()
        {

            // Reset the singleton instance.
            instance = null;
        }

        public void OnControllerUpdate()
        {
            UpdateHandedness();
            UpdateTorsoDirection();
            if (DpnDaydreamController.State == DpnConnectionState.Connected)
            {
                UpdateFromController();
            }
            else
            {
                ResetState();
            }
            if (useAccelerometer)
            {
                UpdateVelocity();
                TransformElbow();
            }
            else
            {
                elbowOffset = Vector3.zero;
            }
            ApplyArmModel();
            UpdateTransparency();
            UpdatePointer();
        }

        private void UpdateHandedness()
        {
            handedMultiplier.Set(0, 1, 1);
            handedMultiplier.x = 1.0f;

            // Place the shoulder in anatomical positions based on the height and handedness.
            shoulderRotation = Quaternion.identity;
            if(interactiveHand == 0)
                shoulderPosition = Vector3.Scale(DEFAULT_SHOULDER_RIGHT, handedMultiplier);
            else
                shoulderPosition = Vector3.Scale(DEFAULT_SHOULDER_LEFT, handedMultiplier);
        }

        private Vector3 GetHeadOrientation()
        {
            return DpnCameraRig._instance.GetPose() * Vector3.one;
        }

        private void UpdateTorsoDirection()
        {
            // Ignore updates here if requested.
            if (followGaze == GazeBehavior.Never)
            {
                return;
            }

            // Determine the gaze direction horizontally.
            Vector3 gazeDirection = GetHeadOrientation();
            gazeDirection.y = 0.0f;
            gazeDirection.Normalize();

            // Use the gaze direction to update the forward direction.
            if (followGaze == GazeBehavior.Always)
            {
                torsoDirection = gazeDirection;
            }
            else if (followGaze == GazeBehavior.DuringMotion)
            {
                float angularVelocity = DpnDaydreamController.Gyro.magnitude;
                float gazeFilterStrength = Mathf.Clamp((angularVelocity - 0.2f) / 45.0f, 0.0f, 0.1f);
                torsoDirection = Vector3.Slerp(torsoDirection, gazeDirection, gazeFilterStrength);
            }

            // Rotate the fixed joints.
            Quaternion gazeRotation = Quaternion.FromToRotation(Vector3.forward, torsoDirection);
            shoulderRotation = gazeRotation;
            shoulderPosition = gazeRotation * shoulderPosition;
        }

        private void UpdateFromController()
        {
            // Get the orientation-adjusted acceleration.
            Vector3 accel = DpnDaydreamController.Orientation * DpnDaydreamController.Accel;

            // Very slowly calibrate gravity force out of acceleration.
            zeroAccel = zeroAccel * GRAVITY_CALIB_STRENGTH + accel * (1.0f - GRAVITY_CALIB_STRENGTH);
            filteredAccel = accel - zeroAccel;

            // If no tracking history, reset the velocity.
            if (firstUpdate)
            {
                filteredVelocity = Vector3.zero;
                firstUpdate = false;
            }

            // IMPORTANT: The accelerometer is not reliable at these low magnitudes
            // so ignore it to prevent drift.
            if (filteredAccel.magnitude < MIN_ACCEL)
            {
                // Suppress the acceleration.
                filteredAccel = Vector3.zero;
                filteredVelocity *= LOW_ACCEL_VELOCITY_SUPPRESS;
            }
            else
            {
                // If the velocity is decreasing, prevent snap-back by reducing deceleration.
                Vector3 newVelocity = filteredVelocity + filteredAccel * Time.deltaTime;
                if (newVelocity.sqrMagnitude < filteredVelocity.sqrMagnitude)
                {
                    filteredAccel *= LOW_VELOCITY_ACCEL_SUPPRESS;
                }
            }
        }

        private void UpdateVelocity()
        {
            // Update the filtered velocity.
            filteredVelocity += filteredAccel * Time.deltaTime;
            filteredVelocity *= VELOCITY_FILTER_SUPPRESS;
        }

        private void ResetState()
        {
            // We've lost contact, quickly reset the state.
            filteredVelocity *= 0.5f;
            filteredAccel *= 0.5f;
            firstUpdate = true;
        }

        private void TransformElbow()
        {
            // Apply the filtered velocity to update the elbow offset position.
            if (useAccelerometer)
            {
                elbowOffset += filteredVelocity * Time.deltaTime;
                elbowOffset.x = Mathf.Clamp(elbowOffset.x, ELBOW_MIN_RANGE.x, ELBOW_MAX_RANGE.x);
                elbowOffset.y = Mathf.Clamp(elbowOffset.y, ELBOW_MIN_RANGE.y, ELBOW_MAX_RANGE.y);
                elbowOffset.z = Mathf.Clamp(elbowOffset.z, ELBOW_MIN_RANGE.z, ELBOW_MAX_RANGE.z);
            }
        }

        private void ApplyArmModel()
        {
            // Find the controller's orientation relative to the player
            Quaternion controllerOrientation = DpnDaydreamController.Orientation;
            //Debug.Log("sss shoulder rotation is " + shoulderRotation);
            controllerOrientation = Quaternion.Inverse(shoulderRotation) * controllerOrientation;

            // Get the relative positions of the joints
            if (interactiveHand == 0)
                elbowPosition = ELBOW_POSITION + new Vector3(0.0f, addedElbowHeight, addedElbowDepth);
            else
                elbowPosition = ELBOW_POSITION_LEFT + new Vector3(0.0f, addedElbowHeight, addedElbowDepth);

            elbowPosition = Vector3.Scale(elbowPosition, handedMultiplier) + elbowOffset;
            //Debug.Log("sss elbow offset is " + elbowOffset);
            //Debug.Log("sss hande Multiplier is " + handedMultiplier);
            wristPosition = Vector3.Scale(WRIST_POSITION, handedMultiplier);
            Vector3 armExtensionOffset;
            if(interactiveHand ==0 )
                armExtensionOffset  = Vector3.Scale(ARM_EXTENSION_OFFSET, handedMultiplier);
            else
                armExtensionOffset = Vector3.Scale(ARM_EXTENSION_OFFSET_LEFT, handedMultiplier);

            // Extract just the x rotation angle
            Vector3 controllerForward = controllerOrientation * Vector3.forward;
            float xAngle = 90.0f - Vector3.Angle(controllerForward, Vector3.up);

            // Remove the z rotation from the controller
            Quaternion xyRotation = Quaternion.FromToRotation(Vector3.forward, controllerForward);

            // Offset the elbow by the extension
            float normalizedAngle = (xAngle - MIN_EXTENSION_ANGLE) / (MAX_EXTENSION_ANGLE - MIN_EXTENSION_ANGLE);
            float extensionRatio = Mathf.Clamp(normalizedAngle, 0.0f, 1.0f);
            //Debug.Log("sss use acclerometer" + useAccelerometer);
            if (!useAccelerometer)
            {
                elbowPosition += armExtensionOffset * extensionRatio;
            }

            // Calculate the lerp interpolation factor
            float totalAngle = Quaternion.Angle(xyRotation, Quaternion.identity);
            float lerpSuppresion = 1.0f - Mathf.Pow(totalAngle / 180.0f, 6);
            float lerpValue = lerpSuppresion * (0.4f + 0.6f * extensionRatio * EXTENSION_WEIGHT);

            // Apply the absolute rotations to the joints
            Quaternion lerpRotation = Quaternion.Lerp(Quaternion.identity, xyRotation, lerpValue);
            elbowRotation = shoulderRotation * Quaternion.Inverse(lerpRotation) * controllerOrientation;
            wristRotation = shoulderRotation * controllerOrientation;

            // Determine the relative positions
            elbowPosition = shoulderRotation * elbowPosition;
            wristPosition = elbowPosition + elbowRotation * wristPosition;
        }

        private void UpdateTransparency()
        {
            // Determine how vertical the controller is pointing.
            float distToFace = Vector3.Distance(wristPosition, Vector3.zero);
            if (distToFace < fadeDistanceFromFace)
            {
                alphaValue = Mathf.Max(0.0f, alphaValue - DELTA_ALPHA * Time.deltaTime);
            }
            else
            {
                alphaValue = Mathf.Min(1.0f, alphaValue + DELTA_ALPHA * Time.deltaTime);
            }
        }

        private void UpdatePointer()
        {
            // Determine the direction of the ray.
            pointerPosition = wristPosition + wristRotation * POINTER_OFFSET;
            pointerRotation = wristRotation * Quaternion.AngleAxis(pointerTiltAngle, Vector3.right);
        }

        public int interactiveHand = 0;
        
    }
}
