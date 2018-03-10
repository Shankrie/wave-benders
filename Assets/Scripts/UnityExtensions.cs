using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Extensions methods by TAHL - four potatoes productions
/// </summary>
namespace TAHL.WAVE_BENDER
{
    public static class GameObjectExtensions
    {
        public static T[] GetComponentsByTag<T>(this GameObject gameObject, string tag)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
            T[] components = new T[gameObjects.Length];
            for(int i = 0; i < gameObjects.Length; i++)
            {
                components[i] = gameObjects[i].GetComponent<T>();
            }
            return components;
        }

        public static GameObject GetChildByTag(this Transform transform, string tag)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                GameObject childGO = transform.GetChild(i).gameObject;
                if(childGO.tag == tag)
                {
                    return childGO;
                }
            }
            return null;
        }

        public static Transform GetChildByName(this Transform transform, string name)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name.ToLower() == name.ToLower())
                    return child;
            }
            return null;
        }

        public static bool IsTextOverflow(this RectTransform parentRect, RectTransform textComponentRect, int overflowWidth)
        {
            //This is the width the text would LIKE to be
            float prefferedWidth = LayoutUtility.GetPreferredWidth(textComponentRect);
            //This is the actual width of the text's parent container
            float parentWidth = parentRect.sizeDelta.x;
            // Check if overflows text width
            return prefferedWidth >= (parentWidth - overflowWidth);
        }

        public static void IncreaseSizeAndPos(this RectTransform transform, Vector2 positionMargin, Vector2 sizeMargin)
        {
            transform.anchoredPosition += positionMargin;
            transform.sizeDelta += sizeMargin;
        }
    }
}
