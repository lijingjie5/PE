using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

namespace BLG
{
    public class UserInfoCtrl : MonoBehaviour
    {
        public NormalBt backBt;
        UserInfo userInfo;
        ExamType examType => SystemManager.Instance.curExamType;
        IReadOnlyDictionary<ExamType, ExamInfo> dic_user_examInfos => SystemManager.Instance.dic_user_examInfos;

        public TMP_InputField inputName;
        public TMP_Dropdown inputGender;
        public TMP_InputField inputAge;
        public TextMeshProUGUI createDate;

        public NormalBt carBt;
        public NormalBt flangkerFishBt;
        public NormalBt memoryBt;
        public NormalBt switcherBt;
        public NormalBt giraffeBt;
        public NormalBt stroopBt;

        public TextMeshProUGUI carScore;
        public TextMeshProUGUI flangkerScore;
        public TextMeshProUGUI memoryScore;
        public TextMeshProUGUI switcherScore;
        public TextMeshProUGUI giraffeScore;
        public TextMeshProUGUI stroopScore;

        public NormalBt copyBt;

        private void OnEnable()
        {
            backBt.onClick.AddListener(() => {

                backBt.onClick.RemoveAllListeners();
                SystemManager.Instance.ShowMainScene();

            });
        }

        private void OnDisable()
        {
            backBt.onClick.RemoveAllListeners();
        }


        // Start is called before the first frame update
        void Start()
        {
                        
            SetBts();

        }

        public void ShowUserDataInfo()
        {
            userInfo = SystemManager.Instance.curUserInfo;
            
            SetInputName();
            SetInputGender();
            SetInputAge();
            SetCreateDate();
            CheckExamState();
        }


        void SetBts()
        {
            carBt.onClick.AddListener(() => { SystemManager.Instance.ShowExamInfo(ExamType.Car); });
            flangkerFishBt.onClick.AddListener(() => { SystemManager.Instance.ShowExamInfo(ExamType.FlankerFish); });
            memoryBt.onClick.AddListener(() => { SystemManager.Instance.ShowExamInfo(ExamType.Memory); });
            switcherBt.onClick.AddListener(() => { SystemManager.Instance.ShowExamInfo(ExamType.Switcher); });
            giraffeBt.onClick.AddListener(() => { SystemManager.Instance.ShowExamInfo(ExamType.Giraffe); });
            stroopBt.onClick.AddListener(() => { SystemManager.Instance.ShowExamInfo(ExamType.Stroop); });

            copyBt.onClick.AddListener(() => {
                Debug.Log("SetClipboardData");

                WX.SetClipboardData(new SetClipboardDataOption()
                {
                    data =SystemManager.Instance.GetUserExamString(),
                    success = (res) => {
                        Debug.Log("success " + res);
                    },
                    fail = (res) => { Debug.Log("failed " + res.errMsg); },

                });

            });
        }

        void CheckExamState()
        {
            carBt.GetComponentInChildren<Image>().color = dic_user_examInfos.ContainsKey(ExamType.Car) ? Color.green : Color.white;
            flangkerFishBt.GetComponentInChildren<Image>().color = dic_user_examInfos.ContainsKey(ExamType.FlankerFish) ? Color.green : Color.white;
            memoryBt.GetComponentInChildren<Image>().color = dic_user_examInfos.ContainsKey(ExamType.Memory) ? Color.green : Color.white;
            switcherBt.GetComponentInChildren<Image>().color = dic_user_examInfos.ContainsKey(ExamType.Switcher) ? Color.green : Color.white;
            giraffeBt.GetComponentInChildren<Image>().color = dic_user_examInfos.ContainsKey(ExamType.Giraffe) ? Color.green : Color.white;
            stroopBt.GetComponentInChildren<Image>().color = dic_user_examInfos.ContainsKey(ExamType.Stroop) ? Color.green : Color.white;

            carScore.text = "0 %";
            flangkerScore.text = "0 %";
            memoryScore.text = "0 %";
            switcherScore.text = "0 %";
            giraffeScore.text = "0 %";
            stroopScore.text = "0 %";

            foreach (var g in SystemManager.Instance.dic_user_examInfos)
            {
                float score = 0f;

                foreach (var s in g.Value.lst_examRecords)
                {                    
                    if (!s.isTimeOut && s.examInput != ExamInput.None)
                    {
                        if ((int)s.examInput == (int)s.examAnswer)
                        {
                            score += 1f;
                        }
                    }
                }
                var res = (score / g.Value.lst_examRecords.Count).ToString("P2");

                switch (g.Key)
                {
                    case ExamType.Car: carScore.text = res;  break;
                    case ExamType.FlankerFish: flangkerScore.text = res; break;
                    case ExamType.Memory: memoryScore.text = res; break;
                    case ExamType.Switcher: switcherScore.text = res; break;
                    case ExamType.Giraffe: giraffeScore.text = res; break;
                    case ExamType.Stroop: stroopScore.text = res; break;

                }

                
            }

        }

        void SetInputName()
        {
            inputName.text = userInfo.name;
            inputName.image.color = SystemManager.Instance.lst_userColors[userInfo.colorId];

            inputName.onSubmit.AddListener(UpdateUserName);
                        
        }

        void UpdateUserName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                inputName.text = userInfo.name;
                return;
            }

            if (userInfo.name == name)
            {
                return;
            }

            userInfo.name = name;
            SystemManager.Instance.UpdateUserInfo(userInfo);

            inputName.text = userInfo.name;

            Debug.Log("onSubmit " + name);
        }

        void SetInputGender()
        {
            
            inputGender.value = (int)userInfo.gender;
            inputGender.options[0].text = Gender.Boy.ToString();
            inputGender.options[1].text = Gender.Girl.ToString();

            inputGender.onValueChanged.AddListener((res) => {

                userInfo.gender = (Gender)res;
                SystemManager.Instance.UpdateUserInfo(userInfo);
                Debug.Log("onValueChanged " + (Gender)res);
            });
        }

        void SetInputAge()
        {
            inputAge.text = userInfo.age.ToString();

            inputAge.onSubmit.AddListener(UpdateUserAge);

        }

        void UpdateUserAge(string age)
        {
            if (string.IsNullOrEmpty(age))
            {
                inputAge.text = userInfo.age;
                return;
            }
            
            if (!int.TryParse(age, out int num))
            {
                inputAge.text = userInfo.age;
                return;
            }

            if (userInfo.age == age)
            {
                return;
            }

            userInfo.age = age;
            SystemManager.Instance.UpdateUserInfo(userInfo);
            inputAge.text = userInfo.age;
            Debug.Log("onSubmit " + age);
        }

        void SetCreateDate()
        {
            createDate.text = userInfo.userCreateDate.ToString();
        }

        // Update is called once per frame
        void Update()
        {

        }
               
    }
}
