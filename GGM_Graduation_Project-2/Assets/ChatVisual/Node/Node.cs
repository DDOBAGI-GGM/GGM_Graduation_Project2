using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    [System.Serializable]
    public class Hierarchy     // �ٷ� ���� ��
    {
        //public Vector3 moveToPosition;
        //public Vector2 lastSpotPosition;
    }

    public abstract class Node
    {
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [TextArea] public string description;
        public int index;
        public Label indexLabel;

        //public virtual Node Clone()     // ��� �����ϴ� �Լ�
        //{
        //    return Instantiate(this);
        //}

        protected abstract void OnStart();
    }
}
