using UnityEngine;
using System.Collections;

public class BloodSplatter : MonoBehaviour {
	
	public Transform[] smallSplat;
	public Transform largeSplat1;
	public Transform largeSplat2;
	
	public Transform[] drip;
	
	public Transform RedScreen;
	
	public Material Splatmat;
	private float scaleVal;
	
	private Vector3 randomSplatPos = new Vector3(0,0,0.2f);
	private Vector3 randomDripPos = new Vector3(0,0,0.3f);
	
    private Color SPLAT_COLOR = new Color(1f,1f,1f,0.7f);
	private Vector3 RED_SCREEN_START_POS = new Vector3(0,0,0.1f);
		
	public void onHitByRayCast() { StartCoroutine( GotShot() ); }
	
	IEnumerator GotShot()
	{
		Splatmat.color = SPLAT_COLOR;
		RedScreen.transform.localPosition = RED_SCREEN_START_POS;
		
		for( int i = 0; i < smallSplat.Length; i ++ )
		{
			randomSplatPos.x = Random.Range(1.8f,-1.8f);
			randomSplatPos.y = Random.Range(1f,-1f);
			
			scaleVal = Random.Range(0.25f,1f);
			
			smallSplat[i].transform.localPosition = randomSplatPos;
			smallSplat[i].transform.localScale = new Vector3(scaleVal,scaleVal,scaleVal);
			
			if( i < drip.Length )
			{
				randomDripPos.x = Random.Range(0.1f,-0.1f);
				randomDripPos.y = Random.Range(0.5f,-0.5f);
				drip[i].transform.localPosition = randomDripPos;
				
				scaleVal = Random.Range( 0.25f, 1f );
				
				drip[i].transform.localScale = new Vector3(scaleVal,scaleVal,scaleVal);
				
				drip[i].animation["Drip"].speed = Random.Range(0.1f,1f);
				drip[i].animation.Stop("Drip");
				drip[i].animation.Play("Drip");
			}
		}
		
		largeSplat1.transform.localPosition = new Vector3(Random.Range(1f,-1f),Random.Range(.5f,-.5f),.1f);
		largeSplat2.transform.localPosition = new Vector3(Random.Range(1f,-1f),Random.Range(.5f,-.5f),.1f);
		
		scaleVal = Random.Range(.25f,.65f);
		largeSplat1.transform.localScale = new Vector3(scaleVal,scaleVal,scaleVal);
		scaleVal = Random.Range(.25f,.9f);
		largeSplat2.transform.localScale = new Vector3(scaleVal,scaleVal,scaleVal);
		largeSplat1.transform.localEulerAngles = new Vector3(0,180,Random.Range(0,360));
		largeSplat2.transform.localEulerAngles = new Vector3(0,180,Random.Range(0,360));
		
		yield return new WaitForSeconds(.01f);
		RedScreen.transform.localPosition = new Vector3(0,-5,0);
		yield return new WaitForSeconds(1.4f);
		for(var n = 0; n < 7; n++){
			Splatmat.color = Splatmat.color - new Color(0,0,0,.1f);
			yield return new WaitForSeconds(.05f);
		}
		
		for( int i = 0; i < smallSplat.Length; i++ )
		{
			smallSplat[i].localPosition = Vector3.zero;
			
			if( i < drip.Length )
				drip[i].transform.localPosition = Vector3.zero;
		}
		
		largeSplat1.transform.localPosition = new Vector3(0,0,0);
		largeSplat2.transform.localPosition = new Vector3(0,0,0);		
	}

}
