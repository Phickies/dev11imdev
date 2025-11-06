using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{

    public int sceneID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchAndLoad()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("MainTim");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SaveSystem.Load();
    }

    public void playMainGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainTim");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("quitting game");
    }

}
