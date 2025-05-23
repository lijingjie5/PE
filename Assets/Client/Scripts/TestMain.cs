using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;


public class TestMain : MonoBehaviour
{
    public static TestMain Instance { get; private set; }

    public InputField id;
    public InputField contant;
    public Button saveBt;
    public Button dirBt;
    public Text tt;

    public static string stringText;

    private static WXFileSystemManager _fileSystemManager;

    // 路径
    // 注意WX.env.USER_DATA_PATH后接字符串需要以/开头
    private static readonly string PathPrefix = WX.env.USER_DATA_PATH + "/ReadFileAndWriteFile";
    //private static readonly string Path = PathPrefix + "/hello.txt";

    // 数据
    //private string _stringData = "String Data ";
    private byte[] _bufferData = { 66, 117, 102, 102, 101, 114, 32, 68, 97, 116, 97, 32 };

    // 更新文件内容
    private static void UpdateFileContent(string path)
    {
        // 使用UTF8编码显示文件内容
        Instance.tt.text = _fileSystemManager.ReadFileSync(path, "utf8");
        Debug.Log(Instance.tt.text);
    }

    /*// 回调函数
    private Action<WXTextResponse> onWriteFileSuccess = (res) =>
    {
        UpdateFileContent();
        WX.ShowModal(
            new ShowModalOption()
            {
                content = "WriteFile Success, Result: " + JsonMapper.ToJson(res)
            }
        );
        
    };
    private Action<WXTextResponse> onWriteFileFail = (res) =>
    {
        WX.ShowModal(
            new ShowModalOption() { content = "WriteFile Fail, Result: " + JsonMapper.ToJson(res) }
        );
    };*/

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        WX.InitSDK((int code) => {
            // 你的主逻辑

            // 获取全局唯一的文件管理器
            _fileSystemManager = WX.GetFileSystemManager();

            // 检查并创建目录
            if (_fileSystemManager.AccessSync(PathPrefix) != "access:ok")
            {
                _fileSystemManager.MkdirSync(PathPrefix, true);
            }


            saveBt.onClick.AddListener(SaveBt);
            dirBt.onClick.AddListener(DirBt);

            WX.ReportGameStart();
        });
    }

    public void SaveBt()
    {
        var eid = id.text;
        string Path = PathPrefix +"/" + eid+ ".txt";
        // 创建文件
        var fd = _fileSystemManager.OpenSync(new OpenSyncOption() { filePath = Path, flag = "w+" });

        // 写入初始数据
        _fileSystemManager.WriteSync(
            new WriteSyncStringOption() { fd = fd, data = contant.text }
        );

        WriteFileSync(contant.text, Path, "string", "utf8");
    }

    public void DirBt()
    {
        var res = _fileSystemManager.ReaddirSync(PathPrefix);

        // 更新文件列表
        UpdateResults(res);

        WX.ShowToast(new ShowToastOption() { title = "ReadDirSync Success" });
    }

    // 更新文件列表
    private void UpdateResults(string[] fileList)
    {
        Instance.tt.text = string.Empty;
        // 遍历文件列表，将每个文件添加到结果列表中
        foreach (var file in fileList)
        {
            Instance.tt.text += "/" + file;
            /*GameManager.Instance.detailsController.AddResult(
                new ResultData() { initialContentText = file }
            );*/
            Debug.Log(file);
        }
    }

    // 同步写入文件
    private void WriteFileSync(string contant, string path, string dataType, string encoding)
    {
        if (dataType == "string")
        {
            if (encoding == "null")
            {
                _fileSystemManager.WriteFileSync(path, contant);
            }
            else
            {
                _fileSystemManager.WriteFileSync(path, contant, encoding);
            }
        }
        else
        {
            if (encoding == "null")
            {
                _fileSystemManager.WriteFileSync(path, _bufferData);
            }
            else
            {
                _fileSystemManager.WriteFileSync(path, _bufferData, encoding);
            }
        }

        // 更新文件内容
        UpdateFileContent(path);
        WX.ShowToast(new ShowToastOption() { title = "WriteFile Success" });
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
