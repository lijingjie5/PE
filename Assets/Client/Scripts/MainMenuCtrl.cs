using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;


namespace BLG
{
    public class MainMenuCtrl : MonoBehaviour
    {
        public NormalBt addUserBt;
        public NormalBt delUserBt;
        public NormalBt optionBt;

        bool m_isDeletingUser;
        bool isDeletingUser
        {
            get => m_isDeletingUser;
            set  { m_isDeletingUser = value; UpdateDeleteUserState(); }
        }
        

        public ScrollRect scrollRect;
        public GameObject itemPrefab;
        public RectTransform content;

        public float itemHeight = 155;              // Height of a single item
        public float itemWidth = 155;               // Width of a single item
        
        int totalRowCount;             // Total number of rows based on item count
        int topRowIndex = 0;
        int visibleRowCount = 6;        // Number of visible rows (e.g. 8 rows)
        int totalItemCount = 0;     // Total number of items in the list
        int columnCount = 4;

        private List<GameObject> userBts = new List<GameObject>(); // Pool of visible items

        
        void Start()
        {
            //Debug.Log("MainMenuCtrl Start");
            addUserBt.onClick.AddListener(AddUser);
            delUserBt.onClick.AddListener(CheckDeleteUser);
            optionBt.onClick.AddListener(() => { SystemManager.Instance.ShowOption(); });
            //SystemManager.Instance.mainMenuCtrl = this;            

        }

        private void OnDestroy()
        {
            addUserBt.onClick.RemoveAllListeners();
            delUserBt.onClick.RemoveAllListeners();
            optionBt.onClick.RemoveAllListeners();
        }

        public void ShowMainMenu()
        {
            SystemManager.Instance.curUserInfo = null;
            SystemManager.Instance.curExamType = ExamType.None;

            UpdateUserList();
        }

        void CheckDeleteUser()
        {
            isDeletingUser = !isDeletingUser;
            
        }

        void UpdateDeleteUserState()
        {
            TextMeshProUGUI itemText = delUserBt.GetComponentInChildren<TextMeshProUGUI>();
            Image image = delUserBt.GetComponentInChildren<Image>();
            if (isDeletingUser)
            {
                itemText.text = "完成\n删除";
                image.color = Color.red;
                itemText.color = Color.red;
            }
            else 
            {
                itemText.text = "删除\n用户";
                image.color = Color.white;
                itemText.color = Color.white;
            }

            foreach (var g in userBts)
                RefreshUserBt(g);
        }
                

        void UpdateUserList()
        {
            foreach (var g in userBts)
            {
                Destroy(g);
            }
            userBts.Clear();

            totalItemCount =  SystemManager.Instance.lst_userInfos.Count;

            if (totalItemCount == 0)
                return;

            // Calculate total number of rows based on the total item count and columns
            totalRowCount = Mathf.CeilToInt((float)totalItemCount / columnCount);

            content.sizeDelta = new Vector2(content.sizeDelta.x, totalRowCount * itemHeight);

            for (int i = 0; i < visibleRowCount * columnCount; i++)
            {
                GameObject newBt = Instantiate(itemPrefab, content);

                var userBt = newBt.transform.Find("UserButton/btPic");
                if (userBt != null)
                {
                    newBt.GetComponent<UserButton>().userBt = userBt.gameObject;

                    userBt.GetComponent<NormalBt>().onClick.AddListener(() => {

                        var dataIndex = newBt.GetComponent<UserButton>().Index;
                        var userInfo = SystemManager.Instance.lst_userInfos[dataIndex];

                        if (isDeletingUser)
                        {
                            //Debug.Log("delete user " + userInfo.name);
                            DelUser(userInfo);
                        }
                        else
                        {
                            //Debug.Log("open user " + userInfo.name);
                            OpenUser(userInfo);
                        }
                                                                        
                    });
                }

                var delIcon = newBt.transform.Find("UserButton/btPic/DelIcon");
                if (delIcon != null)
                {
                    newBt.GetComponent<UserButton>().delIcon = delIcon.gameObject;
                    delIcon.gameObject.SetActive(isDeletingUser);
                }

                var picFrame = newBt.transform.Find("UserButton/btPic/picFrame");
                if (picFrame != null)
                {
                    newBt.GetComponent<UserButton>().btFrame = picFrame.gameObject;
                }

                newBt.SetActive(true);
                userBts.Add(newBt);
            }

            scrollRect.onValueChanged.AddListener(OnScrollChanged);

            UpdateVisibleItems();
        }

        private void OnScrollChanged(Vector2 scrollPos)
        {
            UpdateVisibleItems();
        }

