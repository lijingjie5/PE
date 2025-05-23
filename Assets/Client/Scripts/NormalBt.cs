using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BLG
{
    public class NormalBt : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool isBtPressed;
        public UnityEvent onClick;
                
        void RaiseOnclick()
        {
            onClick?.Invoke();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1);
            isBtPressed = true;
                        
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            gameObject.transform.localScale = Vector3.one;

            if (isBtPressed)
            {
                RaiseOnclick();
                SystemManager.Instance.PlayBtClick();
                //Debug.Log("btClick");
            }

            isBtPressed = false;
        }
                                
    }
}
