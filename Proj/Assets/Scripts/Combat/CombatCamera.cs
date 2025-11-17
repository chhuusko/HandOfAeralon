using UnityEngine;
using UnityEngine.Playables;

public class CombatCamera : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private bool bIntroCinematicDone;
    [SerializeField] private float _cameraSpeed;
    
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

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 cameraMovement = Vector3.zero;
        Vector3 cameraSpeedVector = new Vector3(_cameraSpeed, _cameraSpeed, _cameraSpeed);

        if (Input.GetKey(KeyCode.D))
            cameraMovement += Vector3.right;
        if (Input.GetKey(KeyCode.A))
            cameraMovement += Vector3.left;
        if (Input.GetKey(KeyCode.W))
            cameraMovement += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            cameraMovement += Vector3.back;

        cameraMovement = Vector3.Scale(cameraMovement, cameraSpeedVector);

        cameraMovement *= Time.deltaTime;

        if (cameraMovement != Vector3.zero)
            transform.position = cameraMovement + transform.position;

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
