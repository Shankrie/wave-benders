using UnityEngine;

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
    }
}
