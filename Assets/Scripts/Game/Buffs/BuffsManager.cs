using System.Collections.Generic;
using UnityEngine;

public class BuffsManager : MonoBehaviour
{
    public static BuffsManager Instance;
    public BuffSprites buffSprites;
    public BuffSprites runBasedBuffSprites;

    private List<Buff> buffsData;
    private List<Buff> runBasedBuffs;
    private List<Buff> artifactBuffs;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitBuffsData();
            InitRunBasedBuffs();
            InitArtifactBuffs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitBuffsData()
    {
        buffsData = new List<Buff>()
        {
            //HP
            new Buff(BuffType.HP, 20f, 1, "Health I: Basic HP is increased by 20 points", 1, 20, buffSprites.sprites[0]),
            new Buff(BuffType.HP, 40f, 2, "Health II: Basic HP is increased by 60 points",  2, 80, buffSprites.sprites[1]),
            new Buff(BuffType.HP, 60f, 3, "Health III: Basic HP is increased by 120 points",  3, 80, buffSprites.sprites[2]),
            new Buff(BuffType.HP, 80f, 4, "Health IV: Basic HP is increased by 200 points",  4, 80, buffSprites.sprites[3]),
            new Buff(BuffType.HP, 100f, 5, "Health V: Basic HP is increased by 300 points",  5, 80, buffSprites.sprites[4]),
            //Mana
            new Buff(BuffType.MANA, 20f, 1, "Mana I: Basic Mana is increased by 20 points",  1, 80, buffSprites.sprites[5]),
            new Buff(BuffType.MANA, 40f, 2, "Mana II: Basic Mana is increased by 60 points",  2, 80, buffSprites.sprites[6]),
            new Buff(BuffType.MANA, 60f, 3, "Mana III: Basic Mana is increased by 120 points", 3, 80,  buffSprites.sprites[7]),
            new Buff(BuffType.MANA, 80f, 4, "Mana IV: Basic Mana is increased by 200 points", 4, 80,  buffSprites.sprites[8]),
            new Buff(BuffType.MANA, 100f, 5, "Mana V: Basic Mana is increased by 300 points", 5, 80,  buffSprites.sprites[9]),
            //Stamina
            new Buff(BuffType.STAMINA, 25f, 1, "Stamina I: Basic Stamina is increased by 25 points", 4, 80,  buffSprites.sprites[10]),
            new Buff(BuffType.STAMINA, 25f, 2, "Stamina II: Basic Stamina is increased by 50 points",  5, 80, buffSprites.sprites[11]),
            //Defence
            new Buff(BuffType.DEF_BUFF, 10f, 1, "Defence I: Defence is increased by 10 points",  2, 80, buffSprites.sprites[12]),
            new Buff(BuffType.DEF_BUFF, 15f, 2, "Defence II: Defence is increased by 25 points", 3, 80,  buffSprites.sprites[13]),
            new Buff(BuffType.DEF_BUFF, 15f, 3, "Defence III: Defence is increased by 40 points",  4, 80, buffSprites.sprites[14]),
            //Crit Rate
            new Buff(BuffType.CRIT_RATE, 10f, 1, "Crit Rate I: Crit Rate is increased by 10%", 3, 80,  buffSprites.sprites[15]),
            new Buff(BuffType.CRIT_RATE, 15f, 2, "Crit Rate II: Crit Rate is increased by 25%",  4, 80, buffSprites.sprites[16]),
            //Crit Damage
            new Buff(BuffType.CRIT_DMG, 25f, 1, "Crit Dmg I: Crit Damage is increased by 25%",  3, 80, buffSprites.sprites[17]),
            new Buff(BuffType.CRIT_DMG, 25f, 2, "Crit Dmg II: Crit Damage is increased by 50%",  4, 80, buffSprites.sprites[18])
        };
    }
    private void InitRunBasedBuffs()
    {
        runBasedBuffs = new List<Buff>()
        {
            //bow dmg up
            new Buff(BuffType.MAGIC_BUFF, 20f, 1, "MagicBow Dmg I: MagicBow damage is increased by 20%", runBasedBuffSprites.sprites[0]),
            new Buff(BuffType.MAGIC_BUFF, 20f, 2, "MagicBow Dmg II: MagicBow damage is increased by 40%", runBasedBuffSprites.sprites[1]),
            new Buff(BuffType.MAGIC_BUFF, 20f, 3, "MagicBow Dmg III: MagicBow damage is increased by 60%", runBasedBuffSprites.sprites[2]),
            //sword dmg up
            new Buff(BuffType.SWORD_BUFF, 20f, 1, "Sword Dmg I: Sword damage is increased by 20%", runBasedBuffSprites.sprites[3]),
            new Buff(BuffType.SWORD_BUFF, 20f, 2, "Sword Dmg II: Sword damage is increased by 40%", runBasedBuffSprites.sprites[4]),
            new Buff(BuffType.SWORD_BUFF, 20f, 3, "Sword Dmg III: Sword damage is increased by 60%", runBasedBuffSprites.sprites[5]),
            //Crit Dmg
            new Buff(BuffType.CRIT_DMG, 10f, 1, "Crit Dmg I: Crit Dmg is increased by 10%", runBasedBuffSprites.sprites[6]),
            new Buff(BuffType.CRIT_DMG, 10f, 2, "Crit Dmg II: Crit Dmg is increased by 20%", runBasedBuffSprites.sprites[7]),
            new Buff(BuffType.CRIT_DMG, 10f, 3, "Crit Dmg III: Crit Dmg is increased by 30%", runBasedBuffSprites.sprites[8]),
            //Crit Rate
            new Buff(BuffType.CRIT_RATE, 5f, 1, "Crit Rate I: Crit Rate is increased by 5%", runBasedBuffSprites.sprites[9]),
            new Buff(BuffType.CRIT_RATE, 5f, 2, "Crit Rate II: Crit Rate is increased by 10%", runBasedBuffSprites.sprites[10]),
            new Buff(BuffType.CRIT_RATE, 5f, 3, "Crit Rate III: Crit Rate is increased by 15%", runBasedBuffSprites.sprites[11]),
            //HP full restoration
            new Buff(BuffType.HP_REFILL, 0f, 1, "Fully restores HP", runBasedBuffSprites.sprites[12]),
            //HP regen
            new Buff(BuffType.HP_REGEN, 5f, 1, "HP Regen I: 5% of HP is restored each time a level is completed", runBasedBuffSprites.sprites[13]),
            new Buff(BuffType.HP_REGEN, 5f, 2, "HP Regen II: 10% of HP is restored each time a level is completed", runBasedBuffSprites.sprites[14]),
            new Buff(BuffType.HP_REGEN, 5f, 3, "HP Regen III: 15% of HP is restored each time a level is completed", runBasedBuffSprites.sprites[15]),
            //HP up
            new Buff(BuffType.HP_UP, 20f, 1, "HP Up I: Basic max HP is increased by 20%", runBasedBuffSprites.sprites[16]),
            new Buff(BuffType.HP_UP, 20f, 2, "HP Up II: Basic max HP is increased by 40%", runBasedBuffSprites.sprites[17]),
            new Buff(BuffType.HP_UP, 20f, 3, "HP Up III: Basic max HP is increased by 60%", runBasedBuffSprites.sprites[18]),
            //Mana regen
            new Buff(BuffType.MANA_REGEN, 20f, 1, "Mana Regen I: Mana Regen is increased by 20%", runBasedBuffSprites.sprites[19]),
            new Buff(BuffType.MANA_REGEN, 30f, 2, "Mana Regen II: Mana Regen is increased by 50%", runBasedBuffSprites.sprites[20]),
            new Buff(BuffType.MANA_REGEN, 50f, 3, "Mana Regen III: Mana Regen is increased by 100%", runBasedBuffSprites.sprites[21]),
            //Mana up
            new Buff(BuffType.MANA_UP, 20f, 1, "Mana Up I: Basic max Mana is increased by 20%", runBasedBuffSprites.sprites[22]),
            new Buff(BuffType.MANA_UP, 20f, 2, "Mana Up II: Basic max Mana is increased by 40%", runBasedBuffSprites.sprites[23]),
            new Buff(BuffType.MANA_UP, 20f, 3, "Mana Up III: Basic max Mana is increased by 60%", runBasedBuffSprites.sprites[24]),
            //Stamina regen
            new Buff(BuffType.STAMINA_REGEN, 20f, 1, "Stamina Regen I: Stamina Regen is increased by 20%", runBasedBuffSprites.sprites[25]),
            new Buff(BuffType.STAMINA_REGEN, 30f, 2, "Stamina Regen II: Stamina Regen is increased by 50%", runBasedBuffSprites.sprites[26]),
            new Buff(BuffType.STAMINA_REGEN, 50f, 3, "Stamina Regen III: Stamina Regen is increased by 100%", runBasedBuffSprites.sprites[27]),
            //Stamina up
            new Buff(BuffType.STAMINA_UP, 20f, 1, "Stamina Up I: Basic max Stamina is increased by 20%", runBasedBuffSprites.sprites[28]),
            new Buff(BuffType.STAMINA_UP, 20f, 2, "Stamina Up II: Basic max Stamina is increased by 40%", runBasedBuffSprites.sprites[29]),
            new Buff(BuffType.STAMINA_UP, 20f, 3, "Stamina Up III: Basic max Stamina is increased by 60%", runBasedBuffSprites.sprites[30]),

            //artifact(will change later)
            new Buff(BuffType.ARTIFACT, 20f, 3, "Get artifact: \"Soul in a bottle\"", runBasedBuffSprites.sprites[31]),

        };
    }
    private void InitArtifactBuffs()
    {
        artifactBuffs = new List<Buff>()
        {
            new Buff(BuffType.MANA_REGEN, 20.0f, 1, null, null),
            new Buff(BuffType.MANA, 20.0f, 1, null, null),
            new Buff(BuffType.SWORD_BUFF, 10.0f, 1, null, null),
            new Buff(BuffType.CRIT_RATE, 10.0f, 1, null, null),
            new Buff(BuffType.DMG_REDUCTION, 10.0f, 1, null, null),
            new Buff(BuffType.HP_UP, 10.0f, 1, null, null),
            new Buff(BuffType.MAGIC_BUFF, 10.0f, 1, null, null),
            new Buff(BuffType.MANA_UP, 10.0f, 1, null, null)
        };
    }
    public Buff GetBuff(int id)
    {
        return buffsData[id];
    }
    public Buff GetRunBasedBuff(int id)
    {
        return runBasedBuffs[id];
    }
    public Buff GetArtifactBuff(int id)
    {
        return artifactBuffs[id];
    }
}
