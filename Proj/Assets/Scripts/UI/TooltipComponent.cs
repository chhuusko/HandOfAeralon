// Joel Larsson Wendt | jola6902

using UnityEngine;

public class TooltipComponent : MonoBehaviour
{
    [SerializeField] private string _tooltip;

    public string GetTooltip()
    {
        return _tooltip;
    }
}
