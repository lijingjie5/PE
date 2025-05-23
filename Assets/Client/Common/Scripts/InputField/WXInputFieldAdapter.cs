using UnityEngine;
using WeChatWASM;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

// 添加 InputField 组件的依赖
[RequireComponent(typeof(InputField))]
public class WXInputFieldAdapter : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    private InputField _inputField;
    private bool _isShowKeyboard = false;

    private void Start()
    {
        _inputField = GetComponent<InputField>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        ShowKeyboard(_inputField.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        if (!_inputField.isFocused)
        {
            HideKeyboard();
        }
    }

    private void OnInput(OnKeyboardInputListenerResult v)
    {
        Debug.Log("onInput");
        Debug.Log(v.value);
        
    }

    private void OnConfirm(OnKeyboardInputListenerResult v)
    {
        // 输入法confirm回调
        Debug.Log("onConfirm");
        Debug.Log(v.value);
        HideKeyboard();

        if (_inputField.isFocused)
        {
            _inputField.onSubmit.Invoke(v.value);
        }
        
    }

    private void OnComplete(OnKeyboardInputListenerResult v)
    {
        // 输入法complete回调
        Debug.Log("OnComplete");
        Debug.Log(v.value);
        HideKeyboard();
    }

    private void ShowKeyboard(string def)
    {
        if (_isShowKeyboard) return;

        WX.ShowKeyboard(new ShowKeyboardOption()
        {
            defaultValue = def,
            maxLength = 10,
            confirmType = "done"
        });

        //绑定回调
        WX.OnKeyboardConfirm(this.OnConfirm);
        WX.OnKeyboardComplete(this.OnComplete);
        WX.OnKeyboardInput(this.OnInput);
        _isShowKeyboard = true;
    }

    private void HideKeyboard()
    {
        if (!_isShowKeyboard) return;

        WX.HideKeyboard(new HideKeyboardOption());
        //删除掉相关事件监听
        WX.OffKeyboardInput(this.OnInput);
        WX.OffKeyboardConfirm(this.OnConfirm);
        WX.OffKeyboardComplete(this.OnComplete);
        _isShowKeyboard = false;
    }
}