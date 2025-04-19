using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Android;
using UnityEngine.Events;

public class VoiceRecognitionManager : MonoBehaviour
{
    [Serializable]
    public class StringEvent : UnityEvent<string> { }

    public StringEvent OnVoiceRecognized = new StringEvent();
    public UnityEvent OnVoiceRecognitionStarted = new UnityEvent();
    public UnityEvent OnVoiceRecognitionEnded = new UnityEvent();
    public UnityEvent OnVoiceRecognitionFailed = new UnityEvent();

    [SerializeField] private bool autoRequestPermission = true;
    [SerializeField] private float maxRecordingTime = 10f; // Maximum recording time in seconds
    [SerializeField] private float voiceDetectionThreshold = 0.02f; // Mic sensitivity threshold
    [SerializeField] private GameObject microphonePermissionPrefab;

    private bool isListening = false;
    private AudioClip recordingClip;
    private string deviceMicrophone;
    private float[] samples = new float[128];
    private bool hasRecordedAudio = false;
    private Coroutine listeningCoroutine;

    private void Start()
    {
        // Check if we have microphone permission
        if (autoRequestPermission)
        {
            RequestMicrophonePermission();
        }

        // Get default microphone device
        if (Microphone.devices.Length > 0)
        {
            deviceMicrophone = Microphone.devices[0];
            Debug.Log($"Selected microphone: {deviceMicrophone}");
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
    }

    private void RequestMicrophonePermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            
            // Show UI explaining why we need microphone permission
            if (microphonePermissionPrefab != null)
            {
                Instantiate(microphonePermissionPrefab, transform);
            }
        }
#endif
    }

    public void StartListening()
    {
        if (isListening)
            return;

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found!");
            OnVoiceRecognitionFailed.Invoke();
            return;
        }

        // Check permission again
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            RequestMicrophonePermission();
            OnVoiceRecognitionFailed.Invoke();
            return;
        }
#endif

        isListening = true;
        hasRecordedAudio = false;
        
        // Start recording
        recordingClip = Microphone.Start(deviceMicrophone, false, Mathf.FloorToInt(maxRecordingTime), 44100);
        
        if (recordingClip == null)
        {
            Debug.LogError("Failed to start recording!");
            OnVoiceRecognitionFailed.Invoke();
            isListening = false;
            return;
        }
        
        OnVoiceRecognitionStarted.Invoke();
        
        // Start monitoring audio levels
        listeningCoroutine = StartCoroutine(MonitorAudioLevel());
        
        // Set a timeout in case user doesn't speak or stop manually
        StartCoroutine(ListeningTimeout());
    }

    private IEnumerator MonitorAudioLevel()
    {
        // Wait until microphone starts recording
        yield return new WaitWhile(() => Microphone.GetPosition(deviceMicrophone) <= 0);
        
        while (isListening)
        {
            // Get current microphone sample data
            Microphone.GetPosition(deviceMicrophone);
            recordingClip.GetData(samples, 0);
            
            // Calculate volume
            float sum = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                sum += samples[i] * samples[i]; // Square to make all values positive
            }
            float rmsValue = Mathf.Sqrt(sum / samples.Length);
            
            // Check if volume is above threshold (user is speaking)
            if (rmsValue > voiceDetectionThreshold)
            {
                hasRecordedAudio = true;
            }
            
            yield return null;
        }
    }

    private IEnumerator ListeningTimeout()
    {
        yield return new WaitForSeconds(maxRecordingTime);
        
        if (isListening)
        {
            StopListening();
        }
    }

    public void StopListening()
    {
        if (!isListening)
            return;

        // Stop the coroutine
        if (listeningCoroutine != null)
        {
            StopCoroutine(listeningCoroutine);
            listeningCoroutine = null;
        }

        isListening = false;
        
        // Get position to know how much was recorded
        int recordedLength = Microphone.GetPosition(deviceMicrophone);
        
        // Stop microphone
        Microphone.End(deviceMicrophone);
        
        OnVoiceRecognitionEnded.Invoke();
        
        if (!hasRecordedAudio || recordedLength <= 0)
        {
            Debug.Log("No audio recorded");
            OnVoiceRecognitionFailed.Invoke();
            return;
        }

        // Process the audio
        StartCoroutine(ProcessRecordedAudio(recordingClip, recordedLength));
    }

    private IEnumerator ProcessRecordedAudio(AudioClip clip, int recordedLength)
    {
        // Here, in a real implementation, we would:
        // 1. Convert the audio clip to the required format (WAV, MP3, etc.)
        // 2. Send it to a speech-to-text service (Google, Microsoft, etc.)
        // 3. Get the text result and pass it to the callback

        // For this example, we'll simulate this with a delay
        yield return new WaitForSeconds(1.0f);

        // In a real implementation, the response would come from a service
        string recognizedText = "This is a simulated voice recognition result. In a real implementation, this would be the text transcribed from speech.";
        
        // Invoke callback with the recognized text
        OnVoiceRecognized.Invoke(recognizedText);
    }

    // In a real implementation, you would add methods to:
    // 1. Convert AudioClip to bytes
    // 2. Send the audio data to a speech-to-text service API 
    // 3. Parse the response JSON
    // 4. Handle errors and retries
} 