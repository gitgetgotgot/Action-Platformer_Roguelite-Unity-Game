using UnityEngine;

public class SwordUltimateController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private readonly string EFFECT_STATE_NAME = "Effect";
    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }
    public void ActivateEffect()
    {
        spriteRenderer.enabled = true;
        animator.Play(EFFECT_STATE_NAME, 0, 0.0f);
    }
    public void DeactivateEffect()
    {
        spriteRenderer.enabled = false;
    }
}
