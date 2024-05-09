using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    private bool pause;
    public GameObject pauseMenuObject;
    
    [SerializeField] private List<GameObject> _selectButtons = new List<GameObject>();
    [SerializeField] private GameObject _firstMenu;
    [SerializeField] private List<GameObject> _subMenus = new List<GameObject>();

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Continue()
    {
        Time.timeScale = 1f;
        
        pause = false;
        pauseMenuObject.SetActive(false);
        ActionMapReference.ActivateAllMaps();
    }
    public void SetSelectObject(int i) 
    {
        EventSystem.current.SetSelectedGameObject(null); 
        if (ActionMapReference.isGamepad) { EventSystem.current.SetSelectedGameObject(_selectButtons[i]); }
        else { EventSystem.current.SetSelectedGameObject(_selectButtons[3]); }
    }
    private void Update()
    {
        if (ActionMapReference.playerInput.actions["Pause"].WasPerformedThisFrame()) { TogglePauseMenu(); }
    }
    private void TogglePauseMenu()
    {
        pause = !pause;
        if (pause)
        {
            Time.timeScale = 0f;
            
            if (ActionMapReference.isGamepad) { EventSystem.current.SetSelectedGameObject(_selectButtons[0]); }
            else { EventSystem.current.SetSelectedGameObject(_selectButtons[3]); }
            pauseMenuObject.SetActive(true);
            ActionMapReference.ActivateUINavigation();

            _firstMenu.SetActive(true);
            foreach (GameObject elemento in _subMenus) { elemento.SetActive(false); }
        }
        else if (pause == false)
        {
            Time.timeScale = 1f;
            
            pauseMenuObject.SetActive(false);
            ActionMapReference.ActivateAllMaps();
        }
    }
}
