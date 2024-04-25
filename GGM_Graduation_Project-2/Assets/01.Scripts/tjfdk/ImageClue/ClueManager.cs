using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ClueManager : Singleton<ClueManager>
{
    [SerializeField] private Text description;

    public void Texting(string msg)
    {
        description.DOKill();
        // ���� �ؽ�Ʈ ����
        description.text = "";
        //��Ʈ������ �ؽ�Ʈ �ۼ�
        description.DOText(msg, 1.5f);
    }
}
