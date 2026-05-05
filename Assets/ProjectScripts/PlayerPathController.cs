using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChoosePathPlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Animator remyAnimator;
    public string walkParameter = "isRunning";

    private bool pathChosen = false;

    private const string PREVENTION_SCENE = "GrooveofAwareness";
    private const string MANAGEMENT_SCENE = "ManagementLevel";

    void Update()
    {
        if (pathChosen) return;

        float v = Input.GetAxisRaw("Vertical");

        if (v != 0)
        {
            transform.position += transform.forward * v * moveSpeed * Time.deltaTime;

            if (remyAnimator != null)
                remyAnimator.SetBool(walkParameter, true);
        }
        else
        {
            if (remyAnimator != null)
                remyAnimator.SetBool(walkParameter, false);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChoosePath("Prevention", PREVENTION_SCENE, -90f);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChoosePath("Management", MANAGEMENT_SCENE, 90f);
        }
    }

    void ChoosePath(string pathName, string sceneName, float turnAngle)
    {
        pathChosen = true;

        PlayerPrefs.SetString("SelectedPath", pathName);
        PlayerPrefs.Save();

        Debug.Log("Chosen Path: " + pathName);
        Debug.Log("Scene to load: " + sceneName);

        StartCoroutine(TurnThenLoad(sceneName, turnAngle));
    }

    IEnumerator TurnThenLoad(string sceneName, float turnAngle)
    {
        if (remyAnimator != null)
            remyAnimator.SetBool(walkParameter, true);

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0f, turnAngle, 0f);

        float timer = 0f;
        float duration = 0.6f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, targetRot, timer / duration);
            yield return null;
        }

        transform.rotation = targetRot;

        yield return new WaitForSeconds(0.5f);

        Debug.Log("NOW LOADING EXACT SCENE: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}