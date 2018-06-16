using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour {

    public Gun startingGun;
    public Transform weaponHolder;

    Gun equippedGun;

    void Start()
    {
        if(startingGun != null)
        {
            equipGun(startingGun);
        }
    }
    public void equipGun(Gun gunToEquip)
    {
        if(equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHolder.position, weaponHolder.rotation) as Gun;
        equippedGun.transform.parent = weaponHolder;
    }
}
