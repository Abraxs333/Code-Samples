using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimatorScript : MonoBehaviour
{

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Deground()
    {
        animator.SetBool("IsGrounded", false);
    }

}
