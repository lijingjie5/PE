using LitJson;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using UnityEngine;
using WeChatWASM;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using DG.Tweening;

namespace BLG
{
    [DefaultExecutionOrder(-100)]
    public class SystemManager : BLGPersistentSingleton<SystemManager>
    {
        public AudioClip btSound;
        public AudioClip result;
        public AudioClip resultFail;
        public AudioClip snd_swipe;
        public AudioClip clockTik;

        public AudioSource audioSource;

        public string versionInfo;
        public List<Color32> lst_userColors = new List<Color32>();

        private static WXFileSystemManager _fileSystemManager;

        public List<UserInfo> lst_userInfos = new List<UserInfo>();

        public List<ExamInfo> lst_user_examInfos = new List<ExamInfo>();

        public IReadOnlyDictionary<ExamType, ExamInfo> dic_user_examInfos => lst_user_examInfos.ToDictionary(x => x.examType);

        public MainMenuCtrl mainMenuCtrl;
        public UserInfoCtrl userInfoCtrl;
        public ExamInfoCtrl examInfoCtrl;
        public OptionCtrl optionCtrl;

        public Font font;

        public ExamRunType curExamRunType;

        public UserInfo m_curUserInfo;
        public UserInfo curUserInfo
        {
            get {
                return m_curUserInfo;
            }
            set {

                if (value == null)
                {
                    m_curUserInfo = null;
                    lst_user_examInfos.Clear();
                }
                else 
                {
                    if (m_curUserInfo == value)
                        return;

                    m_curUserInfo = value;
                    GetUserExamInfo();
                }

            }
        }

        public OptionInfo curOption;

        public IReadOnlyDictionary<ExamType, ExamOption> dic_examOptions => curOption.lst_examOptions.ToDictionary(x => x.examType);
        public int soundVolume => curOption.soundVolume;

        public ExamType curExamType;

        public bool isBackFromExam;

        // 路径
        // 注意WX.env.USER_DATA_PATH后接字符串需要以/开头
        private static readonly string PathPrefix = WX.env.USER_DATA_PATH + "/PhysicalExam";
        private static readonly string UserPath = PathPrefix + "/userInfos.txt";
        private static readonly string OptionPath = PathPrefix + "/options.txt";


        protected override void Awake()
        {
            Debug.Log("systemManager Awake");
            
            if (_instance == null)
            {
                Application.targetFrameRate = 60;
                var init = FindObjectOfType<InitManager>();
                if (init != null)
                {
                    font = init.font;
                    versionInfo = init.versionInfo;
                    lst_userColors.Clear();
                    foreach (var g in init.lst_userColors)
                    {
                        lst_userColors.Add(g);
                    }

                    btSound = init.btSound;
                    result = init.result;
                    resultFail = init.resultFail;
                    snd_swipe = init.snd_swipe;
                    clockTik = init.clockTik;
                }
                                

                audioSource = gameObject.AddComponent<AudioSource>();

                btSound.LoadAudioData();
                result.LoadAudioData();
                resultFail.LoadAudioData();
                snd_swipe.LoadAudioData();
                clockTik.LoadAudioData();

                // 获取全局唯一的文件管理器
                _fileSystemManager = WX.GetFileSystemManager();

                GetUserInfos();
                GetOptionInfo();

                WX.ReportGameStart();
            }

            base.Awake();

            Debug.Log("SystemManager Awake finished");
                        
            /*
            WX.InitSDK((int code) => {
                // 你的主逻辑

                Debug.Log(code);

                WX.SetPreferredFramesPerSecond(60);
                             

                WX.OnShow(Show);

                WX.OnHide(Hide);

                GetUserInfos();

                WX.ReportGameStart();

                Debug.Log("Awake finished");

            });*/
        }

                
        
        public enum SoundPlayType { clockTik, btSound, snd_swipe , result , resultFail }

        void PlaySound(SoundPlayType soundPlayType)
        {
            if (curOption.soundVolume == 0)
                return;

            AudioClip clip = null;
            float modVol = 0.5f;
            switch (soundPlayType)
            {
                case SoundPlayType.clockTik: clip = clockTik; modVol = 0.5f * curOption.soundVolume/100f; break;
                case SoundPlayType.btSound: clip = btSound; modVol = 0.5f * curOption.soundVolume / 100f; break;
                case SoundPlayType.snd_swipe: clip = snd_swipe; modVol = 0.3f * curOption.soundVolume / 100f; break;
                case SoundPlayType.result: clip = result; modVol = 0.5f * curOption.soundVolume / 100f; break;
                case SoundPlayType.resultFail: clip = resultFail; modVol = 0.5f * curOption.soundVolume / 100f; break;
            }

            if(clip!=null)
                audioSource.PlayOneShot(clip, modVol);
        }

