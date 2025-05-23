using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BLG
{
    public interface IExamPlayer
    {        
        ExamAnswer correctAnswer { get; }

        void ShowStim(bool isStim1 = false, bool isStim2 = false);

        bool isSkipStim1 { get; }
        bool isDelayCloseStim1 { get; }

        bool isKeepStim1 { get; }
    }

    public abstract class BaseExamPlayer : MonoBehaviour, IExamPlayer
    {
        protected ExamGameCtrl gameCtrl;
        protected NormalBt leftBt;
        protected TextMeshProUGUI leftBtText;
        
        public GameObject stim1;
        public GameObject stim2;

        protected NormalBt rightBt;
        protected TextMeshProUGUI rightBtText;
        
        public abstract bool isSkipStim1 { get; }
        public abstract bool isDelayCloseStim1 { get; }
        public abstract bool isKeepStim1 { get; }

        public abstract ExamAnswer correctAnswer { get; }
        protected abstract void SetRoundA();
        protected abstract void SetRoundB();
        protected abstract void SetRoundB_Button();

        public void ShowStim(bool isStim1 = false, bool isStim2 = false)
        {
            Debug.Log(isStim1 + " " + isStim2);
            stim1.SetActive(isStim1);
            stim2.SetActive(isStim2);
        }
        protected abstract void OnInit();

        public void Init(ExamGameCtrl gameCtrl, NormalBt leftBt, TextMeshProUGUI leftBtText, NormalBt rightBt, TextMeshProUGUI rightBtText)
        {
            this.gameCtrl = gameCtrl;
            this.leftBt = leftBt; this.leftBtText = leftBtText;
            this.rightBt = rightBt; this.rightBtText = rightBtText;
            OnInit();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            if (gameCtrl == null)
                return;
            gameCtrl.OnSetTestRound_A += SetRoundA;
            gameCtrl.OnSetTestRound_B += SetRoundB;
            gameCtrl.OnSetTestRound_B_Button += SetRoundB_Button;
        }

        private void OnDisable()
        {
            if (gameCtrl == null)
                return;
            gameCtrl.OnSetTestRound_A -= SetRoundA;
            gameCtrl.OnSetTestRound_B -= SetRoundB;
            gameCtrl.OnSetTestRound_B_Button -= SetRoundB_Button;
        }

        
    }
}
