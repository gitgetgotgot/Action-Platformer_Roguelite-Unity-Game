using UnityEditor.Experimental;
using UnityEngine;

public class PlayerStats {
    public PlayerStats() {
        basicHP = 200f; basicDEF = 0f; basicMANA = 100f; basicStamina = 100f;
        basic_crit_dmg = 50f; basic_crit_rate = 5f;
        level = 0; money = 0; first_lvl_xp_needed = 100;
    }
    //BUFFS
    //basic stats
    private float basicHP, basicDEF, basicMANA, basicStamina; 
    public float hp, def, mana, stamina;
    public float maxHP, maxMANA, maxStamina;
    public float hp_bonus, def_bonus, mana_bonus, stamina_bonus;
    public float hp_mplr, def_mplr, mana_mplr, stamina_mplr;
    //regen stats
    public float hp_regen, mana_regen, stamina_regen;
    public float hp_regen_delta, mana_regen_delta, stamina_regen_delta;
    //speed stats
    public float walk_speed, attack_speed;
    //dmg stats
    public float basic_crit_dmg, basic_crit_rate;
    public float crit_dmg, crit_rate;
    public float crit_dmg_bonus, crit_rate_bonus;
    public float sword_dmg_mlpr, bow_dmg_mlpr, magic_dmg_mlpr;
    //SKILLS
    public bool hasDoubleJump = false, hasPlungeAttack = false;

    public int level, money, current_xp, xp_for_next_lvl, first_lvl_xp_needed;

    private HUD_Manager hud_manager;
    public void Init_Stats(HUD_Manager HUD_Manager) {
        level = GameContext.activeSave.level;
        money = GameContext.activeSave.money;
        current_xp = GameContext.activeSave.xp;
        xp_for_next_lvl = (level + 1) * first_lvl_xp_needed;

        hud_manager = HUD_Manager;
        hud_manager.Set_Money_Count(money);
        hud_manager.Set_LVL(level);

        Init_Start_Stats();
        //apply available const buffs
        foreach (var const_buff_id in GameContext.activeSave.constBuffs)
            ApplyNewBuff(BuffsManager.Instance.GetBuff((int)const_buff_id));
        //apply available temporary buffs
        foreach (var run_based_buff_id in GameContext.activeSave.runBuffs)
            ApplyNewBuff(BuffsManager.Instance.GetRunBasedBuff((int)run_based_buff_id));
        //apply unactive artifacts
        for(int i = 0; i < 8; i++) {
            uint unactive_slot_artifact_id = GameContext.activeSave.inventory_unactive_items[i];
            if (unactive_slot_artifact_id != 0)
            {
                GameContext.inventory.AddArtifactToUnactiveSlot((int)unactive_slot_artifact_id, i);
            }
        }
        //apply active artifacts and apply their buffs
        for(int i = 0; i < 4; i++) {
            uint active_slot_artifact_id = GameContext.activeSave.inventory_active_items[i];
            if (active_slot_artifact_id != 0)
            {
                GameContext.inventory.AddArtifactToActiveSlot((int)active_slot_artifact_id, i);
                Artifact artifact = ArtifactsManager.Instance.GetArtifact((int)active_slot_artifact_id);
                foreach (var buff_id in artifact.buff_id_list)
                {
                    ApplyNewBuff(BuffsManager.Instance.GetArtifactBuff(buff_id));
                }
            }
        }
    }
    private void Init_Start_Stats() {
        hp = maxHP = basicHP;
        def = basicDEF;
        mana = maxMANA = basicMANA;
        stamina = maxStamina = basicStamina;
        crit_dmg = basic_crit_dmg;
        crit_rate = basic_crit_rate;
        sword_dmg_mlpr = bow_dmg_mlpr = magic_dmg_mlpr = 1.0f;

        hp_bonus = def_bonus = mana_bonus = stamina_bonus = 0.0f;
        crit_dmg_bonus = crit_rate_bonus = 0.0f;

        hp_mplr = def_mplr = mana_mplr = stamina_mplr = 1.0f;
        walk_speed = attack_speed = 1.0f;
        hp_regen = mana_regen = stamina_regen = 1.0f;
        //mana and stamina regeneration is 5% of max value in 1 second
        mana_regen_delta = maxMANA * 0.1f;
        stamina_regen_delta = maxStamina * 0.1f;
    }
    public void GetXP(int amount)
    {
        current_xp += amount;
        if(current_xp >= xp_for_next_lvl)
        {
            level++;
            current_xp -= xp_for_next_lvl;
            xp_for_next_lvl = (level + 1) * first_lvl_xp_needed;
            hud_manager.Set_LVL(level);
        }
    }
    public void GetMoney(int amount)
    {
        money += amount;
        hud_manager.Set_Money_Count(money);
    }
    public void SpendMoney(int amount)
    {
        money -= amount;
        hud_manager.Set_Money_Count(money);
    }
    public void ClearMoney()
    {
        money = 0;
    }
    public void Update_max_hp() {
        maxHP = (basicHP + hp_bonus) * hp_mplr;
    }
    public void Update_max_mana() {
        maxMANA = (basicMANA + mana_bonus) * mana_mplr;
        mana_regen_delta = maxMANA * 0.1f;
    }
    public void Update_def() {
        def = (basicDEF + def_bonus) * def_mplr;
    }
    public void Update_max_stamina() {
        maxStamina = (basicStamina + stamina_bonus) * stamina_mplr;
        stamina_regen_delta = maxStamina * 0.1f;
    }
    public void Update_Crit_Dmg() {
        crit_dmg = basic_crit_dmg + crit_dmg_bonus;
    }
    public void Update_Crit_Rate() {
        crit_rate = basic_crit_rate + crit_rate_bonus;
    }