        public void PlayClockTik()
        {
            PlaySound(SoundPlayType.clockTik);
        }

        public void PlayBtClick()
        {
            PlaySound(SoundPlayType.btSound);
        }

        public void PlaySwipe()
        {
            PlaySound(SoundPlayType.snd_swipe);
        }

        public void PlayResult(bool isWin)
        {
            if (isWin)
            {
                PlaySound(SoundPlayType.result);
            }
            else 
            {
                PlaySound(SoundPlayType.resultFail);
            }
        }

        public void InitMainScene()
        {
            mainMenuCtrl = FindObjectOfType<MainMenuCtrl>(true);
            userInfoCtrl = FindObjectOfType<UserInfoCtrl>(true);
            examInfoCtrl = FindObjectOfType<ExamInfoCtrl>(true);
            optionCtrl = FindObjectOfType<OptionCtrl>(true);

            var version = GameObject.Find("version");
            if(version!=null)
                version.GetComponent<TextMeshProUGUI>().text = "v" + versionInfo;

            ShowMainScene();

        }
        

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("systemManager Start");
            InitMainScene();
        }

        void GetUserExamInfo()
        {
            lst_user_examInfos.Clear();

            // 检查并创建目录
            if (_fileSystemManager.AccessSync(PathPrefix) != "access:ok")
            {
                _fileSystemManager.MkdirSync(PathPrefix, true);
            }
                        
            string userExamPath = PathPrefix + "/" + curUserInfo.id + ".txt";
                        

            if (_fileSystemManager.AccessSync(userExamPath) != "access:ok")
            {
                lst_user_examInfos = new List<ExamInfo>();
            }
            else
            {
                
                var _fd_user = _fileSystemManager.OpenSync(new OpenSyncOption() { filePath = userExamPath, flag = "r" });
                var dd = _fileSystemManager.ReadFileSync(userExamPath, "utf8");
                if (string.IsNullOrEmpty(dd))
                {
                    lst_user_examInfos = new List<ExamInfo>();
                    
                }
                else
                {
                    //string decompString = dd;
                    string decompString = DecompressStringFromBase64(dd);
                    lst_user_examInfos = JsonMapper.ToObject<List<ExamInfo>>(decompString);

                }

                _fileSystemManager.CloseSync(new CloseSyncOption() { fd = _fd_user });
            }

            Debug.Log("lst_user_examInfos count   " + lst_user_examInfos.Count);
        }

        OptionInfo CreateNewOption()
        {
            OptionInfo op = new OptionInfo();
            op.soundVolume = 100;

            op.lst_examOptions.Add(new ExamOption(ExamType.Car));
            op.lst_examOptions.Add(new ExamOption(ExamType.FlankerFish));
            op.lst_examOptions.Add(new ExamOption(ExamType.Memory));
            op.lst_examOptions.Add(new ExamOption(ExamType.Switcher));
            op.lst_examOptions.Add(new ExamOption(ExamType.Giraffe));
            op.lst_examOptions.Add(new ExamOption(ExamType.Stroop));

            return op;
        }

        void GetOptionInfo()
        {
            // 检查并创建目录
            if (_fileSystemManager.AccessSync(PathPrefix) != "access:ok")
            {
                _fileSystemManager.MkdirSync(PathPrefix, true);
            }

            if (_fileSystemManager.AccessSync(OptionPath) != "access:ok")
            {
                Debug.Log("CreateNewOption");
                curOption = CreateNewOption();

            }
            else
            {
                var _fd_user = _fileSystemManager.OpenSync(new OpenSyncOption() { filePath = OptionPath, flag = "r+" });
                var dd = _fileSystemManager.ReadFileSync(OptionPath, "utf8");
                if (string.IsNullOrEmpty(dd))
                {
                    curOption = CreateNewOption();
                }
                else
                {
                    //string decompString = dd;
                    string decompString = DecompressStringFromBase64(dd);
                    curOption = JsonMapper.ToObject<OptionInfo>(decompString);
                }

                _fileSystemManager.CloseSync(new CloseSyncOption() { fd = _fd_user });
            }

            Debug.Log("Option count   " + curOption.lst_examOptions.Count);
        }

        
        void GetUserInfos()
        {

            // 检查并创建目录
            if (_fileSystemManager.AccessSync(PathPrefix) != "access:ok")
            {
                _fileSystemManager.MkdirSync(PathPrefix, true);
            }

            if (_fileSystemManager.AccessSync(UserPath) != "access:ok")
            {
                Debug.Log("Create new saveFile");
                lst_userInfos = new List<UserInfo>();
            }
            else
            {
                var _fd_user = _fileSystemManager.OpenSync(new OpenSyncOption() { filePath = UserPath, flag = "r+" });
                var dd = _fileSystemManager.ReadFileSync(UserPath, "utf8");
                if (string.IsNullOrEmpty(dd))
                {
                    lst_userInfos = new List<UserInfo>();
                }
                else
                {
                    //string decompString = dd;
                    string decompString = DecompressStringFromBase64(dd);
                    lst_userInfos = JsonMapper.ToObject<List<UserInfo>>(decompString);
                }

                _fileSystemManager.CloseSync(new CloseSyncOption() { fd = _fd_user });
            }

            Debug.Log("user count   " + lst_userInfos.Count);
            
        }
        
