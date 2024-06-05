using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public enum ESaveLocation
    {
        NotSave,
        JH, // Jihyen
        JW, // Junwon
        HS, // Hyensuck
        CM, // Cheamin
        HG, // HyenGue
        DY, // Deayang

        Teacher,
        Friend,
        Home
    }

    public enum EChatState
    {
        Other = 0,      // ?브퀗??酉귥????턄 嶺뚮씭??????
        Me = 1,      // ?筌먦끆?€뤆?쎛 嶺뚮씭??????
        Ask = 2,        // ?筌먦끆?€뤆?쎛 ??좉텭???롪퍒???
        LoadNext        // ????怨룹꽑嶺?????? ???덈펲嶺? ???섎?????????      // Ask ?????㏉꺑 ??????낅쭑 ?띠럾???쒑땻?????깅쾳 ?筌먲퐘遊??怨삵룖??.
    }

    public enum EChatType
    {
        Text,
        Image,
        CutScene,
        Question,
        LockQuestion
    }

    public enum EFace
    {
        Default,        // ??쒕샑筌??
        Blush,          // ?獄?낯??(???ㅻ젛??紐욤뚯궪彛?)
        Difficult       // ???????⑥ъ쓤?? ???걞???
    }

    public enum EChatEvent
    {
        Default,
        Vibration,      // ???⑸츩??嶺뚯쉳?х뙴?
        Round,      // ???逾??怨뺣뼺???怨삵룖??     
        Camera     // ?곸궠?筌????節뗪땁 ?影??꽑?낅슣?딁뵳?
    }

    [Serializable]
    public class Chat      // ?熬? ?????嶺뚮씭흮????濡ル츎?띠럾?
    {
        public EChatState state;     // 嶺뚮씭?????롪퍒???????
        public EChatType type;
        public string text;        // 嶺???濡ル츎 ??
        public EFace face;       // 嶺??????踰???戮곗젧
        public bool isCan;
        public List<EChatEvent> textEvent = new List<EChatEvent>();
    }

    [Serializable]
    public class AskAndReply
    {
        public string ask;        // ??좊닔???????덈츎 ??ルㅎ臾며춯?뼿
        public List<Chat> reply = new List<Chat>();     // ?잙갭梨룩굢??????????援?
        public bool is_UseThis;     // ??????덈츎嶺뚯솘?
        public bool isChange;
        public string changeName;
    }

    [Serializable]
    public class LockAskAndReply
    {
        public List<string> evidence = new List<string>();
        public string ask;        // ??좊닔???????덈츎 ??ルㅎ臾며춯?뼿
        public List<Chat> reply = new List<Chat>();     // ?잙갭梨룩굢??????????援?
        public bool is_UseThis;     // ??????덈츎嶺뚯솘?
    }

    [Serializable]
    public class Chapter
    {
        public string showName;     // ?곌랜?삭굢?彛????藥?
        public ESaveLocation saveLocation;     // ?熬곥룗?든춯?뼿
        public List<Chat> chat = new List<Chat>();         // 嶺?????
        public List<AskAndReply> askAndReply = new List<AskAndReply>();           // 嶺뚯쉶?꾣룇??
        public List<LockAskAndReply> lockAskAndReply = new List<LockAskAndReply>();       // ??ル맧??嶺뚯쉶?꾣룇??
        public List<string> round = new List<string>();           // 嶺뚯빘鍮볠뤃?얠뿉??????琉우꽑

        public bool isChapterEnd;     // is this chapter ended?
        public bool isCan;            // can this chapter play?
        public bool is_nextChapter;     // is this chapter have next chapter?
        public string nextChapterName;         // next chapter name
    }

    public class ChatStruct : MonoBehaviour { }
}