using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Manager : MonoBehaviour
{
    [Header("Basic")]
    public Image healthBar;
    public Image manaBar;
    public Image staminaBar;
    public Image ultimateBar;
    public Image weaponHolder;
    public Sprite weaponHolderUsual;
    public Sprite weaponHolderUltimate;
    public TMP_Text money, lvl;
    public float bars_width = 156f;
    public float ultimate_bar_height = 136f;
    public Sprite PlayerSword;
    public Sprite PlayerBow;
    public Image ActiveWeaponSlot;
    [Header("Artifact Tooltip")]
    public GameObject tooltip;
    public TMP_Text artifactName;
    public TMP_Text artifactCost;
    public TMP_Text artifactDescription;
    public Image artifactImage;

    private void Awake()
    {
        tooltip.SetActive(false);
    }
    public void Set_Hp_Bar(float value)
    {
        RectTransform rt = healthBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(value * bars_width, rt.sizeDelta.y);
    }
    public void Set_Mana_Bar(float value)
    {
        RectTransform rt = manaBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(value * bars_width, rt.sizeDelta.y);
    }
    public void Set_Stamina_Bar(float value)
    {
        RectTransform rt = staminaBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(value * bars_width, rt.sizeDelta.y);
    }
    public void Set_LVL(int value)
    {
        lvl.text = "LVL " + value.ToString();
    }
    public void Set_Money_Count(int value)
    {
        money.text = value.ToString();
    }

    public void ChangeWeapon(WeaponType weapon_type)
    {
        if(weapon_type == WeaponType.isSword)
        {
            ActiveWeaponSlot.sprite = PlayerSword;
        }
        else if(weapon_type == WeaponType.isMagicBow)
        {
            ActiveWeaponSlot.sprite = PlayerBow;
        }
    }

    public void UpdateShopArtifactTooltip(Artifact artifact, int cost)
    {
        artifactName.text = artifact.name;
        artifactDescription.text = artifact.description;
        artifactCost.text = "Cost: " + cost.ToString();
        artifactImage.sprite = artifact.sprite;
    }
    public void ShowShopArtifactTooltip()
    {
        tooltip.SetActive(true);
    }
    public void HideShopArtifactTooltip()
    {
        tooltip.SetActive(false);
    }
    public void UpdateWeaponHolder(bool isUltimate, float value)
    {
        if (isUltimate)
        {
            weaponHolder.sprite = weaponHolderUltimate;
        }
        else
        {
            weaponHolder.sprite = weaponHolderUsual;
        }
        RectTransform rect = ultimateBar.rectTransform;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, value * ultimate_bar_height);
    }
}
