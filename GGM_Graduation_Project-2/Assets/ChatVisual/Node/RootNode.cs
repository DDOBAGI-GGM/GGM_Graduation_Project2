using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class RootNode : Node
    {
        public Node child;

        [TextArea] public string description;

        public string showName;
        public ESaveLocation saveLocation;
        public List<string> round = new List<string>();

        public bool is_nextChapter;
        public string nextChapterIndex;

        protected override void OnStart()
        {

        }
    }
}
