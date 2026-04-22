using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunBasedBuffsPage : MonoBehaviour
{
    public static RunBasedBuffsPage Instance;
    public Button selectButton;
    public RunBasedBuff[] availableBuffs; //3 buffs
    public GameObject buffFrame;
    public TMP_Text buffDescription;
    public RectTransform pageTransform;

    private bool isMoving = false;
    private bool isMovingUp = true;
    private float pageUpY = 0f;
    private float pageDownY = -430f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        UpdateAvailableRunBuffs(availableBuffs[0].id, availableBuffs[1].id, availableBuffs[2].id);
        selectButton.onClick.AddListener(LearnChosenBuff);
        GameContext.selectedRunBasedBuff = availableBuffs[1];
        buffDescription.text = BuffsManager.Instance.GetRunBasedBuff(availableBuffs[1].id).description;
    }
    private void Update()
    {
        if (isMoving)
        {
            if (isMovingUp)
            {
                if (pageTransform.localPosition.y >= pageUpY)
                {
                    isMovingUp = false;
                    isMoving = false;
                    //Time.timeScale = 0f;
                    return;
                }
                //float v_multiplier = (pageUpY - pageTransform.localPosition.y) / 430f;
                pageTransform.Translate(0f, 80f * Time.deltaTime, 0f);
            }
            else
            {
                if (pageTransform.localPosition.y <= pageDownY)
                {
                    isMovingUp = true;
                    isMoving = false;
                    ClosePage();
                    return;
                }
                //float v_multiplier = (pageTransform.localPosition.y + 430f) / 430f;
                pageTransform.Translate(0f, -80f * Time.deltaTime, 0f);
            }
        }
    }
    private void OnEnable()
    {
        isMoving = true;
    }
    public void LearnChosenBuff()
    {
        int new_buff_id = GameContext.selectedRunBasedBuff.id;
        GameContext.activeSave.runBuffs.Add((uint)new_buff_id);
        GameContext.playerStats.ManageNewBuff(BuffsManager.Instance.GetRunBasedBuff(new_buff_id), true);
        AudioMixerManager.Instance.PlaySound(14);
        isMoving = true;
        //Time.timeScale = 1f;
    }
    public void UpdateAvailableRunBuffs(int first_id, int second_id, int third_id)
    {
        availableBuffs[0].Set_Data(first_id);
        availableBuffs[1].Set_Data(second_id);
        availableBuffs[2].Set_Data(third_id);
        buffDescription.text = BuffsManager.Instance.GetRunBasedBuff(availableBuffs[1].id).description;
    }
    public void UpdateSelectedBuff(RunBasedBuff buff)
    {
        buffFrame.transform.position = buff.transform.position;
        buffDescription.text = buff.description;
    }
    public void ClosePage()
    {
        gameObject.SetActive(false);
        GameContext.gameState = GameState.inGame;
    }
}
