using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumManager : MonoBehaviour
{
    public static DamageNumManager st;
    public DamageNumItem perfectItem; // UI Ԥ����
    public DamageNumItem goodItem;
    public DamageNumItem badItem;
    public DamageNumItem missItem;
    public Transform missPos;
    public Canvas canvas; // ��Ҫ�� Inspector ��ָ�� Canvas

    private void Awake()
    {
        st = this;
    }

    public void Create(NoteHitType type, Vector3 pos)
    {
        if (type == NoteHitType.Miss)
            pos = missPos.position;
        // ������ռ�λ��ת��Ϊ��Ļ�ռ�λ��
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);


        // �ж��Ƿ��������Ұ��
        if (screenPos.z > 0) // ȷ��λ�����ǰ��
        {
            // �� Canvas ��ʵ���� UI Ԥ����
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
            // �����˺���ֵ
            item.rt.position = screenPos;
        }
    }
}
