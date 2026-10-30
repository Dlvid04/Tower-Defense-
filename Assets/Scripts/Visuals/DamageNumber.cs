using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DamageNumber : Billboard
{
    float speed = 0.5f;
    TMP_Text text;
    public void SetDamage(float damage)
    {
        text = GetComponent<TMP_Text>();
        text.text = Mathf.Round(damage) + "";
    }
    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * speed;
        transform.localScale = Vector3.one * ( 1 - (speed / 20));
        speed += speed * 0.05f;
        if (speed > 20)
        {
            Destroy(gameObject);
        }
    }
}
