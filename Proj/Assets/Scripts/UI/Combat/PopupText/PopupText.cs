using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _destroyTime;
    [SerializeField] private Vector3 _randomStartPositionRange;
    private Vector3 _randomStartPosition;
    [SerializeField] GameObject _characterTarget;
    [SerializeField] Canvas _canvasParent;

    [SerializeField] float _fixedYPos;
    private void OnEnable()
    {
    }

    void Start()
    {
        Destroy(gameObject, _destroyTime);
       
    }

    void Update()
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = transform.position.y; // lock vertical tilt

        transform.LookAt(camPos);
        transform.Rotate(0, 180, 0);

        if(_characterTarget != null)
        {
            Vector3 characterPos = _characterTarget.transform.position;
            characterPos.y = _fixedYPos;
            transform.position = characterPos;
            
            transform.position += _randomStartPosition;
        }
        
    }

    public void Initialize(GameObject owner, Canvas canvasParent)
    {
        SetOwner(owner);    
        SetCanvasParent(canvasParent);
        SetRandomPosition();
    }

    public void SetOwner(GameObject characterTarget)
    {
        _characterTarget = characterTarget;
    }

    private void SetCanvasParent(Canvas canvasParent)
    {
        transform.SetParent(canvasParent.transform);
        transform.SetAsLastSibling();
    }

    private void SetRandomPosition()
    {

        if (_characterTarget != null)
        {
            Vector3 characterPos = _characterTarget.transform.position;
            characterPos.y = _fixedYPos;
            transform.position = characterPos;
        }

        float randomStartPositionX = Random.Range(-_randomStartPositionRange.x, _randomStartPositionRange.x);
        float randomStartPositionY = Random.Range(-_randomStartPositionRange.y, _randomStartPositionRange.y);
        float randomStartPositionZ = Random.Range(-_randomStartPositionRange.z, _randomStartPositionRange.z);

        _randomStartPosition = new Vector3(randomStartPositionX, randomStartPositionY, randomStartPositionZ);

        //Vector3 randomPos = new Vector3(randomStartPositionX, randomStartPositionY, randomStartPositionZ);
        //Vector3 randomPos = new Vector3(randomStartPositionX, _fixedYPos, randomStartPositionZ);
        //transform.position += randomPos;

        transform.position += _randomStartPosition;
    }

    public TMP_Text GetTextMesh() { return _text; }

}

