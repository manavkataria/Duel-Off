using UnityEngine;
using System;
using System.Collections;

public static class BooleanExtensions  {

	public static IEnumerator SetAfterDelay( this bool flag, float delay, Action switchTo )
	{
		yield return new WaitForSeconds(delay);
		
		switchTo();
	}
}
