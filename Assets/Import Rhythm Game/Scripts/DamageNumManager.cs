using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumManager : MonoBehaviour
{
    public static DamageNumManager st;
    public DamageNumItem perfectItem; // UI 预制体
    public DamageNumItem goodItem;
    public DamageNumItem badItem;
    public DamageNumItem missItem;
    public Transform missPos;
    public Canvas canvas; // 需要在 Inspector 中指定 Canvas

    private void Awake()
    {
        st = this;
    }

    public void Create(NoteHitType type, Vector3 pos)
    {
        if (type == NoteHitType.Miss)
            pos = missPos.position;
        // 将世界空间位置转换为屏幕空间位置
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);


        // 判断是否在相机视野内
        if (screenPos.z > 0) // 确保位于相机前方
        {
            // 在 Canvas 下实例化 UI 预制体
            DamageNumItem damageNumPrefab = null;
            switch(type)
            {
                case NoteHitType.Perfect:
                    damageNumPrefab = perfectItem;
                    break;
                case NoteHitType.Good:
                    damageNumPrefab = goodItem;
                    break;
                case NoteHitType.Bad:
                    damageNumPrefab = badItem;
                    break;
                case NoteHitType.Miss:
                    damageNumPrefab = missItem;
                    break;

            }
            DamageNumItem item = Instantiate(damageNumPrefab, canvas.transform);
            // 设置伤害数值
            item.rt.position = screenPos;
        }
    }
}
