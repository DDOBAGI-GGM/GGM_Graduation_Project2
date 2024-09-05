using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RSLinkedData : MonoBehaviour
{
    // Main
    private UIReader_Relationship mainSystem;
    public List<RSLinkedData> linkdataList = new List<RSLinkedData>();
    private LineRenderer lineRenderer;

    // Line
    private GameObject movedDot;
    private BoxCollider2D movedDotCollider;
    private EdgeCollider2D lineEdgeCollider;
    [SerializeField] private List<Vector2> linePosList = new List<Vector2>();

    private Vector2 dotOriginPos;
    private bool is_Hold = false;
    [SerializeField] private int nowLineIndex = 0;
    private int totalLineIndex = 0;

    public RSLinkedData touchObj;
    public bool is_LinkedObject = false;

    private void Awake()
    {
        mainSystem = GetComponentInParent<UIReader_Relationship>();
        lineRenderer = GetComponent<LineRenderer>();
        lineEdgeCollider = GetComponent<EdgeCollider2D>();
    }

    private void Start()
    {
        linkdataList.Add(this);
    }

    private void OnMouseDown()
    {
        Vector3 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Debug.Log(Vector2.Distance(mousePos, transform.position));
        if (Vector2.Distance(mousePos, transform.position) > 0.65f)     // 선을 건들였음.
        {
            if (movedDot == null) return;

            float close = 5;
            int index = -1;
            for (int i = 0; i < linePosList.Count; i++)
            {
                if (Vector2.Distance(mousePos, linePosList[i]) < close)     // 가장 가까운 곳의 포지션 찾기
                {
                    close = Vector2.Distance(mousePos, linePosList[i]);
                    index = i;
                }
            }
            if (index % 2 == 0) { index++; }
            Debug.Log(index);

            movedDot.transform.position = mousePos;

            // 데이터 연동 해제
            linkdataList[index].GetComponent<RSLinkedData>().linkdataList.Remove(this);
            linkdataList.RemoveAt(index);

            // 선 콜라이더 해제
            linePosList.RemoveAt(index + 1);
            linePosList.RemoveAt(index);

           // LineAndColliderChange();

            nowLineIndex = index - 1;
        }

        if (movedDot == null)
        {
            movedDot = Instantiate(mainSystem.linkLineDotPrefabs, transform.position, Quaternion.identity, gameObject.transform);
            movedDot.GetComponent<RSDot>().myData = this;
            movedDotCollider = movedDot.GetComponent<BoxCollider2D>();
            dotOriginPos = movedDot.transform.localPosition;
        }
        else
        {
            movedDotCollider.enabled = true;
        }

        lineRenderer.positionCount += 1;
        nowLineIndex++;
        lineRenderer.SetPosition(nowLineIndex, Vector2.zero);

        is_Hold = true;
    }

    private void OnMouseUp()
    {
        if (movedDot == null) return;
        movedDot.transform.localPosition = dotOriginPos;     // 원위치

        if (is_LinkedObject == true && linkdataList.Contains(touchObj) == false)
        {
            lineRenderer.SetPosition(nowLineIndex, transform.InverseTransformPoint(touchObj.transform.position));
            if (linePosList.Count == 2) linePosList.RemoveAt(1);
            linePosList.Add(transform.InverseTransformPoint(touchObj.transform.position));
            linePosList.Add(new Vector2(0, 0));     // 위치 2개 추가해주기

            linkdataList.Add(touchObj);
            touchObj.GetComponent<RSLinkedData>().linkdataList.Add(this);     // 서로 넣어주기

            lineRenderer.positionCount++;
            nowLineIndex++;
        }
        else
        {
            lineRenderer.positionCount -= 1;
            nowLineIndex--;
            lineRenderer.SetPosition(nowLineIndex, Vector2.zero);

            if (linePosList.Count < 2)
            {
                linePosList.Add(Vector2.zero);
                nowLineIndex = 0;
            }
        }
        LineAndColliderChange();

        movedDotCollider.enabled = false;
        is_Hold = false;
    }

    private void LineAndColliderChange()
    {
        lineRenderer.positionCount = linePosList.Count;
        lineRenderer.SetPositions(ConvertVertor2ToVector3(linePosList).ToArray());      // 선 바꾸기
        lineEdgeCollider.points = linePosList.ToArray();
    }

    private void Update()
    {
        if (is_Hold)
        {
            Vector2 mousePos;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            movedDot.transform.localPosition = transform.InverseTransformPoint(mousePos);
            lineRenderer.SetPosition(nowLineIndex, MovedLineAdjust(transform.InverseTransformPoint(mousePos)));
        }
    }

    private Vector2 MovedLineAdjust(Vector2 originPos)
    {
        return new Vector2(originPos.x - 0.075f, originPos.y - 0.1f);
    }

    private List<Vector3> ConvertVertor2ToVector3(List<Vector2> vector2)
    {
        List<Vector3> vector3 = new List<Vector3>();
        foreach (Vector2 v in vector2)
        {
            vector3.Add(v);
        }
        return vector3;
    }

    private void ChangeLinePosition()
    {
        int dataCount = 1;
        for (int i = 1; i < linePosList.Count; i += 2)
        {
            linePosList[i] = transform.InverseTransformPoint(linkdataList[dataCount].gameObject.transform.position);
            dataCount++;
        }
        LineAndColliderChange();
    }

    public void ChangeOtherLinePosition()
    {
        for (int i = 1; i < linkdataList.Count; i++)
        {
            linkdataList[i].ChangeLinePosition();
        }
    }
}