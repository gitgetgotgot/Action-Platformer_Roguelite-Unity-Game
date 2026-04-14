using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public RectTransform canvasTransform;
    [Header("Player Stats")]
    public TMP_Text lvl;
    public TMP_Text xp;
    public TMP_Text hp;
    public TMP_Text mana;
    public TMP_Text stamina;
    public TMP_Text def;
    public TMP_Text crit_rate;
    public TMP_Text crit_dmg;
    public TMP_Text dmg_reduction;
    public TMP_Text mana_regen_bonus;
    public TMP_Text stamina_regen_bonus;
    public TMP_Text sword_dmg_bonus;
    public TMP_Text bow_dmg_bonus;
    public TMP_Text magic_dmg_bonus;
    [Header("InventorySlots")]
    public GameObject InventorySlots;
    public GameObject ActiveSlots;
    public Image chosenItem;
    [Header("Craft slots")]
    public InventorySlot craftSlot1;
    public InventorySlot craftSlot2;
    public InventorySlot resultSlot;
    [Header("Tooltip")]
    public Image tooltip;
    public TMP_Text tooltipName;
    public TMP_Text tooltipDsrtn;

    private int activeChosenArtifactId = 0;
    private InventorySlot[] unactiveSlots;
    private InventorySlot[] activeSlots;
    private const int UNACTIVE_SLOTS_COUNT = 8;
    private const int ACTIVE_SLOTS_COUNT = 4;
    private void Awake()
    {
        chosenItem.enabled = false;
        tooltip.gameObject.SetActive(false);
        unactiveSlots = InventorySlots.GetComponentsInChildren<InventorySlot>();
        activeSlots = ActiveSlots.GetComponentsInChildren<InventorySlot>();
    }
    private void Update()
    {
        if (activeChosenArtifactId != 0)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            Input.mousePosition,
            Camera.main,
            out localPos
            );
            chosenItem.transform.localPosition = localPos;
        }
    }
    public void UpdateInventoryText()
    {
        PlayerStats stats = GameContext.playerStats;
        lvl.text = GameContext.activeSave.playerName + " Lvl " + stats.level.ToString();
        xp.text = "XP " + stats.current_xp.ToString() + " / " + stats.xp_for_next_lvl.ToString();

        hp.text = "HP " + stats.hp.ToString() + " / " + stats.maxHP.ToString();
        mana.text = "Mana " + stats.mana.ToString() + " / " + stats.maxMANA.ToString();
        stamina.text = "Stamina " + stats.stamina.ToString() + " / " + stats.maxStamina.ToString();
        def.text = "Def " + stats.def.ToString();
        crit_rate.text = "Crit Rate " + stats.crit_rate.ToString() + "%";
        crit_dmg.text = "Crit Dmg " + stats.crit_dmg.ToString() + "%";
        dmg_reduction.text = "Dmg Reduction 0%";
        mana_regen_bonus.text = "Mana Regen Bonus " + (stats.mana_regen * 100f - 100f).ToString() + "%";
        stamina_regen_bonus.text = "Stamina Regen Bonus " + (stats.stamina_regen * 100f - 100f).ToString() + "%";
        sword_dmg_bonus.text = "Sword Dmg Bonus " + (stats.sword_dmg_mlpr * 100f - 100f).ToString() + "%";
        bow_dmg_bonus.text = "Bow Dmg Bonus " + (stats.bow_dmg_mlpr * 100f - 100f).ToString() + "%";
        magic_dmg_bonus.text = "Magic Dmg Bonus " + (stats.magic_dmg_mlpr * 100f - 100f).ToString() + "%";
    }

    public void InitInventorySlots()
    {

    }
    public void AddArtifact(int artifactID)
    {
        foreach(InventorySlot slot in unactiveSlots)
        {
            if(slot.artifact_id == 0)
            {
                slot.AddArtifact(artifactID);
                break;
            }
        }
    }
    public void AddArtifactToActiveSlot(int artifactID, int slotID)
    {
        activeSlots[slotID].AddArtifact(artifactID);
    }
    public void AddArtifactToUnactiveSlot(int artifactID, int slotID)
    {
        unactiveSlots[slotID].AddArtifact(artifactID);
    }
    public void UpdateCraft()
    {
        if(resultSlot.artifact_id == 0)
        {
            //show result if 2 appropriate artifacts are placed
            if (craftSlot1.artifact_id != 0 && craftSlot2.artifact_id != 0)
            {
                Artifact a1 = ArtifactsManager.Instance.GetArtifact(craftSlot1.artifact_id);
                Artifact a2 = ArtifactsManager.Instance.GetArtifact(craftSlot2.artifact_id);
                if(a1.craftableArtifactId == a2.craftableArtifactId && a1.craftableArtifactId != 0)
                {
                    resultSlot.AddArtifact(a1.craftableArtifactId);
                }
            }
        }
        else
        {
            resultSlot.RemoveArtifact();
        }
    }
    public void UseCraftItems()
    {
        craftSlot1.RemoveArtifact();
        craftSlot2.RemoveArtifact();
    }
    public void ShowTooltip(Artifact artifactInfo, Vector3 pos)
    {
        tooltipName.text = artifactInfo.name;
        tooltipDsrtn.text = artifactInfo.description;
        tooltip.rectTransform.localPosition = pos;
        tooltip.gameObject.SetActive(true);
    }
    public void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }
    public void TakeItemFromSlot(int artifact_id)
    {
        activeChosenArtifactId = artifact_id;
        chosenItem.sprite = ArtifactsManager.Instance.GetArtifact(activeChosenArtifactId).sprite;
        chosenItem.enabled = true;
    }
    public void TakeArtifactFromPlayer()
    {
        activeChosenArtifactId = 0;
        chosenItem.enabled = false;
    }
    public int GetChosenArtifactId()
    {
        return activeChosenArtifactId;
    }
    public void SaveArtifactsToSaveData()
    {
        SaveData saveData = GameContext.activeSave;
        for(int i = 0; i < UNACTIVE_SLOTS_COUNT; i++)
        {
            saveData.inventory_unactive_items[i] = (uint)unactiveSlots[i].artifact_id;
        }
        for (int i = 0; i < ACTIVE_SLOTS_COUNT; i++)
        {
            saveData.inventory_active_items[i] = (uint)activeSlots[i].artifact_id;
        }
    }
}
