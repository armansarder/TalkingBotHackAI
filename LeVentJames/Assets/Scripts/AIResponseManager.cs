using UnityEngine;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.Collections.Generic;

public class AIResponseManager : MonoBehaviour
{
    [Header("OpenAI API Settings")]
    [SerializeField] private string apiKey = ""; // Set in Inspector or via secure storage
    [SerializeField] private string modelName = "gpt-3.5-turbo";
    [SerializeField] private int maxTokens = 150;
    [SerializeField] private float temperature = 0.7f;
    
    [Header("Offline Mode")]
    [SerializeField] private bool useOfflineMode = false;
    [SerializeField] private TextAsset offlineResponsesJson;
    [SerializeField, Range(0.5f, 3f)] private float simulatedResponseTime = 1.5f;
    
    [Header("Lebron Personality Settings")]
    [TextArea(3, 5)]
    [SerializeField] private string personalityPrompt = 
        "You are LeVent James, a supportive, wise, and unintentionally hilarious AI chatbot modeled after NBA legend LeBron James. " +
        "Your responses should include empathy, motivational 'dad wisdom,' and lighthearted basketball-themed humor. " +
        "You're like a mix between a life coach and a funny hype man. Keep responses concise (2-3 sentences) but impactful. " +
        "Use basketball metaphors when appropriate and occasionally refer to yourself in the third person. " +
        "Your goal is to help users vent their frustrations and find practical solutions to improve their mental wellness.";
    
    private const string API_URL = "https://api.openai.com/v1/chat/completions";
    private List<Dictionary<string, string>> conversationHistory = new List<Dictionary<string, string>>();
    private Dictionary<string, List<string>> offlineResponses;
    
    private void Start()
    {
        // Initialize conversation history with system prompt
        conversationHistory.Add(new Dictionary<string, string>
        {
            { "role", "system" },
            { "content", personalityPrompt }
        });
        
        // Parse offline responses if needed
        if (useOfflineMode && offlineResponsesJson != null)
        {
            ParseOfflineResponses();
        }
    }
    
    private void ParseOfflineResponses()
    {
        try
        {
            // In a real implementation, you would parse the JSON properly
            // This is just a placeholder
            offlineResponses = new Dictionary<string, List<string>>
            {
                { "greeting", new List<string> { 
                    "Hey champ! LeVent James here, ready to talk it out with you.",
                    "What's up? The King is in the building and ready to listen.",
                    "Yo! LeVent's got your back today. What's on your mind?"
                }},
                { "stress", new List<string> { 
                    "I feel you. Pressure is like a tough playoff game - you gotta take it one play at a time. Maybe try some deep breathing?",
                    "Even champions get stressed. LeVent recommends taking a mental timeout - 5 minutes of quiet can reset your game.",
                    "That's a lot on your plate! Remember, not every pass needs to be perfect. What's one small step you can take right now?"
                }},
                { "default", new List<string> { 
                    "I hear what you're saying. Sometimes the best offense is just taking care of your mental defense. What do you think might help?",
                    "LeVent's been there too. Small steps lead to big victories, on and off the court. Let's figure this out together.",
                    "That's real talk. Remember, it's not about never falling down - it's about how we get back up. What's your next move?"
                }}
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing offline responses: {e.Message}");
            useOfflineMode = false;
        }
    }
    
    public void GetLebronResponse(string userMessage, Action<string> callback)
    {
        // Add user message to conversation history
        conversationHistory.Add(new Dictionary<string, string>
        {
            { "role", "user" },
            { "content", userMessage }
        });
        
        if (useOfflineMode)
        {
            StartCoroutine(GetOfflineResponse(userMessage, callback));
        }
        else
        {
            StartCoroutine(GetAIResponse(callback));
        }
    }
    
    private IEnumerator GetOfflineResponse(string userMessage, Action<string> callback)
    {
        // Simulate network delay
        yield return new WaitForSeconds(simulatedResponseTime);
        
        // Determine which category to use based on keywords in user message
        string category = "default";
        userMessage = userMessage.ToLower();
        
        if (userMessage.Contains("hello") || userMessage.Contains("hi") || userMessage.Contains("hey"))
        {
            category = "greeting";
        }
        else if (userMessage.Contains("stress") || userMessage.Contains("anxious") || 
                userMessage.Contains("worried") || userMessage.Contains("pressure"))
        {
            category = "stress";
        }
        
        // Get random response from appropriate category
        List<string> responses = offlineResponses.ContainsKey(category) 
            ? offlineResponses[category] 
            : offlineResponses["default"];
            
        int randomIndex = UnityEngine.Random.Range(0, responses.Count);
        string response = responses[randomIndex];
        
        // Add response to conversation history
        conversationHistory.Add(new Dictionary<string, string>
        {
            { "role", "assistant" },
            { "content", response }
        });
        
        // Return the response
        callback?.Invoke(response);
    }
    
    private IEnumerator GetAIResponse(Action<string> callback)
    {
        // Create request data
        var requestData = new Dictionary<string, object>
        {
            { "model", modelName },
            { "messages", conversationHistory },
            { "max_tokens", maxTokens },
            { "temperature", temperature }
        };
        
        // Serialize to JSON
        string jsonData = JsonUtility.ToJson(requestData);
        
        // Create web request
        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // Set headers
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            
            // Send request
            yield return request.SendWebRequest();
            
            string response = "";
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Error: {request.error}");
                response = "Sorry, I'm having trouble connecting right now. Let's try again in a moment.";
            }
            else
            {
                // Parse response JSON
                try
                {
                    // In a real implementation, properly parse the OpenAI JSON response
                    // This is a simplified version
                    string jsonResponse = request.downloadHandler.text;
                    Debug.Log($"API Response: {jsonResponse}");
                    
                    // Extract the message content from the response
                    // In real implementation, use proper JSON parsing
                    response = "This is a placeholder for the real API response that would be parsed from JSON.";
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing response: {e.Message}");
                    response = "I didn't quite catch that. Can you try expressing that differently?";
                }
            }
            
            // Add assistant response to conversation history
            conversationHistory.Add(new Dictionary<string, string>
            {
                { "role", "assistant" },
                { "content", response }
            });
            
            // Trim conversation history if it gets too long
            if (conversationHistory.Count > 10)
            {
                // Keep system prompt and last 5 exchanges
                var newHistory = new List<Dictionary<string, string>>();
                newHistory.Add(conversationHistory[0]); // System prompt
                
                for (int i = conversationHistory.Count - 4; i < conversationHistory.Count; i++)
                {
                    newHistory.Add(conversationHistory[i]);
                }
                
                conversationHistory = newHistory;
            }
            
            // Return the response
            callback?.Invoke(response);
        }
    }
} 