using UnityEngine;
using System.Collections;

public class GunUIObject : SuppliesUIObject {

    public Gun gunObj;
	public string model;
	public Vector3 cachedLocalScale;
	public Vector3 cachedLocalPosition;
	
	void Start()
	{
		gunObj.setIsPurchased( mWidget, gunObj.isPurchased );
	}
}
