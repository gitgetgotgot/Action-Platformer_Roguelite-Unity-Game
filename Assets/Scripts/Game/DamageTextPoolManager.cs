using System.Collections.Generic;
using UnityEngine;

public class DamageTextPoolManager : MonoBehaviour
{
    public static DamageTextPoolManager instance;
    public GameObject damageTextPrefab;
    public int initial_pool_size = 20;

    private List<DamageText> free_texts = new();
    private void Awake()
    {
        instance = this;
        for(int i = 0; i < initial_pool_size; i++)
        {
            CreateNewDamageText();
        }
    }
    private DamageText CreateNewDamageText()
    {
        DamageText dt = Instantiate(damageTextPrefab, transform).GetComponent<DamageText>();
        AddDamageTextToFree(dt);
        return dt;
    }
    public void ActivateDamageText(float dmg, bool isCrit, Vector3 startPos)
    {
        if (free_texts.Count == 0)
            CreateNewDamageText();

        DamageText dt = free_texts[0];
        dt.SetDamageText(dmg, isCrit, startPos);
        RemoveDamageTextFromFree(dt);
    }
    public void ActivatePlayerDamageText(float dmg, Vector3 startPos)
    {
        if (free_texts.Count == 0)
            CreateNewDamageText();

        DamageText dt = free_texts[0];
        dt.SetPlayerDamageText(dmg, startPos);
        RemoveDamageTextFromFree(dt);
    }
    public void RemoveDamageTextFromFree(DamageText damageText)
    {
        free_texts.Remove(damageText);
    }
    public void AddDamageTextToFree(DamageText damageText)
    {
        free_texts.Add(damageText);
    }
}
