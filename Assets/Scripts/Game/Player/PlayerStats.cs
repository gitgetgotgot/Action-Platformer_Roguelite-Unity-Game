using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    private float basicHP = 200f, basicDEF = 0f, basicMANA = 100f, basicStamina = 100f; 
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
    public float dmg_reduction;
    public float basic_crit_dmg, basic_crit_rate;
    public float crit_dmg, crit_rate;
    public float crit_dmg_bonus, crit_rate_bonus;
    public float sword_basic_dmg, magic_basic_dmg;
    public float sword_ultimate_basic_dmg, magic_ultimate_basic_dmg;
    public float sword_dmg_mlpr, magic_dmg_mlpr;
    //SKILLS
    public bool hasDoubleJump = false, hasPlungeAttack = false;
    //ULTIMATE
    public float ultimate_stacks = 0;
    public float ultimate_stacks_max = 10;

    public int level = 0, money = 0, current_xp = 0, xp_for_next_lvl = 0, first_lvl_xp_needed = 100;

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
            ManageNewBuff(BuffsManager.Instance.GetBuff((int)const_buff_id), true);
        //apply available temporary buffs
        foreach (var run_based_buff_id in GameContext.activeSave.runBuffs)
            ManageNewBuff(BuffsManager.Instance.GetRunBasedBuff((int)run_based_buff_id), true);
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
                    ManageNewBuff(BuffsManager.Instance.GetArtifactBuff(buff_id), true);
                }
            }
        }
    }
    private void Init_Start_Stats() {
        hp = maxHP = basicHP;
        def = basicDEF;
        mana = maxMANA = basicMANA;
        stamina = maxStamina = basicStamina;

        dmg_reduction = 0f;
        crit_dmg = basic_crit_dmg;
        crit_rate = basic_crit_rate;
        sword_dmg_mlpr = magic_dmg_mlpr = 1.0f;
        sword_basic_dmg = 25f;
        magic_basic_dmg = 50f;
        sword_ultimate_basic_dmg = 75f;
        magic_ultimate_basic_dmg = 100f;

        hp_bonus = def_bonus = mana_bonus = stamina_bonus = 0.0f;
        crit_dmg_bonus = crit_rate_bonus = 0.0f;

        hp_mplr = def_mplr = mana_mplr = stamina_mplr = 1.0f;
        walk_speed = attack_speed = 1.0f;
        hp_regen = 0.0f;
        mana_regen = stamina_regen = 1.0f;

        //mana and stamina regeneration is 5% of max value in 1 second
        mana_regen_delta = maxMANA * 0.1f;
        stamina_regen_delta = maxStamina * 0.1f;

        ultimate_stacks = ultimate_stacks_max;
        hud_manager.UpdateWeaponHolder(ultimate_stacks == ultimate_stacks_max, ultimate_stacks / ultimate_stacks_max);
    }
    public void UpdateStatsOnLevelFinished()
    {
        //heal player by hp_regen% of maxHP
        if(hp_regen > 0) Heal(maxHP * hp_regen);
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
    public void Get_Ultimate_Stack()
    {
        ultimate_stacks += 0.5f; //each hit gives 0.5 stacks
        if (ultimate_stacks > ultimate_stacks_max)
            ultimate_stacks = ultimate_stacks_max;
        hud_manager.UpdateWeaponHolder(ultimate_stacks == ultimate_stacks_max, ultimate_stacks / ultimate_stacks_max);
    }
    public bool Activate_Ultimate()
    {
        if (ultimate_stacks == ultimate_stacks_max)
        {
            ultimate_stacks = 0;
            hud_manager.UpdateWeaponHolder(ultimate_stacks == ultimate_stacks_max, ultimate_stacks / ultimate_stacks_max);
            return true;
        }
        else return false;
    }
    public void Heal(float value)
    {
        hp += value;
        if (hp > maxHP) hp = maxHP;
        hud_manager.Set_Hp_Bar(hp / maxHP);
    }
    public void Update_max_hp() {
        maxHP = (basicHP + hp_bonus) * hp_mplr;
        hud_manager.Set_Hp_Bar(hp / maxHP);
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

    public bool IsCritHit()
    {
        return Random.Range(0, 1000) < (int)(crit_rate * 10f); //[0.0%, 100.0%]
    }
    public float GetSwordDamage(bool isUltimateDamage, bool isCrit)
    {
        if (isUltimateDamage)
        {
            if (isCrit) return sword_ultimate_basic_dmg * sword_dmg_mlpr * (1.0f + crit_dmg * 0.01f);
            else return sword_ultimate_basic_dmg * sword_dmg_mlpr;
        }
        else
        {
            if (isCrit) return sword_basic_dmg * sword_dmg_mlpr * (1.0f + crit_dmg * 0.01f);
            else return sword_basic_dmg * sword_dmg_mlpr;
        }
    }
    public float GetMagicDamage(bool isUltimateDamage, bool isCrit)
    {
        if(isUltimateDamage)
        {
            if (isCrit) return magic_ultimate_basic_dmg * magic_dmg_mlpr * (1.0f + crit_dmg * 0.01f);
            else return magic_ultimate_basic_dmg * magic_dmg_mlpr;
        }
        else
        {
            if (isCrit) return magic_basic_dmg * magic_dmg_mlpr * (1.0f + crit_dmg * 0.01f);
            else return magic_basic_dmg * magic_dmg_mlpr;
        }
    }

    public void Update_Stats() {
        mana += mana_regen_delta * mana_regen * Time.deltaTime;
        if(mana > maxMANA) mana = maxMANA;

        stamina += stamina_regen_delta * stamina_regen * Time.deltaTime;
        if(stamina > maxStamina) stamina = maxStamina;

        hud_manager.Set_Mana_Bar(mana / maxMANA);
        hud_manager.Set_Stamina_Bar(stamina / maxStamina);
    }
    public void ManageNewBuff(Buff buff, bool apply) {
        // apply = true - add buff
        // apply = false - remove buff
        switch(buff.buffType)
        {
            //BUFFS
            case BuffType.HP:
                {
                    if (apply)
                    {
                        hp_bonus += buff.power; hp += buff.power;
                    }
                    else
                    {
                        hp_bonus -= buff.power; hp -= buff.power;
                    }
                    Update_max_hp();
                    break;
                }
            case BuffType.HP_REGEN:
                {
                    if (apply) hp_regen += buff.power * 0.01f;
                    else hp_regen -= buff.power * 0.01f;
                    break;
                }
            case BuffType.HP_UP:
                {
                    if (apply)
                    {
                        hp_mplr += buff.power * 0.01f; hp *= hp_mplr;
                    }
                    else
                    {
                        hp *= 1 / hp_mplr; hp_mplr -= buff.power * 0.01f; hp *= hp_mplr;
                    }
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
                    if(apply)
                    {
                        mana_bonus += buff.power; mana += buff.power;
                    }
                    else
                    {
                        mana_bonus -= buff.power; mana -= buff.power;
                    }
                    Update_max_mana();
                    break;
                }
            case BuffType.MANA_REGEN:
                {
                    if (apply) mana_regen += buff.power * 0.01f;
                    else mana_regen -= buff.power * 0.01f;
                    break;
                }
            case BuffType.MANA_UP:
                {
                    if (apply) mana_mplr += buff.power * 0.01f;
                    else mana_mplr -= buff.power * 0.01f;
                    Update_max_mana();
                    break;
                }
            case BuffType.DEF_BUFF:
                {
                    if (apply) def_bonus += buff.power;
                    else def_bonus -= buff.power;
                    Update_def();
                    break;
                }
            case BuffType.DMG_REDUCTION:
                {
                    if (apply) dmg_reduction += buff.power * 0.01f;
                    else dmg_reduction -= buff.power * 0.01f;
                    break;
                }
            case BuffType.STAMINA:
                {
                    if (apply)
                    {
                        stamina_bonus += buff.power; stamina += buff.power;
                    }
                    else
                    {
                        stamina_bonus -= buff.power; stamina -= buff.power;
                    }
                    Update_max_stamina();
                    break;
                }
            case BuffType.STAMINA_REGEN:
                {
                    if (apply) stamina_regen += buff.power * 0.01f;
                    else stamina_regen -= buff.power * 0.01f;
                    break;
                }
            case BuffType.STAMINA_UP:
                {
                    if (apply) stamina_mplr += buff.power * 0.01f;
                    else stamina_mplr -= buff.power * 0.01f;
                    Update_max_stamina();
                    break;
                }
            case BuffType.CRIT_DMG:
                {
                    if (apply) crit_dmg_bonus += buff.power;
                    else crit_dmg_bonus -= buff.power;
                    Update_Crit_Dmg();
                    break;
                }
            case BuffType.CRIT_RATE:
                {
                    if (apply) crit_rate_bonus += buff.power;
                    else crit_rate_bonus -= buff.power;
                    Update_Crit_Rate();
                    break;
                }
            case BuffType.SWORD_BUFF:
                {
                    if (apply) sword_dmg_mlpr += buff.power * 0.01f;
                    else sword_dmg_mlpr -= buff.power * 0.01f;
                    break;
                }
            case BuffType.MAGIC_BUFF:
                {
                    if (apply) magic_dmg_mlpr += buff.power * 0.01f;
                    else magic_dmg_mlpr -= buff.power * 0.01f;
                    break;
                }

            //SKILLS
            case BuffType.DOUBLE_JUMP:
                {
                    if(apply) hasDoubleJump = true;
                    else hasDoubleJump = false;
                    break;
                }
            case BuffType.PLUNGE_ATTACK:
                {
                    if(apply) hasPlungeAttack = true;
                    else hasPlungeAttack = false;
                    break;
                }
            case BuffType.ARTIFACT:
                {
                    //lvl here is used as artifact ID
                    GameContext.inventory.AddArtifact(buff.lvl);
                    break;
                }
        }

    }
}
