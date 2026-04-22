using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public InventorySlotType slotType = InventorySlotType.isSimpleSlot;
    public int artifact_id = 0; //0 means no artifact in slot
    public Image item_sprite;
    private void Awake()
    {
        if(artifact_id == 0) item_sprite.enabled = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotType == InventorySlotType.isInfoSlot) return;

        int chosenArtifactId = GameContext.inventory.GetChosenArtifactId();
        //if this slot has artifact then take it with cursor
        if (artifact_id != 0 && chosenArtifactId == 0)
        {
            if (slotType == InventorySlotType.isCraftSlot)
            {
                //update craft if artifact is placed in craft slot
                GameContext.inventory.UpdateCraft();
            }
            else if (slotType == InventorySlotType.isCraftResultSlot)
            {
                //use artifacts if crafted item is taken
                GameContext.inventory.UseCraftItems();
            }
            else if (slotType == InventorySlotType.isActiveSlot)
            {
                //remove buffs if artifact is taken from active slot
                RemoveArtifactBuffs();
            }
            else if(slotType == InventorySlotType.isGuideSlot)
            {
                GameContext.inventory.HideGuideTooltip();
            }

            GameContext.inventory.TakeItemFromSlot(artifact_id);
            GameContext.inventory.HideTooltip();
            artifact_id = 0;
            item_sprite.enabled = false;
        }
        //else if cursor holds an item then place it to this slot
        else if (chosenArtifactId != 0 && artifact_id == 0 && slotType != InventorySlotType.isCraftResultSlot)
        {
            artifact_id = chosenArtifactId;
            GameContext.inventory.TakeArtifactFromPlayer();
            OnPointerEnter(null);
            Artifact artifact = ArtifactsManager.Instance.GetArtifact(artifact_id);
            item_sprite.sprite = artifact.sprite;
            item_sprite.enabled = true;
            if (slotType == InventorySlotType.isCraftSlot)
            {
                GameContext.inventory.UpdateCraft();
            }
            else if (slotType == InventorySlotType.isActiveSlot)
            {
                //apply buffs if artifact is placed in active slot
                ApplyArtifactBuffs();
            }
            else if (slotType == InventorySlotType.isGuideSlot)
            {
                GameContext.inventory.ShowGuideTooltip(artifact.craftableArtifactId);
            }
            else if(slotType == InventorySlotType.isRecycleSlot)
            {
                GameContext.inventory.ShowRecycleBox(artifact_id);
            }
        }
        
    }
    public void AddArtifact(int artifact_id)
    {
        this.artifact_id = artifact_id;
        item_sprite.sprite = ArtifactsManager.Instance.GetArtifact(artifact_id).sprite;
        item_sprite.enabled = true;
    }
    public void RemoveArtifact()
    {
        artifact_id = 0;
        item_sprite.enabled = false;
    }
    private void ApplyArtifactBuffs()
    {
        Artifact artifact = ArtifactsManager.Instance.GetArtifact(artifact_id);
        foreach (var buff_id in artifact.buff_id_list)
        {
            GameContext.playerStats.ManageNewBuff(BuffsManager.Instance.GetArtifactBuff(buff_id), true);
        }
        GameContext.inventory.UpdateInventoryText();
    }
    private void RemoveArtifactBuffs()
    {
        Artifact artifact = ArtifactsManager.Instance.GetArtifact(artifact_id);
        foreach (var buff_id in artifact.buff_id_list)
        {
            GameContext.playerStats.ManageNewBuff(BuffsManager.Instance.GetArtifactBuff(buff_id), false);
        }
        GameContext.inventory.UpdateInventoryText();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (artifact_id != 0)
        {
            Vector3 pos = GetComponent<RectTransform>().localPosition;
            pos.x -= 168f;
            GameContext.inventory.ShowTooltip(ArtifactsManager.Instance.GetArtifact(artifact_id), pos);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(artifact_id != 0) GameContext.inventory.HideTooltip();
    }
}
