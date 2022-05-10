using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneChangement : MonoBehaviour
{

    public void GoToScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    
}
