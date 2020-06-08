using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static StageSelector instance;

    // 스크롤 바
    public Scrollbar scrollbar;

    // 스크롤 내 content
    public Transform contentTr;

    // 스테이지를 표시할 텍스트
    public Text StageText;

    [SerializeField] Sprite rewardNotSuccessImg;
    [SerializeField] Sprite rewardSuccessImg;

    // 스크롤 할 컨탠츠 갯수
    const int SIZE = 17;

    // 현재 어디 스크롤인지 저장하기 위함
    float[] pos = new float[SIZE];

    // 현재 각 스크롤 컨탠츠 간의 간격
    float distance, targetPos, curPos;
    
    // 현재 어떤 페이지 인가
    int targetIndex;

    // 드래그 중인지 확인
     bool isDrag;

    private void Awake()
    {
        instance = this;

        // init

        // 스크롤 벨류 측정
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++) pos[i] = distance * i;
    }

    void Start()
    {
        //TabClick(5);
    }


    void Update()
    {

        // 드래그 안하고 있을 시 targetpos로 이동 >> 마우스 빠르게 움직일때 페이지 위치 복구용
        if (!isDrag)
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);

        // 순간적으로 모이는 현상 제거
        if (Time.time < 0.1f) return;

        // 해당 페이지에오면 처리할것은 여기서~~~~
        for (int i = 0; i < SIZE; i++)
        {
            if (i == targetIndex)
            {
                StageText.text = contentTr.GetChild(i).name;
            }
        }
    }

    // 본 스크립트의 contentTr이 모든 스테이지 버튼의 부모 오브젝트임을 이용
    // 전달된 idx번 스테이지 버튼의 interactive를 셋팅하는 함수
    //public void setInteractiveStageBtn(int idx, bool interactive = false)
    public void setStageBtnStatus(int idx, Sprite gradeImg, bool interactive = false, bool isStageSuccess = false)
    {
        GameObject obj = contentTr.GetChild(idx).gameObject;
        Button btn = obj.GetComponentInChildren<Button>();
        Image gradeImgSlot = obj.transform.GetChild(1).GetComponent<Image>();
        Text rewardText = obj.transform.GetChild(3).GetComponent<Text>();

        btn.interactable = interactive;

        // 해당 버튼에 성적 이미지 리소스 삽입
        // 성적이 없는 경우에도 투명한 이미지를 전달(Grade.None)받음
        gradeImgSlot.sprite = gradeImg;

        // 이미 보상 수령한 경우를 표시
        if(isStageSuccess)
        {
            Image rewardImg = obj.transform.GetChild(2).GetComponent<Image>();
            rewardImg.sprite = rewardSuccessImg;
        }
        else
        {
            Image rewardImg = obj.transform.GetChild(2).GetComponent<Image>();
            rewardImg.sprite = rewardNotSuccessImg;
        }

        // 해당 스테이지의 보상 표기
        rewardText.text = $"{StageManager.instance.Level[idx / 4].stage[idx % 4].getReward()}";


        if (interactive)
        {
            // 활성화된 해당 스테이지 버튼을 화면 중앙으로 불러옴
            StageSelector.instance.TabClick(idx);

        }
    }


    public void selectStage()
    {
        //선택한 스테이지 정보 저장
        StageManager.instance.SetStage();

        //// 선택한 스테이지의 인덱스 값 저장
        //GradeManager.instance.setStartStageIdx(targetIndex);

        //씬 변경
        SceneManager.LoadSceneAsync("Ingame");
        //SceneManager.LoadSceneAsync(2);
    }

    public int getTargetIndex() => targetIndex;
    public void setTargetIndex(int idx) => targetIndex = idx;


    // 절반거리를 기준으로 가까운 위치를 반환하는 함수
    float SetPos()
    {
        for (int i = 0; i < SIZE; i++)
        {
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        }
        return 0;
    }

    // 드래그 시작w
    public void OnBeginDrag(PointerEventData eventData) => curPos = SetPos();

    // 드래그 중
    public void OnDrag(PointerEventData eventData) => isDrag = true;

    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
    
        isDrag = false;
        targetPos = SetPos();

        //스크롤 절반이 넘지 않아도 마우스 속도가 빠르다면 넘어가도록
        if (curPos == targetPos)
        {
            if (eventData.delta.x > 18 && curPos - distance >= 0)
            {
                --targetIndex;
                targetPos = curPos - distance;
            }
            else if (eventData.delta.x < -18 && curPos + distance <= 1.01f)
            {
                ++targetIndex;
                targetPos = curPos + distance;
            }
        }
    }

    // 0 ~ 16 (idx)
    public void TabClick(int n)
    {
        targetIndex = n;
        targetPos = pos[n];


        //// 마지막 스테이지가 오픈되어있다면, 업그레이드 초기화 기능 지원
        //if(n == 16)
        //{
        //    UpgradeManager.instance.resetUpgradeBtn.SetActive(true);
        //}
    }

    /// <summary>
    /// Mainmenu 씬에서 버튼 클릭시 레벨 단위로 탭 이동
    /// </summary>
    /// <param name="">true: 다음으로. false: 이전으로</param>
    public void TabMove(bool isNext)
    {
        // 상위 레벨로 가면서 더 갈 수 있는 버튼이 있다면
        if (isNext && targetIndex != 16)
        {
            TabClick((targetIndex / 4 + 1) * 4);
        }
        else if (!isNext && targetIndex != 0)
        {
            TabClick(((targetIndex - 1) / 4) * 4);
        }
    }


}
