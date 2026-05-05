using UnityEngine;

public class ObjectShooterAnimation : MonoBehaviour
{
    public Animator shooterAnimator;
    public string shootTriggerName = "Shoot";

    public void PlayShootAnimation()
    {
        shooterAnimator.SetTrigger(shootTriggerName);
    }
}