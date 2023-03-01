using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform transition;
    private bool pressedPlay = false;
    public Animator animator;

    public void Play()
    {
        if (pressedPlay) return;
        pressedPlay = true;
        animator.SetTrigger("Change");

    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