        float timeFakeDelay;
        public void DelayAction(float delay, float duration, Action callback)
        {
            DOTween.To(() => timeFakeDelay, x => timeFakeDelay = x, 1, duration).From(0).SetDelay(delay).OnComplete(() => { callback?.Invoke(); });
        }

        

        public void GameBackToMain()
        {
            isBackFromExam = true;
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }

        public void ShowMainScene()
        {
            if (mainMenuCtrl == null)
                return;

            
            if (isBackFromExam)
            {
                if (!dic_user_examInfos.ContainsKey(curExamType) || dic_user_examInfos[curExamType].lst_examRecords.Count == 0)
                {
                    OpenUser(curUserInfo);
                    isBackFromExam = false;
                }
                else
                {
                    ShowExamInfo(curExamType);
                    isBackFromExam = false;
                }
            }
            else
            {
                mainMenuCtrl.gameObject.SetActive(true);
                userInfoCtrl.gameObject.SetActive(false);
                examInfoCtrl.gameObject.SetActive(false);
                optionCtrl.gameObject.SetActive(false);

                mainMenuCtrl.ShowMainMenu();

            }

        }

        public void ShowOption()
        {
            mainMenuCtrl.gameObject.SetActive(false);
            userInfoCtrl.gameObject.SetActive(false);
            examInfoCtrl.gameObject.SetActive(false);
            optionCtrl.gameObject.SetActive(true);

            optionCtrl.ShowOptionCtrl();
        }

        public void ShowExamInfo(ExamType examType)
        {
            curExamType = examType;

            mainMenuCtrl.gameObject.SetActive(false);
            userInfoCtrl.gameObject.SetActive(false);
            examInfoCtrl.gameObject.SetActive(true);
            optionCtrl.gameObject.SetActive(false);

            examInfoCtrl.ShowExamInfo();

        }

        public void LoadExamScene(ExamType examType, ExamRunType examRunType)
        {
            curExamType = examType;
            curExamRunType = examRunType;

            SceneManager.LoadScene("ExamScene", LoadSceneMode.Single);

        }


        public void OpenUser(UserInfo info)
        {
            curUserInfo = info;

            mainMenuCtrl.gameObject.SetActive(false);
            userInfoCtrl.gameObject.SetActive(true);
            examInfoCtrl.gameObject.SetActive(false);
            optionCtrl.gameObject.SetActive(false);

            userInfoCtrl.ShowUserDataInfo();
        }

        
        

        void Hide(GeneralCallbackResult res)
        {
            Debug.Log("Hide");
            //SaveUser();
        }

        void Show(OnShowListenerResult res)
        {
            Debug.Log("Show");
            //WX.OnHide(Hide);
        }
                

        // Update is called once per frame
        void Update()
        {

        }

        
        public void DelUser(UserInfo user)
        {
            if (!lst_userInfos.Contains(user))
                return;

           
            string userExamPath = PathPrefix + "/" + user.id + ".txt";

            if (_fileSystemManager.AccessSync(userExamPath) != "access:ok")
            {
                //说明还没有创建出测试数据
                WX.ShowToast(new ShowToastOption() { title = "删除成功", });
                lst_userInfos.Remove(user);
                SaveUser();
            }
            else 
            {
                var res = _fileSystemManager.UnlinkSync(userExamPath);

                if (res == "unlink:ok")
                {
                    WX.ShowToast(new ShowToastOption() { title = "删除成功", });
                    lst_userInfos.Remove(user);
                    SaveUser();
                    Debug.Log(user.id + " User deleted and Saved");
                }
                else
                {
                    WX.ShowToast(new ShowToastOption() { title = "删除失败" });
                    Debug.Log(userExamPath + " " + res);
                    return;
                }
            }

            
                       
        }

