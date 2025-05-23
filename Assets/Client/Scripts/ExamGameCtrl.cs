using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BLG
{
    public class ExamGameCtrl : MonoBehaviour
    {
        #region 成员
        public TextMeshProUGUI gameTitle;

        public GameObject gameInfoRoot;
        public TextMeshProUGUI gameInfo;

        public GameObject roundRoot;
        public TextMeshProUGUI roundNum;
        public NormalBt backBt;
        public GameObject timeBar;
        public Image timeBarValue;

        public GameObject PlayTrans;
        public GameObject DesTrans;
        public GameObject FixationImage;
        public GameObject bgFront;
        public GameObject bgBack;

        public NormalBt leftBt;
        public TextMeshProUGUI leftBtText;
        public NormalBt centerBt;
        public TextMeshProUGUI centerBtText;
        public NormalBt rightBt;
        public TextMeshProUGUI rightBtText;

        public GameObject carDesc; public NormalBt carDescBt;
        public GameObject stroopDesc; public NormalBt stroopDescBt;
        public GameObject giraffeDesc; public NormalBt giraffeDescBt;
        public GameObject switcherDesc; public NormalBt switcherDescBt;
        public GameObject memoryDesc; public NormalBt memoryDescBt;
        public GameObject flankerDesc; public NormalBt flankerDescBt;
                


        public GameObject carExam;
        //public GameObject carExam_stim1;
        //public GameObject carExam_stim2;

        public GameObject stroopExam;
        //public GameObject stroopExam_stim1;
        //public GameObject stroopExam_stim2;

        public GameObject giraffeExam;
        //public GameObject giraffeExam_stim1;
        //public GameObject giraffeExam_stim2;

        public GameObject switcherExam;
        //public GameObject switcherExam_stim1;
        //public GameObject switcherExam_stim2;

        public GameObject memoryExam;
        //public GameObject memoryExam_stim1;
        //public GameObject memoryExam_stim2;

        public GameObject flankerExam;
        //public GameObject flankerExam_stim1;
        //public GameObject flankerExam_stim2;

        public GameObject result;
        public GameObject result_win;
        public GameObject result_lose;

        public TextMeshProUGUI resultInfo;

        public Sprite btPressed;
        public Sprite btNormal;

        public AnimationCurve crossAnimCurve;
        public AnimationCurve resultAnimCurve;

        public CarPlayer carPlayer;
        public FlankerPlayer  flankerPlayer;
        public MemoryPlayer memoryPlayer;
        public StroopPlayer stroopPlayer;
        public SwitcherPlayer switcherPlayer;
        public GiraffePlayer giraffePlayer;

        IExamPlayer curExamPlayer => GetExamPlayer();

        int maxRound
        {
            get {
                if (examRunType == ExamRunType.Practice)
                    return  SystemManager.Instance.dic_examOptions[examType].practiceRound;
                else
                    return SystemManager.Instance.dic_examOptions[examType].examRound;
            }
        }

        //当前的时间限制
        float inputTimeLimit
        {
            get {

                if (examRunType == ExamRunType.Practice)
                    return SystemManager.Instance.dic_examOptions[examType].practiceTimeLimit * 0.5f;
                else
                    return SystemManager.Instance.dic_examOptions[examType].examTimeLimit * 0.5f;
            }
        }

        //正确答案是哪个
        ExamAnswer correct => curExamPlayer.correctAnswer;

        #endregion

        //输入的时间记录
        float inputTime = 0f;
        
        //开始可输入的标记
        bool isStartToWaitTrigger;

        //是否超时的标记
        bool isTimeOut;

        //按钮是否被按下了
        bool isActionTrigged;

        //按钮按下信息
        ExamInput answerInput;
                

        //当前的轮数
        int curRound;

        
        ExamInfo curExamInfo;

        string practiceStartTip => "测试者需要进行<color=#FFAB04>熟练度</color>练习，练习若干轮次后，正确率达到<color=#FFAB04>"+ SystemManager.Instance.dic_examOptions[examType].passScore +"%</color>以上，方可进行正式测试";

        string examStartTip => "下面将进行正式测试，测试难度会有所提高";


        UserInfo userInfo => SystemManager.Instance.curUserInfo;
        ExamType examType => SystemManager.Instance.curExamType;

        public ExamRunType examRunType;

        private void OnEnable()
        {
            backBt.onClick.AddListener(() => {

                if (examRunType == ExamRunType.Practice)
                {
                    backBt.onClick.RemoveAllListeners();
                    SystemManager.Instance.GameBackToMain();
                }
            });
        }

        private void OnDisable()
        {
            backBt.onClick.RemoveAllListeners();
        }

        // Start is called before the first frame update
        void Start()
        {

            examRunType = SystemManager.Instance.curExamRunType;

            carPlayer.Init(this, leftBt, leftBtText, rightBt, rightBtText);
            flankerPlayer.Init(this, leftBt, leftBtText, rightBt, rightBtText);
            memoryPlayer.Init(this, leftBt, leftBtText, rightBt, rightBtText);
            stroopPlayer.Init(this, leftBt, leftBtText, rightBt, rightBtText);
            switcherPlayer.Init(this, leftBt, leftBtText, rightBt, rightBtText);
            giraffePlayer.Init(this, leftBt, leftBtText, rightBt, rightBtText);

            

            carDescBt.onClick.AddListener(TipPractice);
            stroopDescBt.onClick.AddListener(TipPractice);
            giraffeDescBt.onClick.AddListener(TipPractice);  
            switcherDescBt.onClick.AddListener(TipPractice);
            memoryDescBt.onClick.AddListener(TipPractice);
            flankerDescBt.onClick.AddListener(TipPractice);

            RestGame();

            if (examRunType == ExamRunType.Practice)
            {
                RunPractice();
            }
            else 
            {
                RunExam();
            }
        }

        void RunPractice()
        {
            SetTitle("测试规则说明");

            SystemManager.Instance.DelayAction(0.5f, 0, () => { SystemManager.Instance.PlaySwipe(); });

            DesTrans.transform.DOScaleX(1f, 0.5f).From(0).SetDelay(0.2f).OnComplete(() => {

                if (examType != ExamType.None)
                {
                    ShowDesc(true, examType);
                    //ShowCenterBt(true, "开始", TipPractice);
                }

            });
        }

        void RunExam()
        {

            SetTitle("立即进行测试");

            //SystemManager.Instance.DelayAction(0.5f, 0, () => { SystemManager.Instance.PlaySwipe(); });

            ShowGameInfo(true, examStartTip, () => {

                ShowCenterBt(true, "确定", () => {

                    StartExam();


                });
            });


        }

        

        void RestGame()
        {
            DesTrans.SetActive(true);
            DesTrans.transform.localScale = new Vector3(0, 1, 1);

            PlayTrans.SetActive(true);

            curRound = 0;

            ShowFixationImage(false);
            ShowGameInfo(false);
            ShowRoundNum(false);
            ShowCenterBt(false);

            ResetRound();
            
        }

        void ResetRound()
        {
            PlayTrans.transform.localScale = new Vector3(0, 1, 1);

            inputTime = 0f;
            isStartToWaitTrigger = false;
            isTimeOut = false;
            isActionTrigged = false;

            answerInput = ExamInput.None;

            ShowTimeBar(false);
            ShowInputBt(false);
            SetBg(true);
            ShowDesc(false);
            ShowStim(false);
            ShowResult(false);
        }


        void SetTitle(string con)
        {
            gameTitle.text = con;
        }

        void ShowDesc(bool isShow, ExamType examType = ExamType.None)
        {
            if (!isShow || examType==ExamType.None)
            {
                
                carDesc.SetActive(false);
                stroopDesc.SetActive(false);
                giraffeDesc.SetActive(false);
                switcherDesc.SetActive(false);
                memoryDesc.SetActive(false);
                flankerDesc.SetActive(false);

                //centerBt.transform.parent = bottomRoot;
                //centerBt.GetComponent<RectTransform>().anchoredPosition = new Vector3(621, -135, 0);

            }
            else {


                switch (examType)
                {
                    case ExamType.Car: carDesc.SetActive(true); //centerBt.transform.parent = carDescBtRoot; 
                        break;
                    case ExamType.Stroop: stroopDesc.SetActive(true); //centerBt.transform.parent = stroopDescBtRoot; 
                        break;
                    case ExamType.Giraffe: giraffeDesc.SetActive(true); //centerBt.transform.parent = giraffeDescBtRoot; 
                        break;
                    case ExamType.Switcher: switcherDesc.SetActive(true); //centerBt.transform.parent = switcherDescBtRoot; 
                        break;
                    case ExamType.Memory: memoryDesc.SetActive(true); //centerBt.transform.parent = memoryDescBtRoot; 
                        break;
                    case ExamType.FlankerFish: flankerDesc.SetActive(true); //centerBt.transform.parent = flankerDescBtRoot; 
                        break;
                }

            }
        }

        void ShowResult(bool isShow,bool isWin = true)
        {
            if (!isShow)
            {
                result.SetActive(false);
                result_win.SetActive(false);
                result_lose.SetActive(false);
                resultInfo.gameObject.SetActive(false);
            }
            else 
            {
                result.SetActive(true);
                result_win.SetActive(isWin);
                result_lose.SetActive(!isWin);

                if (isWin)
                    result_win.transform.DOScale(new Vector3(2, 2, 2), 0.5f).From(Vector3.one).SetEase(resultAnimCurve);
                else
                    result_lose.transform.DOScale(new Vector3(2, 2, 2), 0.5f).From(Vector3.one).SetEase(resultAnimCurve);


                SystemManager.Instance.PlayResult(isWin);

                if (examRunType == ExamRunType.Practice)
                {
                    resultInfo.gameObject.SetActive(true);

                    if (isTimeOut)
                    {
                        resultInfo.text = "超时! " + " 正确率:" + GetScorePercent().ToString("P2");
                    }
                    else
                    {
                        resultInfo.text ="延迟: "+ inputTime.ToString() + " 正确率:" + GetScorePercent().ToString("P2");
                    }
                }
                else 
                {
                    resultInfo.gameObject.SetActive(false);
                }
                

                
            }
        }

        float GetScorePercent()
        {
            if (curExamInfo == null)
                return 0;

            float score = 0f;
            foreach (var g in curExamInfo.lst_examRecords)
            {
                if ((int)g.examInput == (int)g.examAnswer && !g.isTimeOut)
                {
                    score += 1f;
                }
            }

            return score/(curRound+1f);
        }

        int GetScoreInt()
        {
            if (curExamInfo == null)
                return 0;

            int score = 0;
            foreach (var g in curExamInfo.lst_examRecords)
            {
                if ((int)g.examInput == (int)g.examAnswer && !g.isTimeOut)
                {
                    score += 1;
                }
            }

            return score;
        }

        void ShowCenterBt(bool isShow, string title = null, UnityAction unityAction = null)
        {
            if (isShow)
            {
                centerBtText.text = title;
                centerBt.transform.parent.gameObject.SetActive(true);
                
                if(unityAction!=null)
                    centerBt.onClick.AddListener(unityAction);
            }
            else 
            {
                centerBtText.text = null;
                centerBt.onClick.RemoveAllListeners();
                centerBt.transform.parent.gameObject.SetActive(false);
            }
        }

        void ShowGameInfo(bool isShow, string con = null, UnityAction unityAction = null)
        {
            if (isShow)
            {
                gameInfoRoot.SetActive(true);
                gameInfo.text = null;
                gameInfo.gameObject.SetActive(true);

                gameInfo.DOText(con, con.Length / 40f).SetDelay(0.2f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    unityAction?.Invoke();
                });
            }
            else {
                gameInfoRoot.SetActive(false);
                gameInfo.text = null;
                gameInfo.gameObject.SetActive(false);
            }
        }

        void TipPractice()
        {
            SetTitle("测试前的练习");

            ShowDesc(false);
            DesTrans.SetActive(false);

            ShowCenterBt( false);


            ShowGameInfo(true, practiceStartTip, () => { ShowCenterBt(true, "确定", ()=> {

                StartPractice(); 


            }); });

        }

        void ShowRoundNum(bool isShow)
        {
            roundRoot.SetActive(isShow);
            
            if(isShow)
                roundNum.text = GetScoreInt() + "/" + maxRound;
        }


        IExamPlayer GetExamPlayer()
        {
            switch (examType)
            {
                case ExamType.Car:
                    return carPlayer;
                case ExamType.FlankerFish:
                    return flankerPlayer;
                case ExamType.Switcher:
                    return switcherPlayer;
                case ExamType.Stroop:
                    return stroopPlayer;
                case ExamType.Giraffe:
                    return giraffePlayer;
                case ExamType.Memory:
                    return memoryPlayer;
            }

            return null;
        }

        void StartPractice()
        {
            backBt.gameObject.SetActive(false);

            RestGame();

            curRound = 0;

            CreateExamInfo();

            DoRound();
        }

        void StartExam()
        {
            backBt.gameObject.SetActive(false);

            examRunType = ExamRunType.Exam;

            RestGame();

            curRound = 0;


            CreateExamInfo();

            DoRound();
        }

        void DoRound()
        {
            if (examRunType == ExamRunType.Practice)
                SetTitle("练习第<color=#66FFFF>" + (curRound+1) + "/" + maxRound+ "</color>轮");
            else
                SetTitle("测试第<color=#66FFFF>" + (curRound + 1) + "/" + maxRound+ "</color>轮");

            ShowRoundNum(true);

            ShowFixationImage(true, () => {

                if (curExamPlayer.isSkipStim1)
                {
                    Open(() => {

                        SystemManager.Instance.DelayAction(0.125f, 0f, () => {

                            SetTestRound_B();

                        });

                    });
                }
                else 
                {

                    Open(() => {

                        SetTestRound_A();

                        SystemManager.Instance.DelayAction(1f, 0f, () => {

                            CloseAndOpen(() => {

                                SystemManager.Instance.DelayAction(0.125f, 0f, () => {

                                    SystemManager.Instance.PlaySwipe();

                                    SetTestRound_B();

                                });

                            });

                        });

                    });
                }


                

            });
            
        }

        void SetBg(bool isShowFront)
        {
            bgFront.SetActive(isShowFront);
            bgBack.SetActive(!isShowFront);

            if (examRunType == ExamRunType.Practice)
            {
                bgBack.GetComponent<Image>().color = new Color32(87, 150, 190, 255);
            }
            else 
            {
                bgBack.GetComponent<Image>().color = new Color32(137, 87, 190, 255);
            }
        }

        void Close(Action callback)
        {
            SystemManager.Instance.PlaySwipe();
            PlayTrans.transform.DOScaleX(0, 0.25f).SetEase(Ease.Linear).OnComplete(() => {

                callback?.Invoke();
            });
        }

        void Open(Action callback)
        {;
            PlayTrans.transform.DOScaleX(1, 0.25f).SetDelay(0.2f).SetEase(Ease.Linear).OnComplete(() => {
                
                SystemManager.Instance.PlaySwipe();
                callback?.Invoke();
            });
        }

        void CloseAndOpen(Action callback)
        {
            if(!curExamPlayer.isDelayCloseStim1)
                ShowStim(false);

            PlayTrans.transform.DOScaleX(0, 0.25f).SetEase(Ease.Linear).OnComplete(() => {

                if (curExamPlayer.isDelayCloseStim1)
                    ShowStim(false);

                SetBg(false);
                                
                PlayTrans.transform.DOScaleX(1, 0.25f).SetEase(Ease.Linear).OnComplete(() => {

                    SystemManager.Instance.PlaySwipe();

                    PlayTrans.transform.DOScaleX(0, 0.25f).SetDelay(0.3f).SetEase(Ease.Linear).OnComplete(() => {

                        SetBg(true);
                        ShowStim(true, examType);

                        PlayTrans.transform.DOScaleX(1, 0.125f).SetEase(Ease.Linear).OnComplete(() => {
                                                        
                            callback?.Invoke();

                        });
                                                
                    });

                });

            });
        }

        public event UnityAction OnSetTestRound_A;
        public event UnityAction OnSetTestRound_B;
        public event UnityAction OnSetTestRound_B_Button;

        void SetTestRound_A()
        {
            //Debug.Log("SetTestRound_A");
            ShowStmi1();
            OnSetTestRound_A?.Invoke();
        }

        void ShowTimeBar(bool isShow)
        {
            timeBar.SetActive(isShow);
            timeBarValue.fillAmount = 1f;
        }

        public void ShowInputBt(bool isShow, UnityAction bt1callback = null, UnityAction bt2callback = null)
        {
            if (!isShow)
            {
                leftBt.onClick.RemoveAllListeners();
                leftBt.transform.parent.gameObject.SetActive(false);
                leftBtText.text = null;
                leftBt.GetComponentInChildren<Image>().sprite = btNormal;
                leftBt.GetComponentInChildren<Image>().color = Color.white;

                rightBt.onClick.RemoveAllListeners();
                rightBt.transform.parent.gameObject.SetActive(false);
                rightBtText.text = null;
                rightBt.GetComponentInChildren<Image>().sprite = btNormal;
                rightBt.GetComponentInChildren<Image>().color = Color.white;
            }
            else 
            {
                leftBt.onClick.AddListener(bt1callback);
                leftBt.transform.parent.gameObject.SetActive(true);
                                
                rightBt.onClick.AddListener(bt2callback);
                rightBt.transform.parent.gameObject.SetActive(true);

            }
        }

        
        void InputAction1()
        {
            if (isActionTrigged || !isStartToWaitTrigger)
                return;


            isActionTrigged = true;
            answerInput =  ExamInput.Congruent;

            leftBt.GetComponentInChildren<Image>().sprite = btPressed;
            leftBt.GetComponentInChildren<Image>().color = new Color(0.4f, 0.4f, 0.4f, 0.8f);
            leftBt.onClick.RemoveAllListeners();

            //Debug.Log("InputAction1 " + inputTime + " answerInput " + answerInput);

            AddRoundExamRecord();

            SystemManager.Instance.DelayAction(0.5f, 0f, () => {

                PlayerReslut();

            });
        }

        void InputAction2()
        {
            if (isActionTrigged || !isStartToWaitTrigger)
                return;

            isActionTrigged = true;

            
            answerInput = ExamInput.Incongruent;

            //Debug.Log("InputAction2 " + inputTime + " answerInput " + answerInput);

            rightBt.GetComponentInChildren<Image>().sprite = btPressed;
            rightBt.GetComponentInChildren<Image>().color = new Color(0.4f, 0.4f, 0.4f, 0.8f);
            rightBt.onClick.RemoveAllListeners();

            AddRoundExamRecord();

            SystemManager.Instance.DelayAction(0.5f, 0f, () => {

                PlayerReslut();

            });

            

        }

        void PlayerReslut()
        {
            if (isTimeOut)
            {
                ShowResult(true, false);
            }
            else 
            {
                if ((int)answerInput == (int)correct)
                { 
                    ShowResult(true, true);
                }
                else
                { 
                    ShowResult(true, false);
                }
            }

            ShowRoundNum(true);

            curRound += 1;
            float delay = 2f;
            if (examRunType == ExamRunType.Exam)
            {
                delay = 1f;
            }

            SystemManager.Instance.DelayAction(delay, 0f,() => {

                CheckFinish();

            });
            
            

        }

        void CheckFinish()
        {
            if (curRound < maxRound)
            {
                ResetRound();
                DoRound(); 
            }
            else
            {
                backBt.gameObject.SetActive(true);

                ResetRound();
                if (examRunType == ExamRunType.Practice)
                {
                    float fScore = (float)GetScoreInt() / maxRound;
                    //Debug.Log("fScore " + fScore);

                    if (fScore < SystemManager.Instance.dic_examOptions[examType].passScore / 100f)
                        ShowGameInfo(true, "您的正确率是: <color=#FFAB04>" + fScore.ToString("P2") + "</color>\n您当前的成绩不符合，还需要再进行一次练习", () =>
                        {
                            ShowCenterBt(true, "确定", () =>
                            {

                                StartPractice();

                            });
                        });
                    else
                        ShowGameInfo(true, "您的正确率是: <color=#FFAB04>" + fScore.ToString("P2") + "</color>\n您当前的成绩已经符合，开始正式测试", () =>
                        {
                            ShowCenterBt(true, "确定", () =>
                            {

                                StartExam();

                            });
                        });
                }
                else 
                {
                    float fScore = (float)GetScoreInt() / maxRound;
                    RecordSave();
                    ShowGameInfo(true, "您的正确率是: <color=#FFAB04>" + fScore.ToString("P2") + "</color>\n您当前的成绩已经记录，谢谢您的参与", () => { ShowCenterBt( true, "确定", SystemManager.Instance.GameBackToMain); });
                }
            }
        }
               

        void SetTestRound_B()
        {
            OnSetTestRound_B?.Invoke();
            OnSetTestRound_B_Button?.Invoke();

            ShowInputBt(true, InputAction1, InputAction2);

            if (curExamPlayer.isKeepStim1)
                ShowStmi1and2();
            else
                ShowStmi2();

            ShowTimeBar(true);

            isActionTrigged = false;
            isTimeOut = false;

            isStartToWaitTrigger = true;

        }


        void ShowStmi1()
        {
            ShowStim(true, examType, true, false);
            
        }
       
        void ShowStmi2()
        {
            ShowStim(true, examType, false, true);
        }

        void ShowStmi1and2()
        {
            ShowStim(true, examType, true, true);

        }
        void ShowStim(bool isShow, ExamType examType = ExamType.None, bool isStim1 = false, bool isStim2 = false)
        {
            if (!isShow)
            {
                carExam.SetActive(false);
                stroopExam.SetActive(false);
                giraffeExam.SetActive(false);
                switcherExam.SetActive(false);
                memoryExam.SetActive(false);
                flankerExam.SetActive(false);

                curExamPlayer.ShowStim();
            }
            else 
            {
                switch (examType)
                {
                    case ExamType.Car: 
                        carExam.SetActive(true);
                        break;
                    case ExamType.Stroop:
                        stroopExam.SetActive(true);
                        break;
                    case ExamType.Giraffe:
                        giraffeExam.SetActive(true);
                        break;
                    case ExamType.Switcher:
                        switcherExam.SetActive(true);
                        break;
                    case ExamType.Memory:
                        memoryExam.SetActive(true);
                        break;
                    case ExamType.FlankerFish:
                        flankerExam.SetActive(true);
                        break;
                }

                curExamPlayer.ShowStim(isStim1, isStim2);
            }
        }

        void ShowFixationImage(bool isShow, Action callback = null)
        {
            if (!isShow)
            {
                FixationImage.SetActive(false);
            }
            else 
            {
                FixationImage.SetActive(true);
                FixationImage.GetComponent<Image>().DOFade(1, 0.5f).From(0).SetEase(crossAnimCurve).SetLoops(3);

                SystemManager.Instance.PlayClockTik();
                FixationImage.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).From(Vector3.one).SetEase(crossAnimCurve).OnComplete(() => {

                    SystemManager.Instance.PlayClockTik();
                    FixationImage.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).From(Vector3.one).SetEase(crossAnimCurve).OnComplete(() => {

                        SystemManager.Instance.PlayClockTik();
                        FixationImage.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).From(Vector3.one).SetEase(crossAnimCurve).OnComplete(() => {

                            //Debug.Log("finish cross anim");
                            FixationImage.SetActive(false);
                            callback?.Invoke();

                        });
                    });
                });
            }
            
        }

        
        void UpdateInputTime()
        {
            inputTime += Time.deltaTime;

            float amount = Mathf.Max(1 - inputTime / inputTimeLimit, 0);
            timeBarValue.fillAmount = amount;
            timeBarValue.color = new Color(1f , amount, amount, 1);
            if (inputTime >= inputTimeLimit)
            {
                //Debug.Log("Time out");
                isStartToWaitTrigger = false;
                isTimeOut = true;

                AddRoundExamRecord();

                SystemManager.Instance.DelayAction(0.5f, 0f, () => {

                    PlayerReslut();

                });
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isStartToWaitTrigger && !isActionTrigged)
            {
                UpdateInputTime();
            }
        }
                

        void CreateExamInfo()
        {
            curExamInfo = new ExamInfo();
            curExamInfo.id = userInfo.id;
            curExamInfo.dateTime = DateTime.Now;
            curExamInfo.examType = examType;
        }

        void AddRoundExamRecord()
        {
            //Debug.Log("AddRoundExamRecord");

            if (curExamInfo == null)
                return;

            ExamRecord item = new ExamRecord();
            item.sessionType = "0-1";
            item.examAnswer = correct;
            item.examInput = answerInput;

            //Debug.Log(item.examAnswer + " " + item.examInput);

            item.blockId = curRound + 1;
            item.inputDelay = inputTime;
            item.isTimeOut = isTimeOut;

            curExamInfo.lst_examRecords.Add(item);
        }

        void RecordSave()
        {
            SystemManager.Instance.AddExamInfo(curExamInfo);

        }
    }
}