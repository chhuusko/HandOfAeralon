using UnityEngine;
using UnityEngine.Playables;

public class CombatCamera : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private bool bIntroCinematicDone;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bIntroCinematicDone = false;
        timelineDirector.stopped += OnTimelineStopped;
    }

    void OnDestroy()
    {
        timelineDirector.stopped -= OnTimelineStopped;
    }

    public bool IsIntroCinematicDone() { return bIntroCinematicDone; }
    public void PlayIntroCinematic()
    {
        timelineDirector.Play();
    }

    private void OnTimelineStopped(PlayableDirector pd)
    {
        bIntroCinematicDone = true;
        timelineDirector.Stop();
    }

}
