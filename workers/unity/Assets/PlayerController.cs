using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using Improbable;
using Improbable.Unity.Common.Core.Math;
using Improbable.Core;

public class PlayerController : MonoBehaviour {
	float chargeCapacitator;
	Rigidbody rigidbody;
	bool charging = false;
	Vector3 chargeForce;
	[SerializeField] float maxCharge = 2;

	[Require] private Position.Writer PositionWriter; 
	[Require] private Rotation.Writer RotationWriter; 
	[Require] private PlayerInput.Reader PlayerInputReader;

	void OnEnable () {
		Debug.Log("PlayerController#OnEnable");
		rigidbody = GetComponent<Rigidbody> ();
		charging = false;
		chargeCapacitator = 0;
		chargeForce = Vector3.zero;
		PlayerInputReader.StartChargingTriggered.Add (OnStartChargingTriggered);
		PlayerInputReader.ReleaseChargeTriggered.Add (OnReleaseChargeTriggered);
	}

	void OnDisable () {
		PlayerInputReader.StartChargingTriggered.Remove (OnStartChargingTriggered);
		PlayerInputReader.ReleaseChargeTriggered.Remove (OnReleaseChargeTriggered);
	}

	void OnStartChargingTriggered (StartCharging _) {
		Debug.Log("Started Charging");
		charging = true;
	}

	void OnReleaseChargeTriggered (ReleaseCharge release) {
		Vector3 direction = new Vector3 (release.direction.x, transform.position.y, release.direction.y).normalized;
		Debug.LogWarning (chargeCapacitator);
		Debug.LogWarning (direction);
		chargeForce = direction * (chargeCapacitator / maxCharge) * 3 * 1000;
		charging = false;
		chargeCapacitator = 0;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (charging) {
			chargeCapacitator += Time.fixedDeltaTime;
			if (chargeCapacitator > maxCharge) {
				chargeCapacitator = maxCharge;
			}
		}
		if (chargeForce != Vector3.zero) {
			rigidbody.AddForce (chargeForce);
			chargeForce = Vector3.zero;
		}

		var pos = transform.position.ToSpatialCoordinates ();
		PositionWriter.Send (new Position.Update ().SetCoords (pos));
		var rot = transform.rotation;
		RotationWriter.Send (new Rotation.Update ().SetRotation (new Improbable.Core.Quaternion (rot.x, rot.y, rot.z, rot.w)));

		if (transform.position.y < -5) {
			transform.position = new Vector3 (0, 5, 0);
			rigidbody.velocity = Vector3.zero;
		}
	}
}
