# Cybersecurity Awareness Bot

A Visual Studio C# console application that helps South African citizens practise safer online habits.

## Part 1 features

- Plays a WAV asset at launch.
- Displays a coloured ASCII cybersecurity banner.
- Asks for the user's name and personalises the greeting.
- Answers basic questions about password safety, phishing, and safe browsing.
- Validates empty and unsupported inputs with helpful feedback.
- Uses separate classes for UI, audio, profile data, and chatbot logic.

## Part 2 features
- Random: The chatbot stores several responses for common cybersecurity topics and randomly selects one.
- Conversational flow: It remembers the current topic and handles follow-ups such as "tell me more", "give me another tip", and "I'm confused" without restarting the conversation.
- Memory and recall: It remembers the user's favorite cybersecurity topic when they express interest, then personalizes later responses about the topic.
- Sentiment detection: It recognizes simple sentiment words such as "worried", "curious", "frustrated" and "overwhelmed". it responds supportively and shares a relevant tip in the same reply.
- Error handling and code organization: It handles blank or null input with a helpful prompt and gives a rephrasing suggestion for unknown messages. Responses are organized using lists, arrays, and a response class reusable phrase lists are stored as static fields.

## Run in Visual Studio

1. Open `CybersecurityBot.sln` in **Visual Studio**.
2. Select the `CybersecurityBot` project as the startup project.
3. Press `Ctrl+F5` to run without the debugger, or `F5` to debug.

The project targets **.NET 10 for Windows** because it uses Windows' WAV playback support. Install the matching .NET workload if Visual Studio prompts for it.

## Conversation examples

Try these questions after entering your name:

- `How are you?`
- `What is your purpose?`
- `How do I make a safe password?`
- `How can I spot a phishing email?`
- `How can I browse safely?`
- `exit`

## Continuous integration

The GitHub Actions workflow in `.github/workflows/ci.yml` restores and builds the solution on every push and pull request. After pushing this repository to GitHub, the Actions tab will show each run.
- Successful CI Workflow Run
  <img width="1320" height="484" alt="Screenshot 2026-09-10 124318" src="https://github.com/user-attachments/assets/a9505703-30f6-417a-b755-e1783e986bba" />
  
## Submission checklist

- Replace `Assets/welcome.wav` with my own recorded spoken greeting, then commit the project source, WAV asset, this README, and the GitHub Actions workflow.
- Make at least six meaningful commits
- Record an unlisted YouTube walkthrough explaining the code structure, logic, voice greeting, and formatting.



