# LeVent James

A supportive, wise, and unintentionally hilarious AI chatbot modeled after NBA legend LeBron James. Designed to help users vent their frustrations and find solutions, the bot responds with empathy, motivational "dad wisdom," and lighthearted basketball-themed humor—like a mix between a life coach and your funniest hype man.

## Key Features

- **Voice Chat w/ LeBron**: User speaks freely, Lebron listens and generates thoughtful responses related to user input
- **Playbook Advice**: Offers step-by-step guides for decompressing from stress or anxiety, such as breathing exercises or journaling prompts
- **Progress Tracking**: Daily check-ins to earn rewards, celebrate streaks with Lebron, and gain insights on mental health journey

## Implementation

This Unity project implements the core functionality of LeVent James with the following components:

### 1. Chat System
- **ChatManager.cs**: Handles the chat UI, user input, and displaying Lebron's responses
- **AIResponseManager.cs**: Integrates with OpenAI API to generate Lebron's responses with basketball-themed wisdom and humor
- **VoiceRecognitionManager.cs**: Handles speech-to-text functionality for voice input

### 2. Playbook System
- **PlaybookManager.cs**: Manages a collection of mental wellness exercises and techniques
- **PlaybookEntry**: Data structure for exercises including titles, descriptions, and step-by-step instructions
- Categories include: Breathing Exercises, Mindfulness Activities, Journaling Prompts, Physical Exercises, and Mental Resets

### 3. Progress Tracking
- **ProgressManager.cs**: Handles user's progress, daily check-ins, and streak tracking
- Rewards system for consistent engagement
- Milestone celebrations at key streak points (3, 7, 14, 30 days)

### 4. UI and Navigation
- **MainController.cs**: Coordinates between systems and handles UI navigation
- Tab-based interface for Chat, Playbook, Progress, and Settings
- Adaptive suggestion system that recommends relevant exercises based on conversation

## Getting Started

1. **Open the project in Unity** (recommended version: 2020.3 or newer)
2. **Add your OpenAI API key** to the AIResponseManager component
3. **Build the app** for Android or iOS

## Usage

- Speak or type to interact with LeVent James
- Express your mental state or challenges
- Receive empathetic, motivational responses with basketball metaphors
- Access the Playbook for guided mental wellness exercises
- Track your progress through consistent daily check-ins

## Future Enhancements

- Advanced animation system for Lebron's character
- Expanded playbook with more categories
- Voice output using text-to-speech
- Enhanced metrics and visualization of mental wellness journey
- Community features for shared goals and achievements
