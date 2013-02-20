using UnityEngine;
using System.Collections;

// Simple class to add recoil depending on Gun - Recoil Force
public class CameraRecoil : MonoBehaviour {
	
	[HideInInspector]
	public float recoilForce;
	
	private float upSpeed = 9f;
	private float dnSpeed = 10f;
	
	private Vector3 initialAngle;
	private float recoilAngle;
	private Vector3 smoothAngle = Vector3.zero;
		
	void Start() { initialAngle = transform.localEulerAngles; }
	
	public void applyRecoil() { recoilAngle += recoilForce; }
	
	void Update()
	{
		smoothAngle.x = Mathf.Lerp( smoothAngle.x, recoilAngle, upSpeed * Time.deltaTime );
		transform.localEulerAngles = initialAngle - smoothAngle;
		recoilAngle = Mathf.Lerp( recoilAngle, 0, dnSpeed * Time.deltaTime );
	}
}
