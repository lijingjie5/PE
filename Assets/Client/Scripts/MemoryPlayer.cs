using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BLG
{
    public class MemoryPlayer : BaseExamPlayer
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

        List<GameObject> lst_anims_left_stm1 = new List<GameObject>();
        List<GameObject> lst_anims_right_stm1 = new List<GameObject>();
        List<GameObject> lst_anims_left_stm2 = new List<GameObject>();
        List<GameObject> lst_anims_right_stm2 = new List<GameObject>();
                

        protected override void OnInit()
        {
            for (int i = 0; i < 4; i++)
            {
                lst_anims_left_stm1.Add(transform.GetChild(0).GetChild(0).GetChild(i).gameObject);
                lst_anims_right_stm1.Add(transform.GetChild(0).GetChild(1).GetChild(i).gameObject);

                lst_anims_left_stm2.Add(transform.GetChild(1).GetChild(0).GetChild(i).gameObject);
                lst_anims_right_stm2.Add(transform.GetChild(1).GetChild(1).GetChild(i).gameObject);
            }
                        
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void SetRoundA()
        {
            //Debug.Log("SetRoundA");

            int dropId_1 = Random.Range(0, 4);
            int dropId_2 = Random.Range(0, 4);

            if (dropId_1 != dropId_2)
                dropId_1 = Random.Range(0, 100) < 50 ? dropId_1 : dropId_2;


            int dropId_3 = Random.Range(0, 4);
            int dropId_4 = Random.Range(0, 4);

            if (dropId_3 != dropId_4)
                dropId_3 = Random.Range(0, 100) < 50 ? dropId_3 : dropId_4;

            
            if (Random.Range(0, 100) < 50)
            {
                dropId_3 = dropId_1;
                dropId_4 = dropId_2;
            }
            
           
            if (dropId_1 == dropId_3 && dropId_2 == dropId_4)
            {
                curAnswer = ExamAnswer.Congruent;
            }
            else 
            {
                curAnswer = ExamAnswer.Incongruent;
            }

            for (int i = 0; i < lst_anims_left_stm1.Count; i++)
            {
                lst_anims_left_stm1[i].SetActive(i == dropId_1);
            }

            for (int i = 0; i < lst_anims_right_stm1.Count; i++)
            {
                lst_anims_right_stm1[i].SetActive(i == dropId_2);
            }

            for (int i = 0; i < lst_anims_left_stm2.Count; i++)
            {
                lst_anims_left_stm2[i].SetActive(i == dropId_3);
            }

            for (int i = 0; i < lst_anims_right_stm2.Count; i++)
            {
                lst_anims_right_stm2[i].SetActive(i == dropId_4);
            }

        }

        protected override void SetRoundB()
        {
            //Debug.Log("SetRoundB");
        }

        protected override void SetRoundB_Button()
        {
            //Debug.Log("SetRoundB_Button");

            leftBt.GetComponentInChildren<Image>().sprite = carBtSprite;
            leftBt.GetComponentInChildren<Image>().color = Color.white;

            leftBtText.text = "Ò»ÖÂ";

            rightBt.GetComponentInChildren<Image>().sprite = carBtSprite;
            rightBt.GetComponentInChildren<Image>().color = Color.white;

            rightBtText.text = "²»Í¬";
        }


    }
}
