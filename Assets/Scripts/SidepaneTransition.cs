using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TAHL.WAVE_BENDER
{
    public class SidepaneTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Animator Animator; 
        private bool _sidebarOpen;
        private bool _mouseExitAfterClick = false;
        private void Start() {
            if (!Animator) {
                throw new System.Exception("Animator is not set up");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(_sidebarOpen)
            {
                Animator.SetTrigger("DisabledSidepane");
                _sidebarOpen = false;
            }
            else
            {
                Animator.SetTrigger("SelectedSidepane");
                _sidebarOpen = true;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_sidebarOpen)
            {
                if(_mouseExitAfterClick) {
                    Animator.SetTrigger("HoverSelectedSidepane");
                }
            }
            else
            {
                Animator.SetTrigger("HoverSidepane");
            }
            _mouseExitAfterClick = false; 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_sidebarOpen)
            {
                Animator.SetTrigger("SelectedSidepane");
            }
            else
            {
                Animator.SetTrigger("DisabledSidepane");
            }
            _mouseExitAfterClick = true; 
        }
    }
}
