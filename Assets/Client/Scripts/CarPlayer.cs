using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BLG
{
    public class CarPlayer : BaseExamPlayer
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

        List<GameObject> lst_cars_stm1 = new List<GameObject>();
        List<GameObject> lst_cars_stm2 = new List<GameObject>();

        List<Vector2> lst_poes = new List<Vector2>() 
        { 
            new Vector2(-224, 320), new Vector2( 224, 320),
            new Vector2(-224, 0), new Vector2( 224, 0),
            new Vector2(-224, -320), new Vector2( 224, -320)
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
            for (int i = 0; i < 4; i++)
            {
                lst_cars_stm1.Add(transform.GetChild(1).GetChild(0).GetChild(i).gameObject);
                lst_cars_stm2.Add(transform.GetChild(2).GetChild(0).GetChild(i).gameObject);
            }
                        
        }

        protected override void SetRoundA()
        {
            //Debug.Log("SetRoundA");

            int dropId_1 = Random.Range(0, 6);
            int dropId_2 = Random.Range(0, 6);
            
            if(dropId_1 != dropId_2)
                dropId_1 = Random.Range(0, 100) < 50 ? dropId_1 : dropId_2;

            curAnswer = dropId_1 == dropId_2?  ExamAnswer.Congruent : ExamAnswer.Incongruent;

            int carId1 = Random.Range(0, lst_cars_stm1.Count);
            for (int i = 0; i < lst_cars_stm1.Count; i++)
            {
                if (i == carId1)
                {
                    lst_cars_stm1[i].SetActive(true);
                    lst_cars_stm1[i].GetComponent<RectTransform>().anchoredPosition = lst_poes[dropId_1];
                    
                    var localScale = lst_cars_stm1[i].GetComponent<RectTransform>().localScale;
                    localScale.x *= Random.Range(0, 100) < 50 ? 1 : -1;
                    lst_cars_stm1[i].GetComponent<RectTransform>().localScale = localScale;
                }
                else {

                    lst_cars_stm1[i].SetActive(false);
                }
            }

            int carId2 = Random.Range(0, lst_cars_stm2.Count);
            for (int i = 0; i < lst_cars_stm2.Count; i++)
            {
                if (i == carId2)
                {
                    lst_cars_stm2[i].SetActive(true);
                    lst_cars_stm2[i].GetComponent<RectTransform>().anchoredPosition = lst_poes[dropId_2];
                    
                    var localScale = lst_cars_stm2[i].GetComponent<RectTransform>().localScale;
                    localScale.x *= Random.Range(0, 100) < 50 ? 1 : -1;
                    lst_cars_stm2[i].GetComponent<RectTransform>().localScale = localScale;
                }
                else
                {
                    lst_cars_stm2[i].SetActive(false);
                }
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
