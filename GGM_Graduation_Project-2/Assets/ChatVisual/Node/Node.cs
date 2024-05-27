using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public abstract class Node
    {
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        public int index;
        public Label indexLabel;

        //public virtual Node Clone()     // ��� �����ϴ� �Լ�
        //{
        //    return Instantiate(this);
        //}

        protected abstract void OnStart();
    }
}
