using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.Audio;

public class OverlayUiManager : Singleton<OverlayUiManager>
{

    [field: SerializeField] public ShopUi ShopUi { private set; get; }
    [field: SerializeField] public BonusGameUi BonusGameUi { private set; get; }
    [field: SerializeField] public SettingsUI SettingsUI { private set; get; }
    [field: SerializeField] public InfoPopUp InfoPopUp { private set; get; }
    [field: SerializeField] public GameObject PreLoaderUi { private set; get; }

    private void Start()
    {
        LoadScene("Menu");
    }

    public void LoadScene(string _sceneName)
    {
        StartCoroutine(StartLoading(_sceneName));
    }

    private IEnumerator StartLoading(string _sceneName)
    {
        PreLoaderUi.SetActive(true);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneName);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        PreLoaderUi.SetActive(false);
    }

}