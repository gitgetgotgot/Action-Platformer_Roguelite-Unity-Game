using UnityEngine;

public class SwordTraceController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private readonly string TRACE_DOWN_STATE = "Trace_Down";
    private readonly string TRACE_UP_STATE = "Trace_Up";
    private readonly string TRACE_FORWARD_STATE = "Trace_Forward";
    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }
    public void ActivateVFX(int index)
    {
        spriteRenderer.enabled = true;
        if (index == 1)
        {
            animator.Play(TRACE_DOWN_STATE, 0, 0.0f);
        }
        else if (index == 2)
        {
            animator.Play(TRACE_UP_STATE, 0, 0.0f);
        }
        else
        {
            animator.Play(TRACE_FORWARD_STATE, 0, 0.0f);
        }
    }
    public void DeactivateVFX()
    {
        spriteRenderer.enabled = false;
    }
}
