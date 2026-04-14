using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RunBasedBuff : MonoBehaviour, IPointerClickHandler
{
    public int id;
    public Image image;
    public string description;

    public void Set_Data(int id)
    {
        Buff buffData = BuffsManager.Instance.GetRunBasedBuff(id);
        image.sprite = buffData.sprite;
        description = buffData.description;
        this.id = id;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameContext.selectedRunBasedBuff = this;
        RunBasedBuffsPage.Instance.UpdateSelectedBuff(this);
    }
}
