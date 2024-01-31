using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{

    [Header("Unity Ads Initialization")]
    [SerializeField] private string _androidGameId;
    [SerializeField] private string _iOSGameId;
    [SerializeField] private bool _testMode = true;

    [Header("Unity Ads Ref")]
    [SerializeField] private RewardedAd rewardedAd;

    private string _gameId;
    private bool _isInitialized;
    public static AdsManager Instance;

    void Awake()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; 
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }

        if (!Instance)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
    }

    public void OnInitializationComplete()
    {
        _isInitialized = true;
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        _isInitialized = false;
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void DisplayRewardedAd(Action _completed = null, Action _failed = null)
    {
        if (!_isInitialized)
        {
            Debug.Log("Unity Ads Not Initialized Yet :(");
            _failed?.Invoke();
            return;
        }

        rewardedAd.DisplayAd(_completed, _failed);
    }

}