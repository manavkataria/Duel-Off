using UnityEngine;
using System.Collections;

public class EnemyHealthBarLookAt : MonoBehaviour {

	private Transform thisTransform;
	
	void Awake() { thisTransform = transform; }
	
	void Update() { thisTransform.LookAt( Vector3.zero ); }
}
