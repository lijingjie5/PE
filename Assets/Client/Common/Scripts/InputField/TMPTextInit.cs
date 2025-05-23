using BLG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 添加 Text 组件的依赖
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPTextInit : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        // 获取 Text 组件
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        // 如果 GameManager 的字体已经加载，直接设置 Text 的字体
        if (SystemManager.Instance.font != null)
        {
            _text.font = TMP_FontAsset.CreateFontAsset(SystemManager.Instance.font);
        }
        else
        {
            
        }
    }

    private void OnDestroy()
    {
        // 移除字体加载事件监听器
        //SystemManager.Instance.OnFontLoaded -= OnFontLoaded;
    }

    // 当字体加载完成时，设置 Text 的字体
    private void OnFontLoaded(Font font)
    {
        
    }
}
