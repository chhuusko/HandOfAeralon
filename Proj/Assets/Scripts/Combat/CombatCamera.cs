using UnityEngine;
using UnityEngine.Playables;

public class CombatCamera : MonoBehaviour
{
    [System.Serializable]
    public class CameraBounds
    {
        [SerializeField] public Vector3 _southWest;
        [SerializeField] public Vector3 _northWest;
        [SerializeField] public Vector3 _northEast;
        [SerializeField] public Vector3 _southEast;

        public float GetMinX() {  return _southWest.x; }    
        public float GetMinZ() { return _southWest.z; }
        public float GetMaxX() {  return _northEast.x; }
        public float GetMaxZ() { return _northWest.z; }

    };

    [System.Serializable]
    public class CameraZoomController
    {
        [SerializeField] private int _zoomStep;
        [SerializeField] private int _zoomStepMax;
        [SerializeField] private Vector3[] _zoomAnglePositions = new Vector3[10];
        [SerializeField] private float[] _zoomAngleRotations = new float[10];

        public void UpdateZoomScroll()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if(scroll > 0) 
                _zoomStep++;

            if (scroll < 0)
                _zoomStep--;

            ClampZoomStep();

            
        }

        private void ClampZoomStep()
        {
            if (_zoomStep >= _zoomStepMax)
                _zoomStep = _zoomStepMax - 1;

            if (_zoomStep <= 0)
                _zoomStep = 0;
        }

        public Vector3 GetZoomPosition() { return _zoomAnglePositions[_zoomStep]; }
        public float GetZoomRotation() { return  _zoomAngleRotations[_zoomStep]; }
    };

    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private bool bIntroCinematicDone;
    [SerializeField] private float _cameraSpeed;
    [SerializeField] CameraBounds _cameraBounds;
    [SerializeField] CameraZoomController _cameraZoomController;


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
        ZoomCamera();
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

        ClampToCamerBounds();

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

    private void ClampToCamerBounds()
    {
        Vector3 position = transform.position;

        float x = position.x;
        float y = position.y;
        float z = position.z;

        if (position.x < _cameraBounds.GetMinX())
            x = _cameraBounds.GetMinX();
        
        if (position.x > _cameraBounds.GetMaxX())
            x = _cameraBounds.GetMaxX();

        if (position.z < _cameraBounds.GetMinZ())
            z = _cameraBounds.GetMinZ();

        if (position.z > _cameraBounds.GetMaxZ())
            z = _cameraBounds.GetMaxZ();


        transform.position = new Vector3(x, y, z);
    }

    private void ZoomCamera()
    {
        _cameraZoomController.UpdateZoomScroll();

        Vector3 currentPos = transform.position;
        Vector3 zoomPosition = _cameraZoomController.GetZoomPosition();
        float zoomRotation = _cameraZoomController.GetZoomRotation();
        float y = zoomPosition.y;
        Vector3 newPosition = new Vector3(currentPos.x, y, currentPos.z);
        
        transform.rotation = Quaternion.Euler(zoomRotation, transform.rotation.y, transform.rotation.z);
        transform.position = newPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3[] points = {
            _cameraBounds._southWest,
            _cameraBounds._northWest,
            _cameraBounds._northEast,
            _cameraBounds._southEast
        };

        Gizmos.DrawLineStrip(points, true);
    }

}
