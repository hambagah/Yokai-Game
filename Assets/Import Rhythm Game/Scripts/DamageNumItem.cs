using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DamageNumItem : MonoBehaviour
{
    public Vector3 speed;

    public float lifetime = 1f; // 持续时间

    public TMP_Text damageText; // TextMeshPro 用于显示数字
    public RectTransform rt;

    public void Start()
    {
        StartCoroutine(FadeOutAndDestroy());
    }
    void Update()
    {
        // 每帧向上移动
        rt.position += speed * Time.deltaTime;

        // 可选：增加其他效果（如渐隐）
        //FadeOut();
    }
    private IEnumerator FadeOutAndDestroy()
    {
        float elapsed = 0f;
        Color originalColor = damageText.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}