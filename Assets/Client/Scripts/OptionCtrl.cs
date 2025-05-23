using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

namespace BLG
{
    public class OptionCtrl : MonoBehaviour
    {
        public NormalBt backBt;
        public NormalBt saveBt;

        public GameObject sndRoot;
        public GameObject carRoot;
        public GameObject flankerRoot;
        public GameObject memoryRoot;
        public GameObject switcherRoot;
        public GameObject giraffeRoot;
        public GameObject stroopRoot;

        Dictionary<ExamType, List<Slider>> lst_examOptions_sliders = new Dictionary<ExamType, List<Slider>>();
        Dictionary<ExamType, List<TextMeshProUGUI>> lst_examOptions_labels = new Dictionary<ExamType, List<TextMeshProUGUI>>();
        
        Slider sndSlider;
        TextMeshProUGUI sndLabel;

        int GetSliderValue(ExamType examType, int index)
        {
            var option = SystemManager.Instance.dic_examOptions[examType];
            switch (index)
            {
                case 0: return option.practiceTimeLimit;
                case 1: return option.practiceRound;
                case 2: return option.passScore;
                case 3: return option.examTimeLimit;
                case 4: return option.examRound;
            }

            return 0;
        }

        void UpdateSliderValue(ExamType examType, int index, int value)
        {
            //Debug.Log(examType + " " + index + " " + value);

            var option = SystemManager.Instance.curOption;
            //Debug.Log(option.lst_examOptions.Count);

            ExamOption opt = null;
            switch (examType)
            {
                case ExamType.None:
                    option.soundVolume = value;
                    sndLabel.text = value.ToString();
                    break;
                case ExamType.Car:
                    opt = option.lst_examOptions[0];
                    break;
                case ExamType.FlankerFish:
                    opt = option.lst_examOptions[1];
                    break;
                case ExamType.Memory:
                    opt = option.lst_examOptions[2];
                    break;
                case ExamType.Switcher:
                    opt = option.lst_examOptions[3];
                    break;
                case ExamType.Giraffe:
                    opt = option.lst_examOptions[4];
                    break;
                case ExamType.Stroop:
                    opt = option.lst_examOptions[5];
                    break;

            }

            //Debug.Log(opt != null);

            if (opt != null)
            {
                switch (index)
                {
                    case 0:
                        opt.practiceTimeLimit = value;
                        lst_examOptions_labels[examType][index].text = (value * 0.5f) + "s";

                        //Debug.Log(opt.practiceTimeLimit + " " + value);

                        break;
                    case 1:
                        opt.practiceRound = value;
                        lst_examOptions_labels[examType][index].text = value.ToString();
                        break;
                    case 2:
                        opt.passScore = value;
                        lst_examOptions_labels[examType][index].text = value +"%";
                        break;
                    case 3:
                        opt.examTimeLimit = value;
                        lst_examOptions_labels[examType][index].text = (value * 0.5f) + "s";
                        break;
                    case 4:
                        opt.examRound = value;
                        lst_examOptions_labels[examType][index].text = value.ToString();
                        break;
                }
            }

        }

        void SetSliders()
        {
            sndSlider = sndRoot.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Slider>();
            sndLabel = sndRoot.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            AddListByRoot(ExamType.Car, carRoot);
            AddListByRoot(ExamType.FlankerFish, flankerRoot);
            AddListByRoot(ExamType.Memory, memoryRoot);
            AddListByRoot(ExamType.Switcher, switcherRoot);
            AddListByRoot(ExamType.Giraffe, giraffeRoot);
            AddListByRoot(ExamType.Stroop, stroopRoot);

            ColorUtility.TryParseHtmlString("#C8FF00", out Color cd);

            sndSlider.fillRect.GetComponent<Image>().color = cd;
            sndSlider.wholeNumbers = true;
            sndSlider.minValue = 0;
            sndSlider.maxValue = 100;
            sndSlider.value = SystemManager.Instance.soundVolume;

            sndSlider.onValueChanged.AddListener((num) => {

                UpdateSliderValue(ExamType.None, 0, (int)num);
            });

            sndLabel.text = SystemManager.Instance.soundVolume.ToString();


            foreach (var g in lst_examOptions_sliders)
            {
                for (int i = 0; i < g.Value.Count; i++)
                {
                    int sliderValue = GetSliderValue(g.Key, i);
                    g.Value[i].fillRect.GetComponent<Image>().color = cd;
                    g.Value[i].wholeNumbers = true;
                    g.Value[i].minValue = 0;

                    var sliderIndex = g.Value[i].gameObject.AddComponent<SliderIndex>();
                    sliderIndex.examType = g.Key;
                    sliderIndex.index = i;

                    g.Value[i].onValueChanged.AddListener((num) => {

                        //Debug.Log(num);
                        UpdateSliderValue(sliderIndex.examType, sliderIndex.index, (int)num);
                    });

                    switch (i)
                    {
                        case 0:
                        case 3:
                            g.Value[i].maxValue = 6;
                            g.Value[i].value = sliderValue;
                            lst_examOptions_labels[g.Key][i].text = (sliderValue * 0.5f) + "s";
                            break;
                        case 1:
                        case 4:
                            g.Value[i].maxValue = 30;
                            g.Value[i].value = sliderValue;
                            lst_examOptions_labels[g.Key][i].text = sliderValue.ToString();
                            break;
                        case 2:
                            g.Value[i].maxValue = 100;
                            g.Value[i].value = sliderValue;
                            lst_examOptions_labels[g.Key][i].text = sliderValue + "%";
                            break;
                        
                    }


                }
            }


        }

        void AddListByRoot(ExamType eType, GameObject root)
        {
            List<Slider> lst_sliders = new List<Slider>();

            var practiceTimeLimit = root.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Slider>();
            var practiceTimeLimit_label = root.transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

            var practiceRound = root.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Slider>();
            var practiceRound_label = root.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

            var passScore = root.transform.GetChild(3).GetChild(1).GetChild(0).GetComponent<Slider>();
            var passScore_label = root.transform.GetChild(3).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

            var examTimeLimit = root.transform.GetChild(4).GetChild(1).GetChild(0).GetComponent<Slider>();
            var examTimeLimit_label = root.transform.GetChild(4).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

            var examRound = root.transform.GetChild(5).GetChild(1).GetChild(0).GetComponent<Slider>();
            var examRound_label = root.transform.GetChild(5).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

            lst_examOptions_sliders.Add(eType,new List<Slider>() {
            practiceTimeLimit,practiceRound,passScore,examTimeLimit,examRound
            });

            lst_examOptions_labels.Add(eType, new List<TextMeshProUGUI>() {
            practiceTimeLimit_label,practiceRound_label,passScore_label,examTimeLimit_label,examRound_label
            });

        }


        public void ShowOptionCtrl()
        {
            SystemManager.Instance.curUserInfo = null;
            SystemManager.Instance.curExamType = ExamType.None;

            UpdateOptionsList();

        }

        void UpdateOptionsList()
        { 
        
        }

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
            

            saveBt.onClick.AddListener(() => {

                SystemManager.Instance.SaveOption();
            });

            SetSliders();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
