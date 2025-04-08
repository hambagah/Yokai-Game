using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DamageNumItem : MonoBehaviour
{
    public Vector3 speed;

    public float lifetime = 1f; // ����ʱ��

    public TMP_Text damageText; // TextMeshPro ������ʾ����
    public RectTransform rt;

    public void Start()
    {
        StartCoroutine(FadeOutAndDestroy());
    }
    void Update()
    {
        // ÿ֡�����ƶ�
        rt.position += speed * Time.deltaTime;

        // ��ѡ����������Ч�����罥����
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