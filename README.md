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

## Part 3 Task 1: task assistant

The **Tasks** button opens a task manager where you can add a title and description, optionally set a date and time reminder, view saved tasks, mark them complete or reopen them, and delete them. Tasks are stored in SQL Server LocalDB. The application creates the `CybersecurityBot` database and its `cybersecurity_tasks` table on first use.

## Part 3 Task 2: cybersecurity mini-game

Click **Quiz** in the chatbot to start a 12-question quiz. Questions cover phishing, passwords, safe browsing, multi-factor authentication, privacy, social engineering, public Wi-Fi, and device updates. Multiple-choice and true/false questions appear one at a time. Each answer receives immediate feedback with an explanation, followed by a final score and performance message. Choose **Play again** to restart.

## Part 3 Task 3: NLP simulation

The chat input uses basic NLP simulation with text normalization, keyword and phrase matching, and regular expressions for flexible task/reminder requests and reminder timeframes. Try `Remind me to update my password tomorrow`, `Add a task to enable 2FA`, `in 3 days`, `quiz me`, or `What have you done for me?`. Task requests are saved to SQL Server LocalDB, reminder replies update the saved task, quiz phrases open the mini-game, and recent chatbot actions are summarized in the conversation.

## Part 3 Task 4: activity log

Ask `show activity log`, `what have you done for me?`, or `recent actions` to see up to ten recent actions with timestamps. The log records chat-created tasks and reminders, task manager additions, completions, reopenings and deletions, quiz starts and completed scores, and recognized cybersecurity responses.

## Continuous integration

The GitHub Actions workflow in `.github/workflows/ci.yml` restores and builds the solution on every push and pull request. After pushing this repository to GitHub, the Actions tab will show each run.
- Successful CI Workflow Run
  <img width="1320" height="484" alt="Screenshot 2026-09-10 124318" src="https://github.com/user-attachments/assets/a9505703-30f6-417a-b755-e1783e986bba" />
  
## Submission checklist

- Replace `Assets/welcome.wav` with my own recorded spoken greeting, then commit the project source, WAV asset, this README, and the GitHub Actions workflow.
- Make at least six meaningful commits
- Record an unlisted YouTube walkthrough explaining the code structure, logic, voice greeting, and formatting.



