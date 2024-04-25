using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExpressionState
{
    Defualt = 0, // �⺻ ��ǥ��
    Embarrassed = 1, // ��Ȳ�� ǥ��
    Sadness = 2 // ����? ǥ��
}

public enum HumanStage
{
    HyeonSeok = 0,
    Jihyeon= 1,
    Junwon = 2,             
    Suyeon = 3,
    Daeyang = 4
}

[Serializable]
public struct ProfileChat
{
    public ExpressionState expressionState;
    public HumanStage humanStage;
    public SpriteRenderer[] spriteRenderers;
}

[CreateAssetMenu(fileName = "ChatSO", menuName = "SO/ProfileSO")]
public class ProfileSO : ScriptableObject
{
    public ProfileChat state;
}
