using UnityEngine;
using UnityEngine.TextCore.Text;

public class FootstepSound : MonoBehaviour
{
    public void Footstep()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.BarbarianFootsteps, transform.position);
    }
}
