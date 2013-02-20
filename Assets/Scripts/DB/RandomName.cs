using UnityEngine;
using System.Collections;

public class RandomName : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		UILabel lbl = GetComponent<UILabel>();
		
		System.Object[,] name = DBAccess.instance.getRandomEnemyName();
		
		string fullName = (string)name[0,0] + " " + (string)name[1,0];
		
		lbl.text = fullName;
		
	}

}
