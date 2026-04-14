using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    //public Artifact artifact = null;
    public int artifact_id = 0; //0 means no artifact in slot
    public Image item_sprite;
    private void Awake()
    {
        if(artifact_id == 0) item_sprite.enabled = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        int chosenArtifactId = GameContext.inventory.GetChosenArtifactId();
        //if this slot has artifact then take it with cursor
        if (artifact_id != 0 && chosenArtifactId == 0)
        {
            if (gameObject.CompareTag("CraftSlot"))
            {
                //update craft if artifact is placed in craft slot
                GameContext.inventory.UpdateCraft();
            }
            else if (gameObject.CompareTag("CraftResultSlot"))
            {
                //use artifacts if crafted item is taken
                GameContext.inventory.UseCraftItems();
            }
            else if (gameObject.CompareTag("ActiveSlot"))
            {
                //remove buffs if artifact is taken from active slot
                RemoveArtifactBuffs();
            }
            GameContext.inventory.TakeItemFromSlot(artifact_id);
            GameContext.inventory.HideTooltip();
            artifact_id = 0;
            item_sprite.enabled = false;
        }
        //else if cursor holds an item then place it to this slot
        else if (chosenArtifactId != 0 && artifact_id == 0 && !gameObject.CompareTag("CraftResultSlot"))
        {
            artifact_id = chosenArtifactId;
            GameContext.inventory.TakeArtifactFromPlayer();
            OnPointerEnter(null);
            item_sprite.sprite = ArtifactsManager.Instance.GetArtifact(artifact_id).sprite;
            item_sprite.enabled = true;
            if (gameObject.CompareTag("CraftSlot"))
            {
                GameContext.inventory.UpdateCraft();
            }
            else if (gameObject.CompareTag("ActiveSlot"))
            {
                //apply buffs if artifact is placed in active slot
                ApplyArtifactBuffs();
            }
        }
        //exchange items
        
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
            GameContext.playerStats.ApplyNewBuff(BuffsManager.Instance.GetArtifactBuff(buff_id));
        }
        GameContext.inventory.UpdateInventoryText();
    }
    private void RemoveArtifactBuffs()
    {
        Artifact artifact = ArtifactsManager.Instance.GetArtifact(artifact_id);
        foreach (var buff_id in artifact.buff_id_list)
        {
            GameContext.playerStats.RemoveBuff(BuffsManager.Instance.GetArtifactBuff(buff_id));
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
