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

    // ·��
    // ע��WX.env.USER_DATA_PATH����ַ�����Ҫ��/��ͷ
    private static readonly string PathPrefix = WX.env.USER_DATA_PATH + "/ReadFileAndWriteFile";
    //private static readonly string Path = PathPrefix + "/hello.txt";

    // ����
    //private string _stringData = "String Data ";
    private byte[] _bufferData = { 66, 117, 102, 102, 101, 114, 32, 68, 97, 116, 97, 32 };

    // �����ļ�����
    private static void UpdateFileContent(string path)
    {
        // ʹ��UTF8������ʾ�ļ�����
        Instance.tt.text = _fileSystemManager.ReadFileSync(path, "utf8");
        Debug.Log(Instance.tt.text);
    }

    /*// �ص�����
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
            // ������߼�

            // ��ȡȫ��Ψһ���ļ�������
            _fileSystemManager = WX.GetFileSystemManager();

            // ��鲢����Ŀ¼
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
        // �����ļ�
        var fd = _fileSystemManager.OpenSync(new OpenSyncOption() { filePath = Path, flag = "w+" });

        // д���ʼ����
        _fileSystemManager.WriteSync(
            new WriteSyncStringOption() { fd = fd, data = contant.text }
        );

        WriteFileSync(contant.text, Path, "string", "utf8");
    }

    public void DirBt()
    {
        var res = _fileSystemManager.ReaddirSync(PathPrefix);

        // �����ļ��б�
        UpdateResults(res);

        WX.ShowToast(new ShowToastOption() { title = "ReadDirSync Success" });
    }

    // �����ļ��б�
    private void UpdateResults(string[] fileList)
    {
        Instance.tt.text = string.Empty;
        // �����ļ��б���ÿ���ļ���ӵ�����б���
        foreach (var file in fileList)
        {
            Instance.tt.text += "/" + file;
            /*GameManager.Instance.detailsController.AddResult(
                new ResultData() { initialContentText = file }
            );*/
            Debug.Log(file);
        }
    }

    // ͬ��д���ļ�
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

        // �����ļ�����
        UpdateFileContent(path);
        WX.ShowToast(new ShowToastOption() { title = "WriteFile Success" });
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
