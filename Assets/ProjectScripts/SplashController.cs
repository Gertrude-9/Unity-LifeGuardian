using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    public float waitTime = 3f;

    void Start()
    {
        Debug.Log("Splash started");
        Invoke("GoToMainHub", waitTime);
    }

    void GoToMainHub()
    {
        Debug.Log("Loading MainHub...");
        SceneManager.LoadScene("MainHubScene");
    }
}