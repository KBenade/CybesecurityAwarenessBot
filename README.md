# Cybersecurity Awareness Bot

A Visual Studio C# console application that helps South African citizens practise safer online habits.

## Part 1 features

- Plays a WAV asset at launch (replace the included placeholder with your own recorded spoken greeting before submission).
- Displays a coloured ASCII cybersecurity banner.
- Asks for the user's name and personalises the greeting.
- Answers basic questions about password safety, phishing, and safe browsing.
- Validates empty and unsupported inputs with helpful feedback.
- Uses separate classes for UI, audio, profile data, and chatbot logic.

## Run in Visual Studio

1. Open `CybersecurityBot.sln` in **Visual Studio** (not Visual Studio Code).
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

## Submission checklist

- Replace `Assets/welcome.wav` with your recorded spoken greeting, then commit the project source, WAV asset, this README, and the GitHub Actions workflow.
- Make at least six meaningful commits, for example: project setup; banner; voice greeting; user interaction; chatbot responses; CI and documentation.
- Record an unlisted YouTube walkthrough explaining the code structure, logic, voice greeting, and formatting.
