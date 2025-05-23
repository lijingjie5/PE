using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BLG
{
    public class FlankerPlayer : BaseExamPlayer
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
            new Vector2(0, 393),
            new Vector2(0, 183),
            new Vector2(0, 27), 
            new Vector2( 0, -183),
            new Vector2( 0, -393),
        };

        protected override void OnInit()
        {
            for (int i = 0; i < 25; i++)
            { 
                lst_cars_stm1.Add(transform.GetChild(0).GetChild(i).gameObject); 
            }

            for (int i = 0; i < 5; i++)
            {
                lst_cars_stm2.Add(transform.GetChild(1).GetChild(0).GetChild(i).gameObject);
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

            int dropId = Random.Range(0, 100) < 50 ? 0 : 1;
            curAnswer = dropId == 0 ? ExamAnswer.Congruent : ExamAnswer.Incongruent;
            //Debug.Log(curAnswer);

            for (int i = 0; i < 25; i++)
            {
                var localScale = lst_cars_stm1[i].GetComponent<RectTransform>().localScale;
                localScale.x = Mathf.Abs(localScale.x);
                if (i != 12)
                {
                    localScale.x *= Random.Range(0, 100) < 50 ? 1 : -1;
                    lst_cars_stm1[i].GetComponent<RectTransform>().localScale = localScale;
                }
               
            }

            for (int i = 0; i < 5; i++)
            {
                var localScale = lst_cars_stm2[i].gameObject.transform.localScale;
                localScale.x = Mathf.Abs(localScale.x);
                if (i == 2)
                {
                    localScale.x *= dropId ==0 ? 1 : -1;
                }
                else 
                {
                    localScale.x *= Random.Range(0, 100) < 50 ? 1 : -1;
                }

                lst_cars_stm2[i].GetComponent<RectTransform>().localScale = localScale;
            }

            int rdheightId = Random.Range(0, lst_poes.Count);
            var root = transform.GetChild(1).GetChild(0).gameObject;
            root.GetComponent<RectTransform>().anchoredPosition = lst_poes[rdheightId];
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

            leftBtText.text = "×ó";

            rightBt.GetComponentInChildren<Image>().sprite = carBtSprite;
            rightBt.GetComponentInChildren<Image>().color = Color.white;

            rightBtText.text = "ÓÒ";
        }


    }
}
