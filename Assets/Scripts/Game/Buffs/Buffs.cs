using UnityEngine;

public enum BuffType
{
    HP, MANA, STAMINA,
    HP_UP, MANA_UP, STAMINA_UP,
    HP_REGEN, MANA_REGEN, STAMINA_REGEN, ATTACK_SPEED_BUFF, WALK_SPEED_BUFF,
    DEF_BUFF, DMG_REDUCTION, DMG_EVADE,
    CRIT_DMG, CRIT_RATE,
    SWORD_BUFF, BOW_BUFF, MAGIC_BUFF,
    DOUBLE_JUMP, PLUNGE_ATTACK, HP_REFILL,

    ARTIFACT
}

[System.Serializable]
public class Buff
{
    public Buff(BuffType type, float power, int lvl, string description, int playerLvl, int cost, Sprite sprite)
    {
        this.buffType = type;
        this.power = power;
        this.lvl = lvl;
        this.description = description;
        this.required_lvl = playerLvl;
        this.cost = cost;
        this.sprite = sprite;
    }
    public Buff(BuffType type, float power, int lvl, string description, Sprite sprite)
    {
        this.buffType = type;
        this.power = power;
        this.lvl = lvl;
        this.description = description;
        this.sprite = sprite;
    }
    public Buff() { }

    public Sprite sprite;
    public BuffType buffType;
    public float power;
    public int lvl;
    public string description;
    public int required_lvl;
    public int cost;
}

