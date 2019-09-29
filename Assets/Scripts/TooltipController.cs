using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace TAHL.WAVE_BENDER {
    [RequireComponent(typeof(Image))]
    public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string TooltipText;
        private RectTransform _rectTransform; 
        private GameObject _tooltipGO;
        private TextMeshProUGUI _tooltipText; 
        // Start is called before the first frame update
        void Start()
        {
            this._rectTransform = GetComponent<RectTransform>();
            this._tooltipGO = transform.GetChild(0).gameObject;
            this._tooltipText = this._tooltipGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            this._tooltipText.text = this.TooltipText;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tooltipGO.SetActive(true);
            this._rectTransform.sizeDelta += new Vector2(5, 5);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tooltipGO.SetActive(false);
            this._rectTransform.sizeDelta -= new Vector2(5, 5);
        }
    }
}
