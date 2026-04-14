using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public float change_time = 5.0f;
    private float last_change_time;
    private void Start()
    {
        last_change_time = Time.time;
    }
    void Update()
    {
        if(Time.time - last_change_time > change_time)
        {
            last_change_time = Time.time;
            float dX = Random.Range(0, 4) * 4.0f;
            transform.localPosition = new Vector3(-12.0f + dX, -3.92f, -0.1f);
        }
    }
}
