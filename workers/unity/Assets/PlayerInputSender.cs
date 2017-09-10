using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using Improbable.Unity;

[WorkerType(WorkerPlatform.UnityClient)]
public class PlayerInputSender : MonoBehaviour {

	[Require] private PlayerInput.Writer PlayerInputWriter;

	private void OnEnable()
	{
		GetComponent<Rigidbody> ().isKinematic = true;
		GetComponent<Renderer>().material.color = Color.blue;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			var update = new PlayerInput.Update ();
			update.AddStartCharging (new StartCharging ());
			PlayerInputWriter.Send (update);
		}

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Debug.DrawLine (ray.origin, ray.origin + ray.direction * 20);

		if (Input.GetKeyUp (KeyCode.Space)) {
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				Debug.Log (hit);
				var direction = (hit.point - transform.position).normalized;
				var update = new PlayerInput.Update ();
				update.AddReleaseCharge (new ReleaseCharge (new Direction (direction.x, direction.z)));
				PlayerInputWriter.Send (update);
			}
		}
	}
}
