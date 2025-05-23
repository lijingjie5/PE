using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BLG
{
    public class StroopPlayer : BaseExamPlayer
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

        
        TextMeshProUGUI firstLab;
        TextMeshProUGUI secondLab;

        List<string> labs = new List<string>() {

            "ºìÉ«","À¶É«","ºìÆì" ,"À¶Æì","ºìº£","À¶Ìì","ºìµÆ" ,"À¶»¨"

        };

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
            firstLab = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            secondLab = transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }

        int timeFakeDelay;

        List<string> lst_showString = new List<string>() { "ºìÉ«", "À¶É«", "ºìÆì", "À¶Æì", "º£À¶", "»ÆÉ«", "ÂÌÉ«", "ÌìÀ¶", "Ñªºì" , "À¶»¨", "ºìµÆ" };

        void rdTest()
        {
            firstLab.text = lst_showString[Random.Range(0, lst_showString.Count)];
            firstLab.color = Random.Range(1, 100) < 50 ? new Color32(0, 224, 255, 255) : new Color32(255, 80, 80, 255);
            //Debug.Log("rdTest");
        }

        Sequence mySequence;
        protected override void SetRoundA()
        {
            //Debug.Log("SetRoundA");
            int dropId1 = Random.Range(0, 100) < 50 ? 0 : 1;

            //var tween = DOTween.To(() => timeFakeDelay, x => timeFakeDelay = x, 1, 0.5f).SetDelay(0.5f).From(0);

            mySequence = DOTween.Sequence();
            for (int i = 0; i < 15; i++)
            {
                mySequence.Append(DOTween.To(() => timeFakeDelay, x => timeFakeDelay = x, 1, 0.1f).From(0));
                mySequence.AppendCallback(rdTest);
            }
                        

            curAnswer = dropId1 == 0 ? ExamAnswer.Congruent : ExamAnswer.Incongruent;

            secondLab.color = dropId1 == 0 ? new Color32(0, 224, 255, 255) : new Color32(255, 80, 80, 255);
            secondLab.text = labs[Random.Range(0, labs.Count)];
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

            leftBtText.text = "À¶";

            rightBt.GetComponentInChildren<Image>().sprite = carBtSprite;
            rightBt.GetComponentInChildren<Image>().color = Color.white;

            rightBtText.text = "ºì";
        }


    }
}
