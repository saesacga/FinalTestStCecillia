using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCredits : MonoBehaviour
{
    private static Animator _anim;
    
    private void Start()
    {
       _anim = GetComponent<Animator>();
    }

    public static void StartCredits()
    {
        _anim.SetBool("final", true);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
