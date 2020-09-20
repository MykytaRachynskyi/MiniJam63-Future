using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] bool buttonTrue;

    public void doExitGame() {
        Application.Quit();
    }

    public void destroyMenuOnCall() {
        Destroy(gameObject);
    }
}
