using UnityEngine;
using System.Collections;

// Script to set animation layers of the Character for blending of animations
public class EnemyAnimLayers : MonoBehaviour {
	
	public Transform UpperBody;
	
	void Awake()
	{
		// Layer 1 : Full body movement Layer
		animation["Character_Idle"].layer = 1;
		animation["Character_Run"].layer = 1;
		
		// Layer 2 : Upper Body Layer that controls whether or not he is aiming.
		animation["Character_Aim0"].layer = 2;
		animation["Character_THW_Aim0"].layer = 2;
		animation["Character_THW"].layer = 2;

		// Layer 3 : Controls where he is aiming
		animation["Character_Aim180"].layer = 3;
		animation["Character_Aim-180"].layer = 3;
		animation["Character_THW_Aim180"].layer = 3;
		animation["Character_THW_Aim-180"].layer = 3;
		
		// Layer 4 : Controls his shooting
		animation["Character_Aim0_Shoot"].layer = 4;
		animation["Character_Aim-180_Shoot"].layer = 4;
		animation["Character_Aim180_Shoot"].layer = 4;
		animation["Character_THW_Aim0_Shoot"].layer = 4;
		animation["Character_THW_Aim-180_Shoot"].layer = 4;
		animation["Character_THW_Aim180_Shoot"].layer = 4;
		
		// Define where the layer starts at in the hierarchy
		animation["Character_Aim-180"].AddMixingTransform(UpperBody);
		animation["Character_Aim180"].AddMixingTransform(UpperBody);
		animation["Character_Aim0"].AddMixingTransform(UpperBody);
		animation["Character_Aim-180_Shoot"].AddMixingTransform(UpperBody);
		animation["Character_Aim180_Shoot"].AddMixingTransform(UpperBody);
		animation["Character_Aim0_Shoot"].AddMixingTransform(UpperBody);
		
		animation["Character_THW"].AddMixingTransform(UpperBody);
		animation["Character_THW_Aim-180"].AddMixingTransform(UpperBody);
		animation["Character_THW_Aim180"].AddMixingTransform(UpperBody);
		animation["Character_THW_Aim0"].AddMixingTransform(UpperBody);
		animation["Character_THW_Aim-180_Shoot"].AddMixingTransform(UpperBody);
		animation["Character_THW_Aim180_Shoot"].AddMixingTransform(UpperBody);
		animation["Character_THW_Aim0_Shoot"].AddMixingTransform(UpperBody);
	}
}
