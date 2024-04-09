using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private bool _shootCompleted;
    [SerializeField] private Image _imageTutorial;
    [SerializeField] private Sprite[] _mouseSprites;
    [SerializeField] private Sprite[] _controlSprites;

    public void ShowControlsUI(int index)
    {
        _imageTutorial.color = Color.LerpUnclamped(_imageTutorial.color, Color.white, 10f * Time.deltaTime);
        
        if (ActionMapReference.isGamepad) 
        { 
            _imageTutorial.sprite = _controlSprites[index];
        }
        else 
        { 
            _imageTutorial.sprite = _mouseSprites[index]; 
        }
    } //Lerp
    public void ShowControlsUI(int index, Color color)
    {
        _imageTutorial.color = color;
        
        if (ActionMapReference.isGamepad) 
        { 
            _imageTutorial.sprite = _controlSprites[index];
        }
        else 
        { 
            _imageTutorial.sprite = _mouseSprites[index]; 
        }
    } //No lerp
    
    public void HideControlsUI() //Lerp
    {
        _imageTutorial.color = Color.LerpUnclamped(_imageTutorial.color, Color.clear, 5f * Time.deltaTime);
    }
    public void HideControlsUI(Color color) //No Lerp
    {
        _imageTutorial.color = color;
    }
}
