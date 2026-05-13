using TMPro;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Manager : MonoBehaviour
{
    [Header("Basic")]
    public RectTransform healthBarRT;
    public RectTransform manaBarRT;
    public RectTransform staminaBarRT;
    public RectTransform ultimateBarRT;
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
    [Header("ArenaMessage")]
    public GameObject messageBox;
    public Button messageButton;
    public TMP_Text arenaMessageText;
    [Header("BossHPBar")]
    public GameObject bossBar;
    public TMP_Text bossName;
    public RectTransform bossHPBarTR;
    public float bossHPBar_width = 616f;

    private void Awake()
    {
        tooltip.SetActive(false);
        messageBox.SetActive(false);
        messageButton.onClick.AddListener(HideArenaMessage);
        bossBar.SetActive(false);
    }
    public void Set_Hp_Bar(float value)
    {
        healthBarRT.sizeDelta = new Vector2(value * bars_width, healthBarRT.sizeDelta.y);
    }
    public void Set_Mana_Bar(float value)
    {
        manaBarRT.sizeDelta = new Vector2(value * bars_width, manaBarRT.sizeDelta.y);
    }
    public void Set_Stamina_Bar(float value)
    {
        staminaBarRT.sizeDelta = new Vector2(value * bars_width, staminaBarRT.sizeDelta.y);
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
        ultimateBarRT.sizeDelta = new Vector2(ultimateBarRT.sizeDelta.x, value * ultimate_bar_height);
    }

    public void ShowArenaMessage()
    {
        messageBox.SetActive(true);
        arenaMessageText.text = GameContext.arenaCondition;
        Time.timeScale = 0.0f;
    }
    public void HideArenaMessage()
    {
        messageBox.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ShowBossHPBar(string bossNameText)
    {
        bossBar.SetActive(true);
        bossName.text = bossNameText;
    }
    public void HideBossHPBar()
    {
        bossBar.SetActive(false);
    }
    public void UpdateBossHPBar(float percentage)
    {
        bossHPBarTR.sizeDelta = new Vector2(percentage * bossHPBar_width, bossHPBarTR.sizeDelta.y);
    }
}
