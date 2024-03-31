using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Palomas : MonoBehaviour
{
    [SerializeField] private ItemData _materialNeed;
    [SerializeField] private int _amountRequired;
    [SerializeField] private Animator _matriarcaAnimator;
    private int _amountProgress = 0;
    private bool _alreadyExecuted;

    public void StartEvent(ItemData itemData)
    {
        if (itemData == _materialNeed)
        {
            if (_amountProgress < _amountRequired)
            {
                _amountProgress++;
            }
            if (_amountProgress == _amountRequired && _alreadyExecuted == false)
            {
                GetComponent<Animator>().SetBool("novela", true);
                _matriarcaAnimator.SetBool("palomasEnd", true);
                GetComponent<PlayableDirector>().Play();
                _alreadyExecuted = true;
            }
        } 
    }
    
    private void ControlCameraBlends(int lookAt)
    {
        if (lookAt == 0)
        {
            StartCoroutine(ActionMapReference.ActivateLooking(false, "PalomasCam"));
        }
        else if (lookAt == 1)
        {
            StartCoroutine(ActionMapReference.ActivateLooking(false, "MatriarcaCam"));
        }
        else
        {
            _matriarcaAnimator.SetBool("palomasEnd", false);
            GetComponent<Animator>().SetBool("novela", false);
            StartCoroutine(ActionMapReference.ActivateLooking(true, "POVCam"));
        }
    }

    private void ActivateMove(int move)
    {
        if (move == 0)
        {
            ActionMapReference.playerInput.actions.FindAction("Move").Enable();
        }
        else if (move == 1)
        {
            ActionMapReference.playerInput.actions.FindAction("Move").Disable();
        }
    }
}
