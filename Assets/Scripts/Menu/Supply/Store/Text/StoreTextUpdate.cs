using UnityEngine;
using System;
using System.Collections;

/** Store Description Text
 *  -- Attached to all Text items in the Store to be updated
 *  -- upon object selection.  Chose to do an all in-one class simply
 *  -- for the low use case.
 */ 
public class StoreTextUpdate : MonoBehaviour {

    private UILabel thisLabel;

    void OnEnable() { StoreController.onSelection += updateText; }
    void OnDisable() { StoreController.onSelection -= updateText; }

    void Awake() { thisLabel = GetComponent<UILabel>(); }

    void updateText(SuppliesUIObject sup)
    {
        Type g = typeof(GunUIObject);
        Type h = typeof(HealthUIObject);
        Type a = typeof(AmmoUIObject);

        switch( sup.supplyType )
        {
            case SuppliesUIObject._SupplyType.Gun:
                if (g == sup.GetType())
                {
					GunUIObject gun;
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
                        case "Item Cost": 
                            thisLabel.text =  + gun.gunObj.price + "G"; 
                            break;
                        default: break;
                    }
                }
                break;
            case SuppliesUIObject._SupplyType.Health:
                if (h == sup.GetType())
                {
					HealthUIObject health;
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
                        case "Item Cost":
                            thisLabel.text = health.hPack.price + "G";
                            break;
                    }
                }
                break;
            case SuppliesUIObject._SupplyType.Ammo:
                if (a == sup.GetType())
                {
					AmmoUIObject ammo;
                    ammo = (AmmoUIObject)sup;
                    switch (gameObject.name)
                    {
                        case "Item Power":
                            thisLabel.text = "Power : " + ammo.ammo.dmgModifier;
                            break;
                        case "Item Type":
                            thisLabel.text = ammo.ammo.ammoName;
                            break;
                        case "Item RPM":
                            thisLabel.text = "Clip Size: " + ammo.ammo.magSize;
                            break;
                        case "Item Cost":
                            thisLabel.text = ammo.ammo.price + "G";
                            break;
                    }
                }
                break;
            default: break;
        }
    }
}
