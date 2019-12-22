using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace TAHL.WAVE_BENDER {
    public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject TooltipGO;
        public string TooltipText;
        public float timeoutTime;
        private RectTransform _rectTransform; 
        private TextMeshProUGUI _tooltipText; 
        private IEnumerator _routine = null;
        private bool _showText = false;
        // Start is called before the first frame update
        void Start()
        {
            if(string.IsNullOrEmpty(TooltipText) || !TooltipGO)
            {
                throw new System.Exception("TooltipController parameters not set");
            }
            
            this._rectTransform = GetComponent<RectTransform>();
            this._tooltipText = this.TooltipGO.GetComponentInChildren<TextMeshProUGUI>();
            this._tooltipText.text = this.TooltipText;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _showText = true;
            _routine = ShowInDelay();
            StartCoroutine(_routine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _showText = false;
            TooltipGO.SetActive(false);
        }

        IEnumerator ShowInDelay() {
            yield return new WaitForSeconds(timeoutTime);
            TooltipGO.SetActive(_showText);
        }
    }
}
