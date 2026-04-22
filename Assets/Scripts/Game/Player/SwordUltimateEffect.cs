using UnityEngine;

public class SwordUltimateEffect : MonoBehaviour
{
    private Animator animator;
    private readonly string EFFECT_STATE_NAME = "Effect";
    void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }
    public void ActivateEffect()
    {
        gameObject.SetActive(true);
        animator.Play(EFFECT_STATE_NAME);
    }
    public void DeactivateEffect()
    {
        gameObject.SetActive(false);
    }
}
