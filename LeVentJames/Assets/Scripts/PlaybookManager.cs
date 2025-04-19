using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PlaybookManager : MonoBehaviour
{
    [Serializable]
    public class PlaybookEntry
    {
        public string title;
        [TextArea(3, 5)]
        public string description;
        public string[] steps;
        public string category;
        public Sprite icon;
    }
    
    [Header("Playbook Entries")]
    [SerializeField] private List<PlaybookEntry> breathingExercises;
    [SerializeField] private List<PlaybookEntry> mindfulnessActivities;
    [SerializeField] private List<PlaybookEntry> journalingPrompts;
    [SerializeField] private List<PlaybookEntry> physicalExercises;
    [SerializeField] private List<PlaybookEntry> mentalResets;
    
    [Header("UI References")]
    [SerializeField] private GameObject playbookPanel;
    [SerializeField] private Transform categoryButtonsContainer;
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private Transform playbookEntriesContainer;
    [SerializeField] private GameObject playbookEntryPrefab;
    [SerializeField] private GameObject playbookDetailPanel;
    [SerializeField] private TextMeshProUGUI detailTitleText;
    [SerializeField] private TextMeshProUGUI detailDescriptionText;
    [SerializeField] private Transform detailStepsContainer;
    [SerializeField] private GameObject detailStepPrefab;
    [SerializeField] private Image detailIconImage;
    [SerializeField] private Button detailCloseButton;
    [SerializeField] private Button playbookCloseButton;
    
    private Dictionary<string, List<PlaybookEntry>> categorizedPlaybooks;
    private List<GameObject> instantiatedEntries = new List<GameObject>();
    private List<GameObject> instantiatedSteps = new List<GameObject>();
    private string currentCategory = "";
    
    private void Start()
    {
        // Set up data
        SetupCategorizedPlaybooks();
        
        // Set up UI
        SetupCategoryButtons();
        
        // Set up button listeners
        if (detailCloseButton != null)
            detailCloseButton.onClick.AddListener(() => ShowDetailPanel(false));
            
        if (playbookCloseButton != null)
            playbookCloseButton.onClick.AddListener(() => ShowPlaybookPanel(false));
            
        // Hide panels initially
        ShowDetailPanel(false);
        ShowPlaybookPanel(false);
    }
    
    private void SetupCategorizedPlaybooks()
    {
        categorizedPlaybooks = new Dictionary<string, List<PlaybookEntry>>
        {
            { "Breathing", breathingExercises },
            { "Mindfulness", mindfulnessActivities },
            { "Journaling", journalingPrompts },
            { "Physical", physicalExercises },
            { "Mental Reset", mentalResets }
        };
    }
    
    private void SetupCategoryButtons()
    {
        if (categoryButtonsContainer == null || categoryButtonPrefab == null)
            return;
            
        // Clear any existing buttons
        foreach (Transform child in categoryButtonsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Add a button for each category
        foreach (var category in categorizedPlaybooks.Keys)
        {
            GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryButtonsContainer);
            
            // Set button text
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = category;
            }
            
            // Set button action
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                string categoryName = category; // Create a local copy for the lambda
                button.onClick.AddListener(() => ShowCategory(categoryName));
            }
        }
    }
    
    public void ShowPlaybookPanel(bool show)
    {
        if (playbookPanel != null)
        {
            playbookPanel.SetActive(show);
            
            // If showing the panel, show the first category by default
            if (show && categorizedPlaybooks.Count > 0)
            {
                string firstCategory = categorizedPlaybooks.Keys.First();
                ShowCategory(firstCategory);
            }
        }
    }
    
    private void ShowCategory(string category)
    {
        currentCategory = category;
        
        // Clear existing entries
        ClearEntries();
        
        // Get entries for the category
        List<PlaybookEntry> entries = categorizedPlaybooks[category];
        
        // Populate UI with entries
        foreach (PlaybookEntry entry in entries)
        {
            CreateEntryUI(entry);
        }
    }
    
    private void CreateEntryUI(PlaybookEntry entry)
    {
        if (playbookEntriesContainer == null || playbookEntryPrefab == null)
            return;
            
        GameObject entryObj = Instantiate(playbookEntryPrefab, playbookEntriesContainer);
        instantiatedEntries.Add(entryObj);
        
        // Set title
        TextMeshProUGUI titleText = entryObj.GetComponentInChildren<TextMeshProUGUI>();
        if (titleText != null)
        {
            titleText.text = entry.title;
        }
        
        // Set icon if available
        Image iconImage = entryObj.GetComponentInChildren<Image>();
        if (iconImage != null && entry.icon != null)
        {
            iconImage.sprite = entry.icon;
        }
        
        // Set button action
        Button button = entryObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ShowEntryDetails(entry));
        }
    }
    
    private void ClearEntries()
    {
        foreach (GameObject entry in instantiatedEntries)
        {
            Destroy(entry);
        }
        
        instantiatedEntries.Clear();
    }
    
    private void ShowEntryDetails(PlaybookEntry entry)
    {
        // Set detail panel title and description
        if (detailTitleText != null)
            detailTitleText.text = entry.title;
            
        if (detailDescriptionText != null)
            detailDescriptionText.text = entry.description;
            
        // Set icon if available
        if (detailIconImage != null && entry.icon != null)
            detailIconImage.sprite = entry.icon;
            
        // Clear any existing steps
        ClearSteps();
        
        // Create step items
        if (detailStepsContainer != null && detailStepPrefab != null)
        {
            for (int i = 0; i < entry.steps.Length; i++)
            {
                GameObject stepObj = Instantiate(detailStepPrefab, detailStepsContainer);
                instantiatedSteps.Add(stepObj);
                
                // Set step number and text
                TextMeshProUGUI stepText = stepObj.GetComponentInChildren<TextMeshProUGUI>();
                if (stepText != null)
                {
                    stepText.text = $"{i + 1}. {entry.steps[i]}";
                }
            }
        }
        
        // Show the detail panel
        ShowDetailPanel(true);
    }
    
    private void ClearSteps()
    {
        foreach (GameObject step in instantiatedSteps)
        {
            Destroy(step);
        }
        
        instantiatedSteps.Clear();
    }
    
    private void ShowDetailPanel(bool show)
    {
        if (playbookDetailPanel != null)
            playbookDetailPanel.SetActive(show);
    }
    
    // Method to suggest playbook entry based on user's message
    public PlaybookEntry SuggestPlaybookEntry(string userMessage)
    {
        // Convert to lowercase for easier comparison
        string message = userMessage.ToLower();
        
        // Check for breathing-related keywords
        if (ContainsAny(message, new[] { "breath", "anxious", "panic", "stress", "calm", "relax" }))
        {
            return GetRandomEntry("Breathing");
        }
        // Check for mindfulness-related keywords
        else if (ContainsAny(message, new[] { "focus", "present", "mindful", "attention", "distract", "concentrate" }))
        {
            return GetRandomEntry("Mindfulness");
        }
        // Check for journaling-related keywords
        else if (ContainsAny(message, new[] { "write", "journal", "express", "emotions", "feelings", "thoughts" }))
        {
            return GetRandomEntry("Journaling");
        }
        // Check for physical exercise-related keywords
        else if (ContainsAny(message, new[] { "active", "exercise", "move", "energy", "tired", "physical" }))
        {
            return GetRandomEntry("Physical");
        }
        // Default to mental reset
        else
        {
            return GetRandomEntry("Mental Reset");
        }
    }
    
    private bool ContainsAny(string source, string[] keywords)
    {
        foreach (string keyword in keywords)
        {
            if (source.Contains(keyword))
                return true;
        }
        
        return false;
    }
    
    private PlaybookEntry GetRandomEntry(string category)
    {
        if (categorizedPlaybooks.ContainsKey(category) && categorizedPlaybooks[category].Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, categorizedPlaybooks[category].Count);
            return categorizedPlaybooks[category][randomIndex];
        }
        
        // Fallback to first entry in first category
        foreach (var entries in categorizedPlaybooks.Values)
        {
            if (entries.Count > 0)
                return entries[0];
        }
        
        return null;
    }
} 