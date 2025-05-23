using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BLG
{
    public class SwitcherPlayer : BaseExamPlayer
    {
        public Sprite carBtSprite;
        
        public bool m_isSkipStim1;
        public override bool isSkipStim1 => m_isSkipStim1;
        public bool m_isDelayCloseStim1;
        public override bool isDelayCloseStim1 => m_isDelayCloseStim1;

        public bool m_isKeepStim1;
        public override bool isKeepStim1 => m_isKeepStim1;
        public override ExamAnswer correctAnswer => curAnswer;

        ExamAnswer curAnswer = ExamAnswer.Congruent;

        TextMeshProUGUI firstNum;
        TextMeshProUGUI crossNum;
        TextMeshProUGUI finalNum;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        
        protected override void OnInit()
        {
            firstNum = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            crossNum = transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            finalNum = transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }

        int timeFakeDelay;
        //分类记录
        int dropId1;

        void rdTest()
        {
            firstNum.text = Random.Range(1, 10).ToString();
            firstNum.color = Color.white;
            //Debug.Log("rdTest");
        }

        Sequence mySequence;
        protected override void SetRoundA()
        {
            //Debug.Log("SetRoundA");

            dropId1 = Random.Range(0, 100) < 50 ? 0 : 1;

            firstNum.text = Random.Range(1, 10).ToString();
            firstNum.color = Color.white;

            crossNum.text = "+";
            crossNum.color = dropId1 == 0 ? new Color32(0, 224, 255, 255) : new Color32(255, 80, 80, 255);
            crossNum.gameObject.SetActive(false);

            mySequence = DOTween.Sequence();
            for (int i = 0; i < 30; i++)
            {
                mySequence.Append(DOTween.To(() => timeFakeDelay, x => timeFakeDelay = x, 1, 0.05f).From(0));
                mySequence.AppendCallback(rdTest);
            }

            SystemManager.Instance.DelayAction(0.6f, 0f, () => {

                crossNum.gameObject.SetActive(true);
            });
            
            if (dropId1 == 0)
            {
                int dropId2 = Random.Range(1, 10);
                curAnswer = dropId2 % 2 != 0 ? ExamAnswer.Congruent : ExamAnswer.Incongruent;
                finalNum.text = dropId2.ToString();
            }
            else 
            {
                int dropId3 = Random.Range(1, 5);
                int dropId4 = Random.Range(6, 10);

                int rd = Random.Range(0, 100) < 50 ? dropId3 : dropId4;
                curAnswer = rd<5 ? ExamAnswer.Congruent : ExamAnswer.Incongruent;
                finalNum.text = rd.ToString();
            }

        }

        protected override void SetRoundB()
        {
            //Debug.Log("SetRoundB");
            mySequence.Complete();
        }

        protected override void SetRoundB_Button()
        {
            //Debug.Log("SetRoundB_Button");

            leftBt.GetComponentInChildren<Image>().sprite = carBtSprite;
            leftBt.GetComponentInChildren<Image>().color = Color.white;

            if(dropId1==0)
                leftBtText.text = "奇数";
            else
                leftBtText.text = "小于5";

            if (gameCtrl.examRunType == ExamRunType.Exam)
            {
                leftBtText.text = "左";
            }


            rightBt.GetComponentInChildren<Image>().sprite = carBtSprite;
            rightBt.GetComponentInChildren<Image>().color = Color.white;

            if (dropId1 == 0)
                rightBtText.text = "偶数";
            else
                rightBtText.text = "大于5";

            if (gameCtrl.examRunType == ExamRunType.Exam)
            {
                rightBtText.text = "右";
            }
        }


    }
}
