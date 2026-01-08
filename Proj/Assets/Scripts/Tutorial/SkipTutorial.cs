using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipTutorial : MonoBehaviour
{
    private static List<SkipTutorial> allToggles = new();
    private Toggle toggle;

    private void OnEnable()
    {
        allToggles.Add(this);
        toggle = GetComponent<Toggle>();
        toggle.isOn = Tutorial.SkipTutorial;
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
        allToggles.Remove(this);
    }

    private void OnToggleChanged(bool isOn)
    {
        Tutorial.SkipTutorial = isOn;

        if (isOn) Tutorial.Instance.HidePopups();

        foreach (var t in allToggles)
        {
            if (t != this)
            {
                t.toggle.onValueChanged.RemoveListener(t.OnToggleChanged);
                t.toggle.isOn = Tutorial.SkipTutorial;
                t.toggle.onValueChanged.AddListener(t.OnToggleChanged);
            }
        }
    }
}
