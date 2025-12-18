using UnityEngine;
using UnityEngine.UI;

public class SwapMainMenuImages : MonoBehaviour
{
    [SerializeField] private Sprite sprite16x9;
    [SerializeField] private Sprite sprite21x9;
    [SerializeField] private Image targetImage;

    void Start()
    {
        float aspect = (float)Screen.width / (float)Screen.height;

        if (aspect >= 2.0f)
        {
            targetImage.sprite = sprite21x9;
        }
        else
        {
            targetImage.sprite = sprite16x9;
        }
    }
}
