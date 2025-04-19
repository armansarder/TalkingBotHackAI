using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ChatManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField userInputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private Transform chatContentTransform;
    [SerializeField] private GameObject userMessagePrefab;
    [SerializeField] private GameObject lebronMessagePrefab;
    [SerializeField] private Button voiceInputButton;
    [SerializeField] private Image voiceInputIndicator;

    [Header("Lebron Settings")]
    [SerializeField] private AudioSource lebronAudioSource;
    [SerializeField] private Animator lebronAnimator;
    
    [Header("Dependencies")]
    [SerializeField] private VoiceRecognitionManager voiceManager;
    [SerializeField] private AIResponseManager aiManager;
    [SerializeField] private ProgressManager progressManager;

    private bool isListeningForVoice = false;
    private bool isProcessingResponse = false;

    private void Start()
    {
        // Initialize UI elements
        sendButton.onClick.AddListener(SendUserMessage);
        voiceInputButton.onClick.AddListener(ToggleVoiceInput);
        userInputField.onSubmit.AddListener(_ => SendUserMessage());
        
        // Welcome message
        StartCoroutine(SendLebronWelcomeMessage());
    }

    private IEnumerator SendLebronWelcomeMessage()
    {
        yield return new WaitForSeconds(0.5f);
        
        string welcomeMessage = "Hey there! I'm LeVent James, your personal mental wellness coach. " +
                               "Feel free to talk about whatever's on your mind. I'm here to listen and help you work through it. " +
                               "Remember, even MVPs need to take care of their mental game!";
                               
        CreateLebronMessageUI(welcomeMessage);
        progressManager.CheckFirstTimeUser();
    }

    public void SendUserMessage()
    {
        if (isProcessingResponse)
            return;

        string userMessage = userInputField.text.Trim();
        
        if (string.IsNullOrEmpty(userMessage))
            return;
            
        // Create UI message from user
        CreateUserMessageUI(userMessage);
        
        // Clear input field
        userInputField.text = "";
        
        // Process message and get AI response
        ProcessUserMessage(userMessage);
        
        // Update progress
        progressManager.LogChatInteraction();
    }
    
    public void ProcessVoiceInput(string voiceText)
    {
        if (!string.IsNullOrEmpty(voiceText))
        {
            userInputField.text = voiceText;
            SendUserMessage();
        }
        
        // Reset voice input UI
        isListeningForVoice = false;
        UpdateVoiceInputUI();
    }
    
    private void ToggleVoiceInput()
    {
        if (isListeningForVoice)
        {
            // Stop listening
            voiceManager.StopListening();
            isListeningForVoice = false;
        }
        else
        {
            // Start listening
            voiceManager.StartListening();
            isListeningForVoice = true;
        }
        
        UpdateVoiceInputUI();
    }
    
    private void UpdateVoiceInputUI()
    {
        // Update UI to show listening state
        voiceInputIndicator.color = isListeningForVoice ? Color.red : Color.white;
    }

    private void CreateUserMessageUI(string message)
    {
        GameObject messageObj = Instantiate(userMessagePrefab, chatContentTransform);
        TMP_Text messageText = messageObj.GetComponentInChildren<TMP_Text>();
        messageText.text = message;
        
        // Scroll to bottom
        StartCoroutine(ScrollToBottom());
    }

    private void CreateLebronMessageUI(string message)
    {
        GameObject messageObj = Instantiate(lebronMessagePrefab, chatContentTransform);
        TMP_Text messageText = messageObj.GetComponentInChildren<TMP_Text>();
        messageText.text = message;
        
        // Trigger Lebron animation
        if (lebronAnimator != null)
            lebronAnimator.SetTrigger("Talk");
            
        // Scroll to bottom
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        // Wait for end of frame to ensure UI has updated
        yield return new WaitForEndOfFrame();
        chatScrollRect.normalizedPosition = new Vector2(0, 0);
    }

    private void ProcessUserMessage(string message)
    {
        isProcessingResponse = true;
        
        // Show typing indicator
        StartCoroutine(ShowLebronIsTyping());
        
        // Send to AI service to get response
        aiManager.GetLebronResponse(message, OnResponseReceived);
    }
    
    private IEnumerator ShowLebronIsTyping()
    {
        // Create "Lebron is typing..." message
        GameObject typingObj = Instantiate(lebronMessagePrefab, chatContentTransform);
        TMP_Text typingText = typingObj.GetComponentInChildren<TMP_Text>();
        typingText.text = "Lebron is thinking...";
        
        yield return StartCoroutine(ScrollToBottom());
        
        // Store reference to remove later
        GameObject typingIndicator = typingObj;
        
        // Wait for real response
        yield return new WaitUntil(() => !isProcessingResponse);
        
        // Remove typing indicator
        if (typingIndicator != null)
            Destroy(typingIndicator);
    }

    private void OnResponseReceived(string response)
    {
        CreateLebronMessageUI(response);
        
        // Play audio if text-to-speech is enabled
        if (lebronAudioSource != null)
        {
            // This would be connected to a text-to-speech system
            // lebronAudioSource.clip = TextToSpeechManager.ConvertToSpeech(response);
            // lebronAudioSource.Play();
        }
        
        isProcessingResponse = false;
    }
} 