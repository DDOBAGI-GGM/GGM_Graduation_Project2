using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class ChatContainer : MonoBehaviour
{
    public MemberProfile currentHuman;
    public List<ChatTree> chatTrees = new List<ChatTree>();

    public ChatTree GetChatTree(string name)
    {
        foreach (ChatTree tree in chatTrees)
        {
            if (tree.humanInfo.name == name)
                return tree;
        }

        return null;
    }
}