    public void Update_Stats() {
        mana += mana_regen_delta * mana_regen * Time.deltaTime;
        if(mana > maxMANA) mana = maxMANA;

        stamina += stamina_regen_delta * stamina_regen * Time.deltaTime;
        if(stamina > maxStamina) stamina = maxStamina;

        hud_manager.Set_Mana_Bar(mana / maxMANA);
        hud_manager.Set_Stamina_Bar(stamina / maxStamina);
    }
    public void ApplyNewBuff(Buff buff) {
        switch(buff.buffType)
        {
            //BUFFS
            case BuffType.HP:
                {
                    hp_bonus += buff.power; hp += buff.power;
                    hud_manager.Set_Hp_Bar(hp / maxHP);
                    Update_max_hp();
                    break;
                }
            case BuffType.HP_REGEN:
                {
                    hp_regen += buff.power;
                    break;
                }
            case BuffType.HP_UP:
                {
                    hp_mplr += buff.power * 0.01f; hp *= hp_mplr;
                    Update_max_hp();
                    break;
                }
            case BuffType.HP_REFILL:
                {
                    hp = maxHP;
                    hud_manager.Set_Hp_Bar(hp / maxHP);
                    break;
                }
            case BuffType.MANA:
                {
                    mana_bonus += buff.power; mana += buff.power;
                    Update_max_mana();
                    break;
                }
            case BuffType.MANA_REGEN:
                {
                    mana_regen += buff.power * 0.01f;
                    break;
                }
            case BuffType.MANA_UP:
                {
                    mana_mplr += buff.power * 0.01f;
                    Update_max_mana();
                    break;
                }
            case BuffType.DEF_BUFF:
                {
                    def_bonus += buff.power;
                    Update_def();
                    break;
                }
            case BuffType.STAMINA:
                {
                    stamina_bonus += buff.power; stamina += buff.power;
                    Update_max_stamina();
                    break;
                }
            case BuffType.STAMINA_REGEN:
                {
                    stamina_regen += buff.power * 0.01f;
                    break;
                }
            case BuffType.STAMINA_UP:
                {
                    stamina_mplr += buff.power * 0.01f;
                    Update_max_stamina();
                    break;
                }
            case BuffType.CRIT_DMG:
                {
                    crit_dmg_bonus += buff.power;
                    Update_Crit_Dmg();
                    break;
                }
            case BuffType.CRIT_RATE:
                {
                    crit_rate_bonus += buff.power;
                    Update_Crit_Rate();
                    break;
                }
            case BuffType.SWORD_BUFF:
                {
                    sword_dmg_mlpr += buff.power * 0.01f;
                    break;
                }
            case BuffType.BOW_BUFF:
                {
                    bow_dmg_mlpr += buff.power * 0.01f;
                    break;
                }
            case BuffType.MAGIC_BUFF:
                {
                    magic_dmg_mlpr += buff.power * 0.01f;
                    break;
                }

            //SKILLS
            case BuffType.DOUBLE_JUMP:
                {
                    hasDoubleJump = true;
                    break;
                }
            case BuffType.PLUNGE_ATTACK:
                {
                    hasPlungeAttack = true;
                    break;
                }
            case BuffType.ARTIFACT:
                {
                    GameContext.inventory.AddArtifact(1);
                    break;
                }
        }

    }
    public void RemoveBuff(Buff buff)
    {
        switch(buff.buffType)
        {
            case BuffType.MANA:
                {
                    mana_bonus -= buff.power; mana -= buff.power;
                    Update_max_mana();
                    break;
                }
            case BuffType.MANA_REGEN:
                {
                    mana_regen -= buff.power * 0.01f;
                    break;
                }
        }
    }
}
