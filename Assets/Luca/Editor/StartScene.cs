using UnityEngine;
using UnityEditor;
using System.Collections;
 
class EditorScrips : EditorWindow
{
 
    [MenuItem("Scene Change/StartFromMenu")]
    public static void RunMainSceneFusion()
    {
        EditorApplication.OpenScene("Assets/Multi/Scene/Lobby/1.Intro.unity");
        EditorApplication.isPlaying = true;
    }
    
    [MenuItem("Scene Change/Go to main scene")]
    public static void GoToMainSceneFusion()
    {
        EditorApplication.OpenScene("Assets/Multi/Scene/GamePlay/Server Authority Fusion.unity");
    }
    
    [MenuItem("Scene Change/Go to Luca scene")]
    public static void GoToLucaScene()
    {
        EditorApplication.OpenScene("Assets/Multi/Scene/GamePlay/L_Server Authority Fusion.unity");
    }
    
    
    
    [MenuItem("Scene Change/StartFromMenu------------------Old")]
    public static void RunMainScene()
    {
        EditorApplication.OpenScene("Assets/Scenes/MainMenu.unity");
        EditorApplication.isPlaying = true;
    }
    
    [MenuItem("Scene Change/Go to main scene------------------Old")]
    public static void GoToMainScene()
    {
        EditorApplication.OpenScene("Assets/Julien/FPS_Demo.unity");
    }
}
