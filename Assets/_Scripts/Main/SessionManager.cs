using System;
using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{

    public int PlayerCoins { private set; get; }
    public int HighScores { private set; get; }
    public int HammarCount { private set; get; }
    public int MultiplierCount { private set; get; }
    public bool SoundActive { private set; get; }
    public bool MusicActive { private set; get; }

    private string lastWatchedAdDate;
    private string lastBonusGameDate;


    private void Start()
    {
        PlayerCoins = PlayerPrefs.GetInt(CONSTANTS.PLAYER_COINS, 0);
        HighScores = PlayerPrefs.GetInt(CONSTANTS.HIGH_SCORES, 0);
        HammarCount = PlayerPrefs.GetInt(CONSTANTS.HAMMAR_POWERUPS, 0);
        MultiplierCount = PlayerPrefs.GetInt(CONSTANTS.MULTIPLIER_POWERUPS, 0);
        lastWatchedAdDate = PlayerPrefs.GetString(CONSTANTS.LAST_AD_WATCH_DATE, "");
        lastBonusGameDate = PlayerPrefs.GetString(CONSTANTS.LAST_BONUS_Game_DATE, "");
        SoundActive = PlayerPrefs.GetInt(CONSTANTS.SOUND, 0) == 1;
        MusicActive = PlayerPrefs.GetInt(CONSTANTS.MUSIC, 0) == 1;
    }

    public void AddCoins(int coins)
    {
        PlayerCoins += coins;
        SaveInt(CONSTANTS.PLAYER_COINS, PlayerCoins);
    }

    public void ModifyHammarPowerup(int count)
    {
        HammarCount += count;
        SaveInt(CONSTANTS.HAMMAR_POWERUPS, HammarCount);
    }

    public void ModifyMultiplierPowerup(int count)
    {
        MultiplierCount += count;
        SaveInt(CONSTANTS.MULTIPLIER_POWERUPS, MultiplierCount);
    }

    public void SaveNewHighScores(int newHighScores)
    {
        HighScores = newHighScores;
        SaveInt(CONSTANTS.HIGH_SCORES, newHighScores);
    }

    public bool IsAdAvailable()
    {
        if (!string.IsNullOrEmpty(lastWatchedAdDate))
        {
            if (DateTime.TryParse(lastWatchedAdDate, out DateTime lastWatchDate)) 
            {
                Debug.Log(lastWatchDate + " || " + DateTime.UtcNow + " :: " + (DateTime.UtcNow - lastWatchDate).TotalDays);
                return (DateTime.UtcNow - lastWatchDate).TotalDays >= 1;
            }
        }

        return true;
    }

    public bool BonusGameAvailable()
    {
        if (!string.IsNullOrEmpty(lastBonusGameDate))
        {
            if (DateTime.TryParse(lastBonusGameDate, out DateTime lastGameDate))
                return (DateTime.UtcNow - lastGameDate).TotalDays >= 1;
        }

        return true;
    }

    public void TodaysAdViewed()
    {
        lastWatchedAdDate = DateTime.UtcNow.ToString();
        SaveString(CONSTANTS.LAST_AD_WATCH_DATE, lastWatchedAdDate);
    }

    public void TodaysBonusGamePlayed()
    {
        lastBonusGameDate = DateTime.UtcNow.ToString();
        SaveString(CONSTANTS.LAST_BONUS_Game_DATE, lastBonusGameDate);
    }

    public void SetSound(bool val)
    {
        SoundActive = val;
        SaveInt(CONSTANTS.SOUND, val ? 1 : 0);
    }

    public void SetMusic(bool val)
    {
        MusicActive = val;
        SaveInt(CONSTANTS.MUSIC, val ? 1 : 0);
    }

    private void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    private void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

}

public class CONSTANTS
{
    public static readonly string HIGH_SCORES = "High Scores";
    public static readonly string PLAYER_COINS = "Coins";
    public static readonly string HAMMAR_POWERUPS = "Hammar Powerup";
    public static readonly string MULTIPLIER_POWERUPS = "Multiplier Powerup";
    public static readonly string LAST_AD_WATCH_DATE = "Last Ad Watch Date";
    public static readonly string LAST_BONUS_Game_DATE = "Last Bonus Game Date";
    public static readonly string SOUND = "Sound";
    public static readonly string MUSIC = "Music";
}