        public void AddUser(UserInfo user)
        {
            if (lst_userInfos.Contains(user))
                return;

            lst_userInfos.Insert(0, user);

            SaveUser();

            Debug.Log(user.id + " User Added and Saved");
        }

        public void UpdateUserInfo(UserInfo user)
        {
            if (!lst_userInfos.Contains(user))
                return;

            SaveUser();

            Debug.Log(user.id + " User Updated and Saved");
        }

        void SaveUser()
        {
            string saveString = JsonMapper.ToJson(lst_userInfos);

            // 打开文件，并写入初始数据
            var _fd_user = _fileSystemManager.OpenSync(new OpenSyncOption() { filePath = UserPath, flag = "w+" });

            _fileSystemManager.WriteSync(new WriteSyncStringOption() { fd = _fd_user, data = CompressStringToBase64(saveString) });
            //_fileSystemManager.WriteSync(new WriteSyncStringOption() { fd = _fd_user, data = saveString });

            _fileSystemManager.CloseSync(new CloseSyncOption() { fd = _fd_user });

            
        }

        public void SaveOption()
        {
            string saveString = JsonMapper.ToJson(curOption);

            // 打开文件，并写入初始数据
            var _fd_user = _fileSystemManager.OpenSync(new OpenSyncOption() { filePath = OptionPath, flag = "w+" });

            var res = _fileSystemManager.WriteSync(new WriteSyncStringOption() { fd = _fd_user, data = CompressStringToBase64(saveString) });
            //var res = _fileSystemManager.WriteSync(new WriteSyncStringOption() { fd = _fd_user, data = saveString });

            if (res!=null)
            {
                WX.ShowToast(new ShowToastOption() { title = "保存完成", });
            }

            _fileSystemManager.CloseSync(new CloseSyncOption() { fd = _fd_user });


        }

        public void AddExamInfo(ExamInfo examInfo)
        {
            if (lst_user_examInfos.Contains(examInfo))
                return;

            if (dic_user_examInfos.ContainsKey(examInfo.examType))
            {
                lst_user_examInfos.Remove(dic_user_examInfos[examInfo.examType]);
            }

            lst_user_examInfos.Add(examInfo);
            SaveUserExam();

            Debug.Log("add examInfo and saved");
        }


        public string GetUserExamString()
        {
            var dt = new UserExamData();
            dt.user = curUserInfo;
            dt.lst_examInfos = lst_user_examInfos;

            return JsonMapper.ToJson(dt);
        }

        public void RemoveExamInfo(ExamType  examType)
        {
            if (dic_user_examInfos.ContainsKey(examType))
            {
                lst_user_examInfos.Remove(dic_user_examInfos[examType]);
            }
                        
            SaveUserExam();

            Debug.Log("remove " + examType.ToString() + " and saved");
        }

        void SaveUserExam()
        {
            if (curUserInfo == null)
                return;

            string userExamPath = PathPrefix + "/" + curUserInfo.id + ".txt";

            string saveString = JsonMapper.ToJson(lst_user_examInfos);

            // 打开文件，并写入初始数据
            var _fd_user = _fileSystemManager.OpenSync(new OpenSyncOption() { filePath = userExamPath, flag = "w+" });

            //_fileSystemManager.WriteSync(new WriteSyncStringOption() { fd = _fd_user, data = saveString });
            _fileSystemManager.WriteSync(new WriteSyncStringOption() { fd = _fd_user, data = CompressStringToBase64(saveString) });

            _fileSystemManager.CloseSync(new CloseSyncOption() { fd = _fd_user });

        }

        #region 工具


        public static string CompressStringToBase64(string text)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(text);

            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionMode.Compress))
                {
                    gzip.Write(byteArray, 0, byteArray.Length);
                }

                return Convert.ToBase64String(output.ToArray());
            }
        }

        public static string DecompressStringFromBase64(string base64String)
        {
            byte[] byteArray = Convert.FromBase64String(base64String);

            using (var input = new MemoryStream(byteArray))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gzip.CopyTo(output);
                return Encoding.UTF8.GetString(output.ToArray());
            }
        }

        #endregion
    }
}
