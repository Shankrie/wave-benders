using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public enum HoverActionType
    {
        ChangeSprite = 0,
        ChangeTextColor = 1,
        ChangeImageColor = 2,
        Animate = 3,
        IncreaseTransparency = 4
    }

    public Sprite NormalSprite;
    public Sprite ChangedSprite; 
    public Color NormalColor;
    public Color ChangedColor; 
    public float IncreaseTransparencyBy;
    public string AnimationTriggerName;
    public HoverActionType ActionType;

    private Animator _animator;
    private Image _image;
    private TextMeshProUGUI _UIText;

    private void Start() {
        if (ActionType == HoverActionType.Animate)
        {
            _animator = GetComponent<Animator>();
            if (!_animator)            
            {
                throw new Exception("Animator component is not connected for animating: " + gameObject.name);
            }
        }
        else if (ActionType == HoverActionType.ChangeSprite || ActionType == HoverActionType.ChangeImageColor || ActionType == HoverActionType.IncreaseTransparency)
        {
            _image = GetComponent<Image>();
            if (!_image)            
            {
                throw new Exception("Image component is not connected for changing sprite for: " + gameObject.name);
            }
        }
        else if (ActionType == HoverActionType.ChangeTextColor)
        {
            _UIText = GetComponent<TextMeshProUGUI>();
            if (!_UIText)  
            {
                throw new Exception("UI text component is not added to GO: " + gameObject.name);
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch(ActionType)
        {
            case HoverActionType.Animate:
                _animator.SetTrigger(AnimationTriggerName);
                break;
            case HoverActionType.ChangeTextColor:
                _UIText.color = ChangedColor;
                break; 
            case HoverActionType.ChangeImageColor:
                _image.color = ChangedColor;
                break; 
            case HoverActionType.ChangeSprite:
                _image.sprite = ChangedSprite;
                break;
            case HoverActionType.IncreaseTransparency:
                Color col = _image.color;
                _image.color = new Color(col.r, col.g, col.b, col.a + IncreaseTransparencyBy);
                break;
            default:
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch(ActionType)
        {
            case HoverActionType.ChangeTextColor:
                _UIText.color = NormalColor;
                break; 
            case HoverActionType.ChangeImageColor:
                _image.color = NormalColor;
                break; 
            case HoverActionType.ChangeSprite:
                _image.sprite = NormalSprite;
                break;
            case HoverActionType.IncreaseTransparency:
                Color col = _image.color;
                _image.color = new Color(col.r, col.g, col.b, col.a - IncreaseTransparencyBy);
                break;
            default:
                break;
        }
    }
}
