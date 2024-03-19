using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class test
{
    public string text;
    public List<test> next = new List<test>();
}

[CreateAssetMenu(fileName = "dialogueSO", menuName = "SO",  order = 0)]
public class DialogueSO : ScriptableObject
{
    public int chapter;
    public List<test> temp = new List<test>();

}
