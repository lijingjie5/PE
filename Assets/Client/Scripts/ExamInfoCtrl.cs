using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

namespace BLG
{
    public class ExamInfoCtrl : MonoBehaviour
    {
        public TextMeshProUGUI title;
        public NormalBt backBt;

        public GameObject recordList;
        public TextMeshProUGUI recordDate;
        public TextMeshProUGUI recordAcc;
        public GameObject recordPrefab;
        public RectTransform recordContent;

        public NormalBt Func1Bt;
        public NormalBt Func2Bt;
                               

        UserInfo userInfo => SystemManager.Instance.curUserInfo;
        ExamType examType => SystemManager.Instance.curExamType;
        IReadOnlyDictionary<ExamType, ExamInfo> dic_user_examInfos => SystemManager.Instance.dic_user_examInfos;

        List<GameObject> lst_recordGos = new List<GameObject>();

        
        // Start is called before the first frame update
        void Start()
        {
            //SystemManager.Instance.examInfoCtrl = this;
            SetExamBts();
        }

        public void ShowExamInfo()
        {
            SetTitle();
            UpdateRecordList();
        }

        void SetTitle()
        {
            title.text = examType.ToString() + " 测试";
        }

        private void OnEnable()
        {
            backBt.onClick.AddListener(() => {

                backBt.onClick.RemoveAllListeners();
                SystemManager.Instance.OpenUser(userInfo);

            });
        }

        private void OnDisable()
        {
            backBt.onClick.RemoveAllListeners();
        }


        void SetExamBts()
        {
            
            Func1Bt.onClick.AddListener(() => {

                RunPractice();
                
            });

            Func2Bt.onClick.AddListener(() => {

                RunExperiment();
            });

                        
        }

        void ClearRecordList()
        {
            for (int i = 0; i < lst_recordGos.Count; i++)
            {
                Destroy(lst_recordGos[i]);
            }

            lst_recordGos.Clear();

            recordList.SetActive(false);

            recordDate.enabled = true;
            recordDate.text = userInfo.name + " 当前没有数据" ;

            recordAcc.enabled = false;
        }

        void UpdateRecordList()
        {
            ClearRecordList();

            if (!dic_user_examInfos.ContainsKey(examType))
                return;

            recordList.SetActive(true);
            
            var record = dic_user_examInfos[examType];
            //Debug.Log(record.lst_examRecords.Count);
            recordDate.text = userInfo.name + " "+  record.dateTime.ToString();
            recordDate.enabled = true;

            float score = 0f;

            foreach (var g in record.lst_examRecords)
            {
                GameObject newBt = Instantiate(recordPrefab, recordContent);
                newBt.SetActive(true);
                lst_recordGos.Add(newBt);

                var label = newBt.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = UpdateExamRecord(g);


                if (!g.isTimeOut && g.examInput != ExamInput.None)
                {
                    if ((int)g.examInput == (int)g.examAnswer)
                    {
                        score += 1f;
                    }
                }
            }

            recordAcc.text = "ACC: " + (score / record.lst_examRecords.Count).ToString("P2");
            recordAcc.enabled = true;

        }

        StringBuilder rd = new StringBuilder();
        string UpdateExamRecord(ExamRecord record)
        {
            rd.Clear();
            rd.Append(record.blockId +" | ");
            //rd.Append(record.sessionType + " | ");
            rd.Append(record.examAnswer + " | ");
            rd.Append(record.examInput + " | ");
            rd.Append(record.inputDelay + " | ");
            rd.Append(record.isTimeOut);


            return rd.ToString();
        }


        void ClearRecordAndSave()
        {

            if (Application.isEditor)
            {
                SystemManager.Instance.RemoveExamInfo(examType);
            }
            else
            {
                WX.ShowModal(new ShowModalOption()
                {
                    title = "删除该用户的数据",
                    content = "数据将永久删除",
                    cancelText = "取消",
                    confirmText = "确定",
                    success = (res) => {
                        if (res.confirm)
                        {
                            SystemManager.Instance.RemoveExamInfo(examType);

                        }
                    }
                });
            }
        }

        void RunPractice()
        {
            SystemManager.Instance.LoadExamScene(examType, ExamRunType.Practice);
        }

        void RunExperiment()
        {

            SystemManager.Instance.LoadExamScene(examType, ExamRunType.Exam);
            return;

            if (Application.isEditor)
            {
                GetRecordAndSave();
                
            }
            else
            {
                WX.ShowToast(new ShowToastOption()
                {
                    title = "实验完成产生数据",
                    duration = 2000,
                    mask = true,
                    success = (res) => {

                        GetRecordAndSave();
                        
                    }
                });
            }
        }

        void GetRecordAndSave()
        {
            var dd = new ExamInfo();
            dd.id = userInfo.id;
            dd.dateTime = DateTime.Now;
            dd.examType = examType;
           

            for (int i = 0; i < 36; i++)
            {
                ExamRecord aa = new ExamRecord();
                aa.sessionType = "0-1";
                aa.examAnswer = (ExamAnswer)UnityEngine.Random.Range(1, 3);
                aa.examInput = (ExamInput)UnityEngine.Random.Range(1, 3);
                aa.blockId = i+1;
                aa.inputDelay = UnityEngine.Random.Range(1f, 100f);
                aa.isTimeOut = UnityEngine.Random.Range(0, 100) > 80 ? true : false;

                dd.lst_examRecords.Add(aa);
            }

            SystemManager.Instance.AddExamInfo(dd);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
