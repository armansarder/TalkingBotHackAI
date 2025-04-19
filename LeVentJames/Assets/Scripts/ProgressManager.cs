using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class ProgressManager : MonoBehaviour
{
    [Serializable]
    public class StreakEvent : UnityEvent<int> { }
    
    public StreakEvent OnStreakUpdated = new StreakEvent();
    public UnityEvent OnFirstTimeUser = new UnityEvent();
    public UnityEvent OnDailyCheckInComplete = new UnityEvent();
    
    [Header("Progress Settings")]
    [SerializeField] private int interactionsForDailyCheckIn = 5;
    [SerializeField] private int streakMilestone1 = 3;
    [SerializeField] private int streakMilestone2 = 7;
    [SerializeField] private int streakMilestone3 = 14;
    [SerializeField] private int streakMilestone4 = 30;
    
    [Header("UI References")]
    [SerializeField] private GameObject streakMilestonePopupPrefab;
    [SerializeField] private Transform popupParent;
    
    // Keys for PlayerPrefs
    private const string LAST_CHECK_IN_DATE_KEY = "LastCheckInDate";
    private const string CURRENT_STREAK_KEY = "CurrentStreak";
    private const string LONGEST_STREAK_KEY = "LongestStreak";
    private const string TOTAL_CHECK_INS_KEY = "TotalCheckIns";
    private const string TOTAL_INTERACTIONS_KEY = "TotalInteractions";
    private const string TODAY_INTERACTIONS_KEY = "TodayInteractions";
    private const string FIRST_TIME_KEY = "FirstTimeUser";
    
    private int todayInteractions = 0;
    private bool dailyCheckInComplete = false;
    private DateTime today;
    
    private void Start()
    {
        // Get today's date for tracking
        today = DateTime.Today;
        
        // Load today's interaction count
        LoadTodayInteractions();
        
        // Check if we need to reset daily counter
        CheckDateForReset();
    }
    
    public void CheckFirstTimeUser()
    {
        bool isFirstTime = !PlayerPrefs.HasKey(FIRST_TIME_KEY);
        
        if (isFirstTime)
        {
            // Mark as no longer first time
            PlayerPrefs.SetInt(FIRST_TIME_KEY, 1);
            PlayerPrefs.Save();
            
            // Trigger first time event
            OnFirstTimeUser.Invoke();
        }
    }
    
    private void LoadTodayInteractions()
    {
        // Check if we have an entry for today
        string todayString = today.ToString("yyyy-MM-dd");
        string savedDateString = PlayerPrefs.GetString("InteractionDate", "");
        
        if (savedDateString == todayString)
        {
            // Load today's count
            todayInteractions = PlayerPrefs.GetInt(TODAY_INTERACTIONS_KEY, 0);
            dailyCheckInComplete = todayInteractions >= interactionsForDailyCheckIn;
        }
        else
        {
            // New day, reset counter
            todayInteractions = 0;
            dailyCheckInComplete = false;
            
            // Save new date
            PlayerPrefs.SetString("InteractionDate", todayString);
            PlayerPrefs.SetInt(TODAY_INTERACTIONS_KEY, 0);
            PlayerPrefs.Save();
        }
    }
    
    private void CheckDateForReset()
    {
        // Check last check-in date
        string lastCheckInString = PlayerPrefs.GetString(LAST_CHECK_IN_DATE_KEY, "");
        
        if (!string.IsNullOrEmpty(lastCheckInString))
        {
            DateTime lastCheckIn = DateTime.Parse(lastCheckInString);
            TimeSpan timeSinceLastCheckIn = today - lastCheckIn;
            
            // If more than one day has passed, check if streak should be reset
            if (timeSinceLastCheckIn.TotalDays > 1)
            {
                // Streak is broken, reset to 0
                if (timeSinceLastCheckIn.TotalDays > 1.5) // Allow some buffer for timezone issues
                {
                    ResetStreak();
                }
            }
        }
    }
    
    public void LogChatInteraction()
    {
        // Increment total interactions
        int totalInteractions = PlayerPrefs.GetInt(TOTAL_INTERACTIONS_KEY, 0) + 1;
        PlayerPrefs.SetInt(TOTAL_INTERACTIONS_KEY, totalInteractions);
        
        // Increment today's interactions if not already completed check-in
        if (!dailyCheckInComplete)
        {
            todayInteractions++;
            PlayerPrefs.SetInt(TODAY_INTERACTIONS_KEY, todayInteractions);
            
            // Check if daily goal reached
            if (todayInteractions >= interactionsForDailyCheckIn)
            {
                CompleteCheckIn();
            }
        }
        
        PlayerPrefs.Save();
    }
    
    private void CompleteCheckIn()
    {
        dailyCheckInComplete = true;
        
        // Mark today as checked in
        PlayerPrefs.SetString(LAST_CHECK_IN_DATE_KEY, today.ToString("yyyy-MM-dd"));
        
        // Increment total check-ins
        int totalCheckIns = PlayerPrefs.GetInt(TOTAL_CHECK_INS_KEY, 0) + 1;
        PlayerPrefs.SetInt(TOTAL_CHECK_INS_KEY, totalCheckIns);
        
        // Update streak
        UpdateStreak();
        
        // Notify about completion
        OnDailyCheckInComplete.Invoke();
    }
    
    private void UpdateStreak()
    {
        // Increment streak
        int currentStreak = PlayerPrefs.GetInt(CURRENT_STREAK_KEY, 0) + 1;
        PlayerPrefs.SetInt(CURRENT_STREAK_KEY, currentStreak);
        
        // Update longest streak if needed
        int longestStreak = PlayerPrefs.GetInt(LONGEST_STREAK_KEY, 0);
        if (currentStreak > longestStreak)
        {
            PlayerPrefs.SetInt(LONGEST_STREAK_KEY, currentStreak);
        }
        
        PlayerPrefs.Save();
        
        // Notify about streak update
        OnStreakUpdated.Invoke(currentStreak);
        
        // Check for milestones
        CheckStreakMilestones(currentStreak);
    }
    
    private void ResetStreak()
    {
        // Reset streak to 0
        PlayerPrefs.SetInt(CURRENT_STREAK_KEY, 0);
        PlayerPrefs.Save();
        
        // Notify about streak reset
        OnStreakUpdated.Invoke(0);
    }
    
    private void CheckStreakMilestones(int streak)
    {
        // Check if we've hit any milestone
        if (streak == streakMilestone1 || 
            streak == streakMilestone2 || 
            streak == streakMilestone3 || 
            streak == streakMilestone4)
        {
            ShowStreakMilestonePopup(streak);
        }
    }
    
    private void ShowStreakMilestonePopup(int streak)
    {
        if (streakMilestonePopupPrefab != null && popupParent != null)
        {
            GameObject popup = Instantiate(streakMilestonePopupPrefab, popupParent);
            
            // If the popup has a StreakMilestonePopup component, configure it
            StreakMilestonePopup popupComponent = popup.GetComponent<StreakMilestonePopup>();
            if (popupComponent != null)
            {
                popupComponent.SetStreak(streak);
            }
        }
    }
    
    // Getters for UI display
    public int GetCurrentStreak()
    {
        return PlayerPrefs.GetInt(CURRENT_STREAK_KEY, 0);
    }
    
    public int GetLongestStreak()
    {
        return PlayerPrefs.GetInt(LONGEST_STREAK_KEY, 0);
    }
    
    public int GetTotalCheckIns()
    {
        return PlayerPrefs.GetInt(TOTAL_CHECK_INS_KEY, 0);
    }
    
    public int GetTodayInteractions()
    {
        return todayInteractions;
    }
    
    public int GetInteractionsRequired()
    {
        return interactionsForDailyCheckIn;
    }
    
    public bool IsDailyCheckInComplete()
    {
        return dailyCheckInComplete;
    }
}

// Helper class for milestone popup
public class StreakMilestonePopup : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI streakText;
    [SerializeField] private TMPro.TextMeshProUGUI messageText;
    
    public void SetStreak(int streak)
    {
        if (streakText != null)
        {
            streakText.text = streak.ToString() + " Day Streak!";
        }
        
        if (messageText != null)
        {
            // Customize message based on streak length
            string message = "Way to stay consistent! LeVent is proud of your dedication.";
            
            if (streak >= 30)
            {
                message = "CHAMPIONSHIP LEVEL COMMITMENT! You're building mental strength like a true MVP!";
            }
            else if (streak >= 14)
            {
                message = "That's All-Star dedication right there! Your mental game is getting stronger every day!";
            }
            else if (streak >= 7)
            {
                message = "One week strong! You're showing playoff-level mental toughness!";
            }
            
            messageText.text = message;
        }
    }
} 