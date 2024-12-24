using UnityEngine;

public class LeverPath1 : MonoBehaviour
{
    public bool isMovingLever = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isMovingLever && animator.enabled == false)
        {
            animator.enabled = true;
        }
        if (isMovingLever && animator.enabled == true)
        {
            animator.SetBool("IsMovingLeverPath1", true);
        }
        else { animator.SetBool("IsMovingLeverPath1", false); }
    }
}
