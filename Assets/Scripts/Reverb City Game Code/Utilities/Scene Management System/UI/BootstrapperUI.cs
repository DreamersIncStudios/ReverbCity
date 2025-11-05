using System.Collections;
using DreamersInc.UIToolkitHelpers;
using UnityEngine;
using UnityEngine.UIElements;
using static DreamersInc.ReverbCity.GameCode.UI.UIExtensionMethods;
public class BootstrapperUI : UIManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Generate());
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        StartCoroutine(Generate());
    }

    public  override IEnumerator Generate()
    {
        var root = document.rootVisualElement;
        root.Clear();
        root.styleSheets.Add(_styleSheet);
        var label = Create<Label>("LoadingLabel");
        label.text = "Loading...";
        root.Add(label);
        var progress = Create<ProgressBar>("LoadingBar");
        root.Add(progress);
        yield return null;
    }
}
