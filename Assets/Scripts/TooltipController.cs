using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace TAHL.WAVE_BENDER {
    [RequireComponent(typeof(Image))]
    public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string TooltipText;
        public Image Tooltip;
        private RectTransform _rectTransform; 
        private GameObject _tooltipGO;
        private TextMeshProUGUI _tooltipText; 
        private IEnumerator _routine = null;
        private bool _showText = false;
        // Start is called before the first frame update
        void Start()
        {
            if(string.IsNullOrEmpty(TooltipText) || !Tooltip)
            {
                throw new System.Exception("TooltipController parameters not set");
            }
            
            this._rectTransform = GetComponent<RectTransform>();
            this._tooltipGO = Tooltip.gameObject;
            this._tooltipText = this._tooltipGO.GetComponentInChildren<TextMeshProUGUI>();
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
            _tooltipGO.SetActive(false);
        }

        IEnumerator ShowInDelay() {
            yield return new WaitForSeconds(0.2f);
            _tooltipGO.SetActive(_showText);
        }
    }
}
