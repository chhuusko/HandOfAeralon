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
        [Range(0, 1)]
        [SerializeField] private float _zoom = 0.5f;
        [SerializeField] private float _zoomSpeed = 2.0f;
        [SerializeField] private float _smoothSpeed = 10.0f;

        [SerializeField] private AnimationCurve heightCurve;
        [SerializeField] private AnimationCurve tiltCurve;

        [SerializeField] private float _height;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeight;

        [SerializeField] private float _tilt;
        [SerializeField] private float _minTilt;
        [SerializeField] private float _maxTilt;
        [SerializeField] private float _scroll;


        public void UpdateZoomScroll()
        {
            float scroll = -Input.GetAxis("Mouse ScrollWheel");
            _zoom = Mathf.Clamp01(_zoom + scroll * _zoomSpeed * Time.deltaTime);

            _height = _minHeight + heightCurve.Evaluate(_zoom) * _maxHeight;

            _tilt = _minTilt + tiltCurve.Evaluate(_zoom) * _maxTilt;
        }

        public float GetHeight() { return _height; }
        public float GetTilt() { return _tilt; }
        public float GetSmoothSpeed() { return _smoothSpeed; }

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

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, _cameraZoomController.GetHeight(), Time.deltaTime * _cameraZoomController.GetSmoothSpeed());
        transform.position = pos;

        Vector3 rot = transform.eulerAngles;
        rot.x = Mathf.Lerp(rot.x, _cameraZoomController.GetTilt(), Time.deltaTime * _cameraZoomController.GetSmoothSpeed());
        transform.eulerAngles = rot;
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
