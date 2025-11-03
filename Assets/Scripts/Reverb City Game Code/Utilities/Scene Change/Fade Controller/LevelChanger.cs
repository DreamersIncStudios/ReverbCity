using System;
using System.Threading.Tasks;
using DreamersInc.SceneManagement;
using DreamersInc.ServiceLocatorSystem;
using UnityEngine;


public class LevelChanger : MonoBehaviour
{
    static Animator animator;
    static bool changeLevel;

    private int levelToLoad;

    private void Awake()
    {
        if (ServiceLocator.Global.TryGetComponent(this.GetType(), out var service))
            ServiceLocator.Global.Unregister(this.GetType(), service);
        ServiceLocator.Global.Register(this.GetType(), this);

        animator = GetComponent<Animator>();
    }
    private void OnDisable()
    {
    
        ServiceLocator.Global.Unregister(this.GetType(), this);

    }

    private void Start()
    {
    }

    public void FadeOut(bool change = true)
    {
        animator.SetTrigger("FadeOut");
        changeLevel = change;
    }

    public async Task FadeOutManually()
    {
        animator.SetTrigger("FadeOutManual");
        changeLevel = false;
        await Task.Delay(500);
    }

    public void FadeIn()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fade_In")) return;
        animator.SetTrigger("FadeIn");
    }

    public void FadeToLevel(int levelIndex)
    {
        FadeToLevel((uint)levelIndex);
    }

    public void FadeToLevel(uint levelIndex)
    {
        changeLevel = true;
        levelToLoad = (int)levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public async void OnFadeComplete()
    {
        if (changeLevel)
            await ServiceLocator.Global.Get<SceneLoader>().LoadSceneGroup(levelToLoad);
        await Task.Delay(500);
        // FadeIn();
    }
}