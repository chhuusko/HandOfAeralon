using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GridShaderHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Material m_Material;
    [SerializeField] private Transform unitTransform;

    [SerializeField] private Vector3 minBound, maxBound;

    [SerializeField] private float radius;
    [SerializeField] private float speed;
    [SerializeField] private Color highlightColor;

    void Start()
    {
        float halfWidth  = transform.localScale.x * 0.5f;
        float halfDepth = transform.localScale.z * 0.5f;

        minBound = transform.position - new Vector3(halfWidth, 0.0f, halfDepth);
        maxBound = transform.position + new Vector3(halfWidth, 0.0f, halfDepth);
    }

   
    void Update()
    {
        float horz = Input.GetAxis("Horizontal"); 
        float vert = Input.GetAxis("Vertical");   

        Vector3 move = new Vector3(horz, 0, vert).normalized;
        
        unitTransform.position += move * speed * Time.deltaTime;

        Vector3 localPos = transform.InverseTransformPoint(unitTransform.position);

        // If UV is mirrored, flip X or Z as needed
        float u = localPos.x + 0.5f; // 0 → 1 across X
        float v = localPos.z + 0.5f; // 0 → 1 across Z

        // If mirrored, flip:
        u = 1f - u; // flip U if it goes opposite
        v = 1f - v; // flip V if needed

        Vector2 uv = new Vector2(u, v);


        m_Material.SetVector("_HighlightPosition", uv);
        m_Material.SetFloat("_HighlightRadius", radius);
        m_Material.SetColor("_HighlightColor", highlightColor);
        
        if (Input.GetMouseButton(0))
        {
   


        }
    }
}

