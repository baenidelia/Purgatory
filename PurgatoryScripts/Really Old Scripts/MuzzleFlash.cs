using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {

    public GameObject muzzleHolder;
    public Sprite[] muzzleSprites;
    public SpriteRenderer[] spriteRend;

    public float muzzleTime;

    private void Start()
    {
        Deactivate();
    }

    public void Activate() {
        muzzleHolder.SetActive(true);

        int muzzleSpriteIndex = Random.Range(0, muzzleSprites.Length);
        for (int i = 0; i < spriteRend.Length; i++)
        {
            spriteRend[i].sprite = muzzleSprites[muzzleSpriteIndex];
        }

        Invoke("Deactivate", muzzleTime);
    }

    void Deactivate()
    {
        muzzleHolder.SetActive(false);
    }
}

