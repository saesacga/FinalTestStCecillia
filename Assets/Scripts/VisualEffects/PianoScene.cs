using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PianoScene : MonoBehaviour
{
    private int _pianoSceneReady;
    private void Awake()
    {
        _pianoSceneReady = PlayerPrefs.GetInt("pianoActivated", 0); //Cargar la informacion guardada
        if (_pianoSceneReady == 1)
        {
            GetComponent<Animator>().SetInteger("Fade", 2);
            PlayerPrefs.SetInt("pianoActivated", 0);
        }
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        GetComponent<Animator>().SetInteger("Fade", 1);
    }

    public void ChangeScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
        if (sceneNumber == 1)
        {
            PlayerPrefs.SetInt("pianoActivated", 1); //Guardar la informacion
        }
    }
}
