using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour
{
    public string abilityButtonAxisName = "Fire1";
    public Image darkMask;
    public Text cooldownTextDisplay;

    [SerializeField]
    private AbilityPew ability;
    [SerializeField]
    private GameObject weaponHolder;
    private Image myButtonImage;
    private AudioSource abilitySource;
    //[HideInInspector]
    public float coolDownDuration;
    private float nextReadyTime;
    private float coolDownTimeLeft;

    void Start()
    {
        Initialize(ability, weaponHolder);
    }

    public void Initialize(AbilityPew selectedAbility, GameObject weaponHolder)
    {
        ability = selectedAbility;
        myButtonImage = GetComponent<Image>();
        abilitySource = GetComponent<AudioSource>();
        myButtonImage.sprite = ability.aSprite;
        darkMask.sprite = ability.aSprite;
        coolDownDuration = ability.aBaseCooldown;
        ability.Initialize(weaponHolder);
        AbilityReady();
    }

    void Update()
    {
        bool coolDownComplete = (Time.time > nextReadyTime);
        if (coolDownComplete)
        {
			
            AbilityReady();
            if (Input.GetButton(abilityButtonAxisName))
            {
                ButtonTriggered();
            }
        }
        else
		{
            CoolDown();
        }
    }

    private void AbilityReady()
    {
        cooldownTextDisplay.enabled = false;
        darkMask.enabled = false;
    }

    private void CoolDown()
    {
        coolDownTimeLeft -= Time.deltaTime;
        float roundedCd = Mathf.Round(coolDownTimeLeft);
        cooldownTextDisplay.text = roundedCd.ToString();
        darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
    }

    private void ButtonTriggered()
    {
        nextReadyTime = coolDownDuration + Time.time;
        coolDownTimeLeft = coolDownDuration;
        darkMask.enabled = true;
        cooldownTextDisplay.enabled = true;

        abilitySource.clip = ability.aSound;
        abilitySource.Play();
        ability.TriggerAbility();
    }

    public void UpdateCooldown(GameObject obj, float duration)
    {
        AbilityCooldown newAbCd = obj.GetComponent<AbilityCooldown>();
        newAbCd.coolDownDuration = duration;
    }
}
