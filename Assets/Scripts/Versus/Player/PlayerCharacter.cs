using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int playerId = 1; // 1 o 2
    public Animator animator;

    public void PlayLaugh()
    {
        if (animator != null)
        {
            animator.SetTrigger("Laugh");
        }
    }

    public void PlayAngry()
    {
        if (animator != null)
        {
            animator.SetTrigger("Angry");
        }
    }
}
