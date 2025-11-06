using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject player;
    public GameObject camera;
    public bool isPaused = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (player != null)
        {
            var controller = player.GetComponent<PlayerControllers>();
            if (controller != null)
            {
                controller.enabled = false;
            }
        }
        

    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        
        if (player != null)
        {
            var controller = player.GetComponent<PlayerControllers>(); 
            if (controller != null)
            {
                controller.enabled = true;
            }
        }
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SaveGame()
    {
        SaveSystem.Save();
        Debug.Log("game saved");
    }

    public void LoadGame()
    {
        SaveSystem.Load();
        Debug.Log("Game loaded");
        Resume();
    }

    public void QuitToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainTim");
    }
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
