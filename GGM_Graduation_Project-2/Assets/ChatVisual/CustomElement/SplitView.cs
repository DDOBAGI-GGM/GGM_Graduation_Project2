using UnityEngine.UIElements;

namespace ChatVisual
{
    public class SplitView : TwoPaneSplitView       // �ڽ��� �� ���� ���� �Ǵ� ���� â�� �迭�ϴ� �����̳�
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
        public new class UxmlTraits : TwoPaneSplitView.UxmlTraits { }
    }
}
