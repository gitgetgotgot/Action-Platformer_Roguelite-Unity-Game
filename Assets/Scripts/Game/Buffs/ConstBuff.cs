using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConstBuff : MonoBehaviour, IPointerClickHandler
{
    public bool isLearned;
    public int id;
    public ConstBuff prevSkill;
    public Image buffImage;
    public void Set_Sprite()
    {
        buffImage.sprite = BuffsManager.Instance.GetBuff(id).sprite;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameContext.selectedConstBuff = this;
        ConstBuffsPage.Instance.UpdateSelectedBuff(this);
    }
}
