using UnityEngine;
using UnityEngine.EventSystems;
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
            _zoom = Mathf.Clamp01(_zoom + scroll * _zoomSpeed);

            _height = _minHeight + heightCurve.Evaluate(_zoom) * _maxHeight;

            _tilt = _minTilt + tiltCurve.Evaluate(_zoom) * _maxTilt;
        }

        public float GetHeight() { return _height; }
        public float GetTilt() { return _tilt; }
        public float GetSmoothSpeed() { return _smoothSpeed; }
        
    };

    [SerializeField] private PlayableDirector _timelineDirector;
    [SerializeField] private bool bIntroCinematicDone;
    [SerializeField] CameraBounds _cameraBounds;
    [SerializeField] CameraZoomController _cameraZoomController;
    [SerializeField] Vector3 _lastMousePosition;


    [SerializeField] private float _mouseMoveScreenLimitX, _mouseMoveScreenLimitY;
    [SerializeField] private float _moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bIntroCinematicDone = false;
        _timelineDirector.stopped += OnTimelineStopped;
    }

    void OnDestroy()
    {
        _timelineDirector.stopped -= OnTimelineStopped;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);

            foreach (var r in results)
            {
                DebugLog.CJLog("Hit UI: " + r.gameObject.name);
            }
        }
        Move();
        ZoomCamera();
    }

    private void Move()
    {
        Vector3 cameraMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x > Screen.width - _mouseMoveScreenLimitX)
            cameraMovement += Vector3.right;
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x < _mouseMoveScreenLimitX)
            cameraMovement += Vector3.left;
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y > Screen.height - _mouseMoveScreenLimitY)
            cameraMovement += Vector3.forward;
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y < _mouseMoveScreenLimitY)
            cameraMovement += Vector3.back;



        // NOTE (Calle): Moving camera with scroll button pressed
        if (Input.GetMouseButtonDown(2))
        {
            _lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 dragDelta = Input.mousePosition - _lastMousePosition;

            // Convert drag delta to world movement
            Vector3 move = new Vector3(-dragDelta.x, 0, -dragDelta.y) * (_moveSpeed * 0.05f) * Time.deltaTime;

            transform.position += move;

            _lastMousePosition = Input.mousePosition;
        }
        else
        {

            // NOTE (Calle): Normalized for consisten diagonal movement
            if (cameraMovement != Vector3.zero)
                transform.position += cameraMovement.normalized * Time.deltaTime * _moveSpeed;


        }

        ClampToCamerBounds();

        if (Input.GetKeyDown(KeyCode.Space) && !IsIntroCinematicDone())
            InterruptIntroCinematic();

    }

    private void InterruptIntroCinematic()
    {
        CutSceneManager.GetInstance().HideCutsceneCanvas();
        bIntroCinematicDone = true;
        _timelineDirector.Stop();
    }

    public bool IsIntroCinematicDone() { return bIntroCinematicDone; }
    public void PlayIntroCinematic()
    {
        _timelineDirector.Play();
    }

    private void OnTimelineStopped(PlayableDirector pd)
    {
        InterruptIntroCinematic();
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

        // Handle height
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, _cameraZoomController.GetHeight(),
                           Time.deltaTime * _cameraZoomController.GetSmoothSpeed());
        transform.position = pos;

        // Handle tilt (safe version)
        float targetTilt = _cameraZoomController.GetTilt();
        Quaternion targetRot = Quaternion.Euler(targetTilt, transform.eulerAngles.y, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * _cameraZoomController.GetSmoothSpeed()
        );
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
