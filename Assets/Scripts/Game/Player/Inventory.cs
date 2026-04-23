using TMPro;
using UnityEditor.Experimental;
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
    public TMP_Text magic_dmg_bonus;
    public Button constUpgradesButton;
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
    [Header("Guide")]
    public GameObject guideTooltip;
    public GameObject craftingInfo;
    public GameObject notification;
    public InventorySlot infoSlot1;
    public InventorySlot infoSlot2;
    public InventorySlot infoResultSlot;
    [Header("Recycle")]
    public InventorySlot recycleSlot;
    public GameObject recycleMessage;
    public TMP_Text shardsCount;
    public Button approve;
    public Button decline;

    private int activeChosenArtifactId = 0;
    private int recycleActiveArtifactID = 0;
    private InventorySlot[] unactiveSlots;
    private InventorySlot[] activeSlots;
    private const int UNACTIVE_SLOTS_COUNT = 10;
    private const int ACTIVE_SLOTS_COUNT = 4;
    private void Awake()
    {
        chosenItem.enabled = false;
        tooltip.gameObject.SetActive(false);
        guideTooltip.SetActive(false);
        recycleMessage.SetActive(false);
        approve.onClick.AddListener(ApproveHandler);
        decline.onClick.AddListener(DeclineHandler);

        unactiveSlots = InventorySlots.GetComponentsInChildren<InventorySlot>();
        activeSlots = ActiveSlots.GetComponentsInChildren<InventorySlot>();
        constUpgradesButton.onClick.AddListener(OpenConstBuffsPage);
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

        hp.text = "HP " + stats.hp.ToString("0.0") + " / " + stats.maxHP.ToString("0.0");
        mana.text = "Mana " + stats.mana.ToString("0.0") + " / " + stats.maxMANA.ToString("0.0");
        stamina.text = "Stamina " + stats.stamina.ToString("0.0") + " / " + stats.maxStamina.ToString("0.0");
        def.text = "Def " + stats.def.ToString("0.0");
        crit_rate.text = "Crit Rate " + stats.crit_rate.ToString("0.0") + "%";
        crit_dmg.text = "Crit Dmg " + stats.crit_dmg.ToString("0.0") + "%";
        dmg_reduction.text = "Dmg Reduction " + (stats.dmg_reduction * 100f).ToString("0.0") + "%";
        mana_regen_bonus.text = "Mana Regen Bonus " + (stats.mana_regen * 100f - 100f).ToString("0.0") + "%";
        stamina_regen_bonus.text = "Stamina Regen Bonus " + (stats.stamina_regen * 100f - 100f).ToString("0.0") + "%";
        sword_dmg_bonus.text = "Sword Dmg Bonus " + (stats.sword_dmg_mlpr * 100f - 100f).ToString("0.0") + "%";
        magic_dmg_bonus.text = "Magic Dmg Bonus " + (stats.magic_dmg_mlpr * 100f - 100f).ToString("0.0") + "%";
    }

    public void InitInventorySlots()
    {

    }
    public void OpenConstBuffsPage()
    {
        GameMenuScript.Instance.OpenConstBuffsPage();
    }
    public void AddArtifact(int artifactID)
    {
        //add artifact to inventory if free slot is available
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
    public void ShowGuideTooltip(int craftable_artifact_id)
    {
        guideTooltip.SetActive(true);
        if (craftable_artifact_id == 0)
        {
            notification.SetActive(true);
            craftingInfo.SetActive(false);
            return;
        }
        notification.SetActive(false);
        craftingInfo.SetActive(true);
        Artifact artifact = ArtifactsManager.Instance.GetArtifact(craftable_artifact_id);
        int child1_id = artifact.child1_artifact_id;
        int child2_id = artifact.child2_artifact_id;
        infoSlot1.AddArtifact(child1_id);
        infoSlot2.AddArtifact(child2_id);
        infoResultSlot.AddArtifact(craftable_artifact_id);
    }
    public void HideGuideTooltip()
    {
        guideTooltip.SetActive(false);
    }
    public void ShowRecycleBox(int artifact_id)
    {
        recycleMessage.SetActive(true);
        recycleActiveArtifactID = artifact_id;
        shardsCount.text = "x" + ArtifactsManager.Instance.GetArtifact(artifact_id).cost.ToString();
    }
    public void HideRecycleBox(bool break_artifact)
    {
        recycleSlot.RemoveArtifact();
        recycleMessage.SetActive(false);
        
        if (break_artifact)
        {
            GameContext.playerStats.GetMoney(ArtifactsManager.Instance.GetArtifact(recycleActiveArtifactID).cost);
        }
        else
        {
            AddArtifact(recycleActiveArtifactID);
        }
    }
    private void ApproveHandler()
    {
        HideRecycleBox(true);
    }
    private void DeclineHandler()
    {
        HideRecycleBox(false);
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
