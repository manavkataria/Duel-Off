using UnityEngine;
using System;
using System.Collections;

/** Stash Text Update
 * -- Used to update all Text associated with selected SuppliesUIObject
 * -- inside the clipped view of the Stash Window.
 */ 
public class StashTextUpdate : MonoBehaviour {
	
	// This label to be checked against
    private UILabel thisLabel;
	
	// Cached placeholders for checks inside below switch statements
    private GunUIObject gun;
    private HealthUIObject health;

    void OnEnable() { StashController.onSelection += updateText; }
    void OnDisable() { StashController.onSelection -= updateText; }

    void Awake() { thisLabel = GetComponent<UILabel>(); }

    void updateText( SuppliesUIObject sup )
    {
        Type g = typeof(GunUIObject);
        Type h = typeof(HealthUIObject);

        switch( sup.supplyType )
        {
            case SuppliesUIObject._SupplyType.Gun:
                if (g == sup.GetType())
                {
                    gun = (GunUIObject)sup;
                    switch (gameObject.name)
                    {
                        case "Item Power":
                            thisLabel.text = "Power : " + gun.gunObj.power;
                            break;
                        case "Item Type":
                            thisLabel.text = gun.gunObj.model;
                            break;
                        case "Item RPM":
                            thisLabel.text = "firespeed : " + gun.gunObj.firingRate;
                            break;
                        default:
                            break;
                    }
                }
                break;
            case SuppliesUIObject._SupplyType.Health:
                if (h == sup.GetType())
                {
                    health = (HealthUIObject)sup;
                    switch (gameObject.name)
                    {
                        case "Item Power":
                            thisLabel.text = "Health : " + health.hPack.power;
                            break;
                        case "Item Type":
                            thisLabel.text = health.hPack.model;
                            break;
                        case "Item RPM":
                            thisLabel.text = "Quantity : " + health.hPack.quantity;
                            break;
                        default:
                            break;
                    }
                }
                break;
            default: break;
        }
    }
}
