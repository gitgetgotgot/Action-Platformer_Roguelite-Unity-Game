using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstBuffsPage : MonoBehaviour
{
    public static ConstBuffsPage Instance;
    [Header("Main objects")]
    public Button activationButton;
    public TMP_Text activationButtonText;
    public Button exitButton;
    public GameObject buffsHolder;
    public GameObject buffFrame;
    public ConstBuff[] constBuffs;
    public Sprite buffBackActiveSprite;
    public TMP_Text buffDescription;
    [Header("Player info")]
    public TMP_Text moneyAmount;
    public TMP_Text lvlText;
    [Header("Requirements objects")]
    public GameObject requirements;
    public TMP_Text requirementsTextLvl;
    public TMP_Text requirementsTextMoney;

    private Vector3 prevMousePos;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        activationButton.onClick.AddListener(LearnNewConstBuff);
        exitButton.onClick.AddListener(Close);
        GameContext.selectedConstBuff = constBuffs[0];
        buffDescription.text = BuffsManager.Instance.GetBuff(constBuffs[0].id).description;
        foreach(var constBuff in constBuffs)
        {
            Buff buff = BuffsManager.Instance.GetBuff(constBuff.id);
            constBuff.Set_Sprite();
            //activate buff if player already has it
            foreach (var playerBuffId in GameContext.activeSave.constBuffs)
            {
                Buff playerConstBuff = BuffsManager.Instance.GetBuff((int)playerBuffId);
                if(playerConstBuff.buffType == buff.buffType && playerConstBuff.lvl == buff.lvl)
                {
                    constBuff.isLearned = true;
                    constBuff.GetComponent<Image>().sprite = buffBackActiveSprite;
                }
            }
        }
        UpdateSelectedBuff(constBuffs[0]);
    }

    private void OnEnable()
    {
        moneyAmount.text = GameContext.playerStats.money.ToString();
        lvlText.text = "Lvl " + GameContext.playerStats.level.ToString();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            prevMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 moveVector = currentMousePos - prevMousePos;
            prevMousePos = currentMousePos;
            buffsHolder.transform.Translate(moveVector, Space.Self);
        }
    }
    private void LearnNewConstBuff()
    {
        ConstBuff selectedBuff = GameContext.selectedConstBuff;
        //can learn this skill only if previous is learned
        if (selectedBuff.prevSkill.isLearned && !selectedBuff.isLearned && selectedBuff)
        {
            Buff newBuff = BuffsManager.Instance.GetBuff(selectedBuff.id);
            if (newBuff.required_lvl > GameContext.playerStats.level || newBuff.cost > GameContext.playerStats.money) return;
            
            GameContext.activeSave.constBuffs.Add((uint)selectedBuff.id);
            GameContext.playerStats.ApplyNewBuff(newBuff);
            GameContext.playerStats.SpendMoney(newBuff.cost);
            selectedBuff.isLearned = true;
            selectedBuff.GetComponent<Image>().sprite = buffBackActiveSprite;

            moneyAmount.text = GameContext.playerStats.money.ToString();
            UpdateSelectedBuff(selectedBuff);
            AudioMixerManager.Instance.PlaySound(7);
        }
        
    }
    private void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void UpdateSelectedBuff(ConstBuff buff)
    {
        buffFrame.transform.position = buff.transform.position;
        buffDescription.text = BuffsManager.Instance.GetBuff(buff.id).description;
        if (!buff.isLearned)
        {
            Buff buffInfo = BuffsManager.Instance.GetBuff(buff.id);
            requirements.SetActive(true);
            requirementsTextLvl.text = "Requirements: Lvl " + buffInfo.required_lvl.ToString();
            requirementsTextMoney.text = buffInfo.cost.ToString();

            activationButton.GetComponent<Image>().color = Color.yellow;
            activationButtonText.text = "Activate";
        }
        else
        {
            requirements.SetActive(false);

            activationButton.GetComponent<Image>().color = Color.green;
            activationButtonText.text = "Activated";
        }
    }
}