        private void UpdateVisibleItems()
        {
            // Calculate which row is at the top based on the scroll position
            float contentY = content.anchoredPosition.y;
            topRowIndex = Mathf.FloorToInt( contentY / itemHeight);

            // Clamp the top row index to prevent overflow
            topRowIndex = Mathf.Max(Mathf.Clamp(topRowIndex, 0, totalRowCount - visibleRowCount), 0);
            
            // Update each visible item
            for (int row = 0; row < visibleRowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    int itemIndex = (topRowIndex + row) * columnCount + col;
                    
                    //Debug.Log(topRowIndex + " " + row +  " " + columnCount + " " + col + " " + itemIndex);

                    GameObject item = userBts[row * columnCount + col];

                    if (itemIndex < totalItemCount)
                    {
                        item.SetActive(true);
                        item.GetComponent<UserButton>().Index = itemIndex;
                        
                        UpdateItem(item);
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
        }

        void RefreshUserBt(GameObject item)
        {
            var dataIndex = item.GetComponent<UserButton>().Index;
            var userInfo = SystemManager.Instance.lst_userInfos[dataIndex];
                        
            TextMeshProUGUI itemText = item.GetComponentInChildren<TextMeshProUGUI>();
            itemText.text = userInfo.name.ToArray()[0].ToString();// +  " #" + dataIndex ;

            
            var co = SystemManager.Instance.lst_userColors[userInfo.colorId]; ;

            if (isDeletingUser)
                item.GetComponent<UserButton>().userBt.GetComponent<Image>().color = new Color(co.r / 512f, co.g / 512f, co.b / 512f, 0.5f);
            else
                item.GetComponent<UserButton>().userBt.GetComponent<Image>().color = new Color32(co.r, co.g, co.b, 128);

            item.GetComponent<UserButton>().btFrame.GetComponent<Image>().color = new Color32(co.r, co.g, co.b, 255);
                        
            item.GetComponent<UserButton>().delIcon.SetActive(isDeletingUser);
        }

        // Updates the item with its corresponding data
        private void UpdateItem(GameObject item)
        {
            var dataIndex = item.GetComponent<UserButton>().Index;
            //Debug.Log(dataIndex);
            
            //Debug.Log(SystemManager.Instance.lst_userInfos[dataIndex].id);
            // Example: Set text for the item (you can replace this with your own logic)
            // Set the item's position in the grid

            //update position
            RectTransform rt = item.GetComponent<RectTransform>();
            int row = dataIndex / columnCount;
            int col = dataIndex % columnCount;
            rt.anchoredPosition = new Vector2(col * itemWidth, -row * itemHeight);

            //update view
            RefreshUserBt(item);
        }

        void OpenUser(UserInfo userInfo)
        {
            SystemManager.Instance.OpenUser(userInfo);
        }

        void DelUser(UserInfo userInfo)
        {
            if (Application.isEditor)
            {
                SystemManager.Instance.DelUser(userInfo);
                UpdateUserList();
            }
            else 
            {
                WX.ShowModal(new ShowModalOption()
                {
                    title = "删除该用户的数据",
                    content = "数据将永久删除",
                    cancelText = "取消",
                    confirmText = "确定",
                    success = (res) => {
                        
                        if (res.confirm)
                        {
                            SystemManager.Instance.DelUser(userInfo);
                            UpdateUserList();
                            
                        }
                    }
                });
            }
            
        }

        void AddUser()
        {
            SystemManager.Instance.AddUser(CreateNewUser());

            UpdateUserList();

            isDeletingUser = false;
                        
        }

        UserInfo CreateNewUser()
        {
            var user = new UserInfo()
            {
                id = GenerateUniqueId(),
                name =  ((char)UnityEngine.Random.Range(65, 91)).ToString()+ ((char)UnityEngine.Random.Range(65, 91))+ ((char)UnityEngine.Random.Range(65, 91)),
                userCreateDate = DateTime.Now,
                colorId = UnityEngine.Random.Range(0, SystemManager.Instance.lst_userColors.Count),
                gender = Gender.Boy,
                age = "10",
                ptName = "Ken"
            };

            return user;
            
        }

        

        // Update is called once per frame
        void Update()
        {

        }

        public static long GenerateUniqueId()
        {
            // 获取当前时间的时间戳
            long timestamp = System.DateTime.Now.Ticks;
            // 生成一个随机数
            int randomValue = UnityEngine.Random.Range(1000, 10000);
            // 将时间戳和随机数组合起来生成唯一ID
            return math.abs(timestamp ^ randomValue);
        }
    }
}
