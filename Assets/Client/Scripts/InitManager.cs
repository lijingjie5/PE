using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

namespace BLG
{
    public class InitManager : MonoBehaviour
    {
        public List<Color32> lst_userColors = new List<Color32>();
        public string versionInfo;
        public Font font;

        public AudioClip btSound;
        public AudioClip result;
        public AudioClip resultFail;
        public AudioClip snd_swipe;
        public AudioClip clockTik;

        private void Awake()
        {
            if (SystemManager.Instance == null)
            {
                WX.InitSDK((int code) =>
                {
                    // ÄãµÄÖ÷Âß¼­

                    Debug.Log("WX.InitSDK " + code);

                    new GameObject("SystemManager", typeof(SystemManager)).GetComponent<SystemManager>();

                    Debug.Log("InitManager finish");

                });
            }
            
        }

        private void Start()
        {
            SystemManager.Instance.InitMainScene();
            Destroy(this.gameObject);

        }
    }
}
