using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private VoiceRecognitionManager voiceManager;
    [SerializeField] private AIResponseManager aiManager;
    [SerializeField] private ProgressManager progressManager;
    [SerializeField] private PlaybookManager playbookManager;
    
    [Header("UI Navigation")]
    [SerializeField] private Button chatTabButton;
    [SerializeField] private Button playbookTabButton;
    [SerializeField] private Button progressTabButton;
    [SerializeField] private Button settingsButton;
    
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private GameObject playbookPanel;
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Playbook Suggestion")]
    [SerializeField] private Button suggestPlaybookButton;
    [SerializeField] private GameObject playbookSuggestionPanel;
    [SerializeField] private TMPro.TextMeshProUGUI suggestionTitleText;
    [SerializeField] private TMPro.TextMeshProUGUI suggestionDescriptionText;
    [SerializeField] private Button viewDetailButton;
    [SerializeField] private Button dismissSuggestionButton;
    
    private PlaybookManager.PlaybookEntry currentSuggestion;
    
    private void Start()
    {
        // Set up tab navigation
        chatTabButton.onClick.AddListener(() => ShowTab(TabType.Chat));
        playbookTabButton.onClick.AddListener(() => ShowTab(TabType.Playbook));
        progressTabButton.onClick.AddListener(() => ShowTab(TabType.Progress));
        settingsButton.onClick.AddListener(() => ShowTab(TabType.Settings));
        
        // Set up playbook suggestion
        if (suggestPlaybookButton != null)
            suggestPlaybookButton.onClick.AddListener(SuggestPlaybook);
            
        if (viewDetailButton != null)
            viewDetailButton.onClick.AddListener(ViewSuggestionDetail);
            
        if (dismissSuggestionButton != null)
            dismissSuggestionButton.onClick.AddListener(() => ShowSuggestionPanel(false));
        
        // Set up voice recognition callbacks
        if (voiceManager != null)
        {
            voiceManager.OnVoiceRecognized.AddListener(OnVoiceRecognized);
        }
        
        // Set up progress callbacks
        if (progressManager != null)
        {
            progressManager.OnDailyCheckInComplete.AddListener(OnDailyCheckInComplete);
            progressManager.OnStreakUpdated.AddListener(OnStreakUpdated);
        }
        
        // Default to Chat tab
        ShowTab(TabType.Chat);
        
        // Hide suggestion panel initially
        ShowSuggestionPanel(false);
    }
    
    private void OnVoiceRecognized(string recognizedText)
    {
        if (chatManager != null)
        {
            chatManager.ProcessVoiceInput(recognizedText);
        }
    }
    
    private void OnDailyCheckInComplete()
    {
        // Show congratulations or rewards
        Debug.Log("Daily check-in complete!");
        
        // Suggest a playbook entry as a reward
        SuggestPlaybook();
    }
    
    private void OnStreakUpdated(int streak)
    {
        Debug.Log($"Streak updated: {streak} days");
        
        // Could trigger streak milestone celebrations here
    }
    
    private void SuggestPlaybook()
    {
        if (playbookManager == null)
            return;
            
        // Get the last user message from chat history to contextualize suggestion
        // For demo, we'll just use a default entry
        currentSuggestion = playbookManager.SuggestPlaybookEntry("stress anxiety");
        
        if (currentSuggestion != null)
            ShowSuggestion(currentSuggestion);
    }
    
    private void ShowSuggestion(PlaybookManager.PlaybookEntry entry)
    {
        if (suggestionTitleText != null)
            suggestionTitleText.text = entry.title;
            
        if (suggestionDescriptionText != null)
            suggestionDescriptionText.text = entry.description;
            
        ShowSuggestionPanel(true);
    }
    
    private void ViewSuggestionDetail()
    {
        // Hide suggestion panel
        ShowSuggestionPanel(false);
        
        // Switch to playbook tab
        ShowTab(TabType.Playbook);
        
        // Show the playbook entry details
        // This would require a public method in PlaybookManager to show a specific entry
        // For now, we just show the playbook panel
        if (playbookManager != null)
            playbookManager.ShowPlaybookPanel(true);
    }
    
    private void ShowSuggestionPanel(bool show)
    {
        if (playbookSuggestionPanel != null)
            playbookSuggestionPanel.SetActive(show);
    }
    
    private enum TabType
    {
        Chat,
        Playbook,
        Progress,
        Settings
    }
    
    private void ShowTab(TabType tab)
    {
        // Hide all panels first
        chatPanel.SetActive(false);
        playbookPanel.SetActive(false);
        progressPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        // Show selected panel
        switch (tab)
        {
            case TabType.Chat:
                chatPanel.SetActive(true);
                break;
            case TabType.Playbook:
                playbookPanel.SetActive(true);
                break;
            case TabType.Progress:
                progressPanel.SetActive(true);
                break;
            case TabType.Settings:
                settingsPanel.SetActive(true);
                break;
        }
        
        // Update button states
        UpdateTabButtonStates(tab);
    }
    
    private void UpdateTabButtonStates(TabType activeTab)
    {
        // Visual feedback for active tab (e.g., color change)
        // This is just a placeholder - implement according to your UI design
        Color activeColor = new Color(0.9f, 0.9f, 0.9f);
        Color inactiveColor = new Color(0.7f, 0.7f, 0.7f);
        
        chatTabButton.GetComponent<Image>().color = (activeTab == TabType.Chat) ? activeColor : inactiveColor;
        playbookTabButton.GetComponent<Image>().color = (activeTab == TabType.Playbook) ? activeColor : inactiveColor;
        progressTabButton.GetComponent<Image>().color = (activeTab == TabType.Progress) ? activeColor : inactiveColor;
        settingsButton.GetComponent<Image>().color = (activeTab == TabType.Settings) ? activeColor : inactiveColor;
    }
} 