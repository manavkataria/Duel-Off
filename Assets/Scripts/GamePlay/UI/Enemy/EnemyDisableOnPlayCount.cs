using UnityEngine;
using System.Collections;

public class EnemyDisableOnPlayCount : MonoBehaviour {

	void Start()
	{
		int count = DBAccess.instance.userPrefs.playCount;
		
		switch( name )
		{
		case "Character1":
			break;
		case "Character2":
			if( count == 0 )
				gameObject.SetActiveRecursively(false);
			break;
		case "Character3":
			if( count == 0 )
				gameObject.SetActiveRecursively(false);
			break;
		case "Character4":
			if( count == 0 )
				gameObject.SetActiveRecursively(false);
			break;
		case "Character5":
			if( count == 0 )
				gameObject.SetActiveRecursively(false);
			break;
		case "Character6":
			if( count == 0 )
				gameObject.SetActiveRecursively(false);
			break;
		default:
			break;
		}
	}
}
