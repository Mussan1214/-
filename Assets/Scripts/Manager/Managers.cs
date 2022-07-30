using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers s_instance = null;
    public static Managers Instance => s_instance;

    private static MainManager s_main = new MainManager();
    public static MainManager Main { get { Init(); return s_main;}}
    

    private static ResourceManager s_resourceManager = new ResourceManager();
    public static ResourceManager Resource { get { Init(); return s_resourceManager; } }

    
    private static UIManager s_uiManager = new UIManager();
    public static UIManager UI { get { Init(); return s_uiManager; } }

    private static DataManager s_dataManager = new DataManager();
    public static DataManager Data { get { Init(); return s_dataManager; }}

    private void Start()
    {
        Init();
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject {name = "@Managers"};

            s_instance = Utils.GetOrAddComponent<Managers>(go);
            DontDestroyOnLoad(go);
            
            s_dataManager.Init();
            s_main.Init();

            Application.targetFrameRate = 60;
        }
    }
}