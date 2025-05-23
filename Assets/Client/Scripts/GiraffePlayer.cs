using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BLG
{
    public class GiraffePlayer : BaseExamPlayer
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

        List<GameObject> lst_pic1 = new List<GameObject>();
        List<GameObject> lst_pic2 = new List<GameObject>();

        int stim1Rot;
        int offsetRot = 45;

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
            lst_pic1.Add(transform.GetChild(0).GetChild(0).gameObject);
            lst_pic1.Add(transform.GetChild(0).GetChild(1).gameObject);

            lst_pic2.Add(transform.GetChild(1).GetChild(0).gameObject);
            lst_pic2.Add(transform.GetChild(1).GetChild(1).gameObject);
        }

        protected override void SetRoundA()
        {
            //Debug.Log("SetRoundA");
            int dropId1 = Random.Range(0, 100) < 50 ? 0 : 1;
            
            int dropId2 = Random.Range(0, 100) < 50 ? 0 : 1;

            curAnswer = dropId1 == dropId2 ? ExamAnswer.Congruent : ExamAnswer.Incongruent;

            for (int i = 0; i < lst_pic1.Count; i++)
            {
                var col = lst_pic1[i].GetComponent<Image>().color;
                col.a = 1f;
                lst_pic1[i].GetComponent<Image>().color = col;

                if (i == dropId1)
                {
                    lst_pic1[i].SetActive(true);
                    stim1Rot = Random.Range(0, 360);
                    lst_pic1[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, stim1Rot);
                }
                else
                {
                    lst_pic1[i].SetActive(false);
                }
            }

            for (int i = 0; i < lst_pic2.Count; i++)
            {
                var col = lst_pic2[i].GetComponent<Image>().color;
                col.a = 1f;
                lst_pic2[i].GetComponent<Image>().color = col;

                if (i == dropId2)
                {
                    lst_pic2[i].SetActive(true);

                    int newRot = (int)GetRandomAngle();

                    Debug.Log(" "  + newRot + " " + stim1Rot);

                    lst_pic2[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, newRot);
                }
                else 
                {
                    lst_pic2[i].SetActive(false);
                }
            }

        }

        public int GetRandomAngle()
        {
            int minAngle = stim1Rot - offsetRot;
            int maxAngle = stim1Rot + offsetRot;

            // 确保角度在 0 到 360 的范围内
            minAngle = NormalizeAngle(minAngle);
            maxAngle = NormalizeAngle(maxAngle);

            // 生成一个随机角度
            int randomAngle = Random.Range(0, 360);

            // 如果随机角度在不允许的范围内，则调整到另一边的范围
            if (randomAngle > minAngle && randomAngle < maxAngle)
            {
                randomAngle = randomAngle < (minAngle + maxAngle) / 2 ? maxAngle : minAngle;
            }

            return randomAngle;
        }

        private int NormalizeAngle(int angle)
        {
            // 将角度标准化到 0 到 360 的范围
            angle = angle % 360;
            return angle < 0 ? angle + 360 : angle;
        }

        protected override void SetRoundB()
        {
            //Debug.Log("SetRoundB");

            for (int i = 0; i < lst_pic1.Count; i++)
            {
                var col = lst_pic1[i].GetComponent<Image>().color;
                col.a = 0.33f;
                lst_pic1[i].GetComponent<Image>().color = col;
                                
            }
        }

        protected override void SetRoundB_Button()
        {
            //Debug.Log("SetRoundB_Button");

            leftBt.GetComponentInChildren<Image>().sprite = carBtSprite;
            leftBt.GetComponentInChildren<Image>().color = Color.white;

            leftBtText.text = "一致";

            rightBt.GetComponentInChildren<Image>().sprite = carBtSprite;
            rightBt.GetComponentInChildren<Image>().color = Color.white;

            rightBtText.text = "不同";
        }


    }
}
