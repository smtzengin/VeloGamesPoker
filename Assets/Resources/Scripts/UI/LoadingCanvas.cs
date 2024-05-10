using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Resources.Scripts.Utility;

public class LoadingCanvas : MonoBehaviour
{
    public static LoadingCanvas Instance => Singleton<LoadingCanvas>.Instance;

    [SerializeField] private TextMeshProUGUI _loadingText;
    private string[] loadingPhrases = { "Loading.", "Loading..", "Loading..." };
    private float switchDuration = 0.5f;

    public IEnumerator LoadNewScene(string name)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);

        StartCoroutine(ShowLoadingTextEffect());
        yield return new WaitForSeconds(2); 
        SceneManager.LoadScene(name);
        StopCoroutine(ShowLoadingTextEffect());
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private IEnumerator ShowLoadingTextEffect()
    {
        int phraseIndex = 0;
        while (true)
        {
            _loadingText.text = loadingPhrases[phraseIndex];
            phraseIndex = (phraseIndex + 1) % loadingPhrases.Length;
            yield return new WaitForSeconds(switchDuration);
        }
    }
}
