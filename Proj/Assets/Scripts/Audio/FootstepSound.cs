using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public void Footstep()
    {
        CharacterClass charClass = gameObject.GetComponent<Character>().GetCharacterClass();

        switch(charClass)
        {
            case CharacterClass.Barbarian: AudioManager.Instance.PlayOneShot(FMODEvents.Instance.BarbarianFootsteps, transform.position); break;
            case CharacterClass.Bard: AudioManager.Instance.PlayOneShot(FMODEvents.Instance.BardFootsteps, transform.position); break;
            case CharacterClass.Rogue: AudioManager.Instance.PlayOneShot(FMODEvents.Instance.RogueFootsteps, transform.position); break;
            case CharacterClass.Sorceress: AudioManager.Instance.PlayOneShot(FMODEvents.Instance.SorceressFootsteps, transform.position); break;
        }
    }
}
