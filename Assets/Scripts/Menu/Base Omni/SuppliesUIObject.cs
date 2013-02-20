using UnityEngine;
using System.Collections;

/** Supply Store Objects
 * -- Used across all supply store GUI items to represent internal data
 * -- and to pass to objects for property based checks. Classes that inherit
 * -- include but not limited to: GunUIObject, AmmoUIObject, HealthUIObject,
 * -- ClickableUIObject.
 * 
 * -- Classes that inherit are also used as a typeof() check so that all items
 * -- can be populated in Arrays of this base type for ease of use, storage, and search.
 */
public class SuppliesUIObject : MonoBehaviour {

    public enum _SupplyType { None, Gun, Health, Ammo }
    public _SupplyType supplyType;

    public enum _ItemLocale 
	{
		None, 
		StashTop, 
		StashBottom, 
		StoreTop, 
		StashGridItem, 
		StoreGridItem, 
		Arrow 
	}
    public _ItemLocale itemLocale;

    public UISprite mWidget;

    public void Awake() { mWidget = GetComponent<UISprite>(); }
}
