using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class ResourceManager
{
    public Dictionary<string, Sprite> Sprites { get; set; } = new Dictionary<string, Sprite>();

    public void Init()
    {
    }

    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(Sprite))
        {
            if (Sprites.TryGetValue(path, out Sprite sprite))
                return sprite as T;

            Sprite loadSprite = Resources.Load<Sprite>(path);
            Sprites.Add(path, loadSprite);
            return loadSprite as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Instantiate(prefab, parent);
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;
        
        Object.Destroy(go);
    }
}
