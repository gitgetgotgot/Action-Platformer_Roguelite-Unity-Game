using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float lifetime = 2f;
    public float fade_offset = 1f;
    public float y_move_distance = 1f;
    public float max_scale = 2f;

    private TMP_Text dmg_text;
    private RectTransform rectTransform;

    private Color usual_color = new(0.75f, 0.72f, 0.63f);
    private Color critical_color = new(0.83f, 0.19f, 0.0f);

    private float activation_time;
    private float fade_time;
    private float delta_y_move;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        dmg_text = GetComponent<TMP_Text>();
        fade_time = lifetime - fade_offset;
        delta_y_move = y_move_distance / lifetime;
        gameObject.SetActive(false);
    }
    void Update()
    {
        float deltaTime = Time.time - activation_time;
        if(deltaTime > lifetime)
        {
            gameObject.SetActive(false);
            DamageTextPoolManager.instance.AddDamageTextToFree(this);
            return;
        }
        if (deltaTime >= fade_offset)
        {
            dmg_text.color = new Color(dmg_text.color.r, dmg_text.color.g, dmg_text.color.b, 1.0f - (deltaTime - fade_offset) / fade_time);
        }
        rectTransform.Translate(0, Time.deltaTime * delta_y_move, 0, Space.World);
        rectTransform.localScale = (deltaTime / lifetime * max_scale + 1.0f) * Vector3.one;
    }

    public void SetDamageText(float dmg, bool isCrit, Vector3 startPos)
    {
        dmg_text.text = ((int)dmg).ToString();
        if (isCrit)
        {
            dmg_text.color = critical_color;
            dmg_text.fontStyle = FontStyles.Bold;
        }
        else
        {
            dmg_text.color = usual_color;
            dmg_text.fontStyle = FontStyles.Normal;
        }
        startPos.y += 1f;
        rectTransform.position = startPos;
        rectTransform.localScale = Vector3.one;
        activation_time = Time.time;
        gameObject.SetActive(true);
    }
}
