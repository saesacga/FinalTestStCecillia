using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private bool _shootCompleted;
    public Image imageTutorial;
    [SerializeField] private Sprite[] _mouseSprites;
    [SerializeField] private Sprite[] _controlSprites;

    public void ShowControlsUI(int index)
    {
        imageTutorial.color = Color.LerpUnclamped(imageTutorial.color, Color.white, 10f * Time.deltaTime);
        
        if (ActionMapReference.isGamepad) 
        { 
            imageTutorial.sprite = _controlSprites[index];
        }
        else 
        { 
            imageTutorial.sprite = _mouseSprites[index]; 
        }
    } //Lerp
    public void ShowControlsUI(int index, Color color)
    {
        imageTutorial.color = color;
        
        if (ActionMapReference.isGamepad) 
        { 
            imageTutorial.sprite = _controlSprites[index];
        }
        else 
        { 
            imageTutorial.sprite = _mouseSprites[index]; 
        }
    } //No lerp
    
    public void HideControlsUI() //Lerp
    {
        imageTutorial.color = Color.LerpUnclamped(imageTutorial.color, Color.clear, 5f * Time.deltaTime);
    }
    public void HideControlsUI(Color color) //No Lerp
    {
        imageTutorial.color = color;
    }
}
