using UnityEngine;
using UnityEditor;
using System.Collections;
 
class EditorScrips : EditorWindow
{
 
    [MenuItem("Scene Change/StartFromMenu")]
    public static void RunMainScene()
    {
        EditorApplication.OpenScene("Assets/Scenes/MainMenu.unity");
        EditorApplication.isPlaying = true;
    }
    
    [MenuItem("Scene Change/Go to main scene")]
    public static void GoToMainScene()
    {
        EditorApplication.OpenScene("Assets/Julien/FPS_Demo.unity");
    }
}
