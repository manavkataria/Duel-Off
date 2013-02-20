using UnityEngine;
using System.Collections;


public class CubeRotator : MonoBehaviour
{
	public float speed = 15.0f;

	private Transform cube;
	private bool shouldRotate = true;
	
	void Start()
	{
		cube = GetComponent<Transform>();
	}
	
	
	// Update is called once per frame
	void Update()
	{
		if( shouldRotate )
			cube.Rotate( Vector3.forward, Time.deltaTime * speed );
	}
	
	
	public void togglePauseRotation()
	{
		Debug.Log( "toggle pause" );
		shouldRotate = !shouldRotate;
	}
}
