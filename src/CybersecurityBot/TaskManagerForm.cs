namespace CybersecurityBot;

internal sealed class TaskManagerForm : Form
{
    public event Action<string>? ActivityRecorded;

    private readonly SqlServerTaskRepository _repository;
    private readonly DataGridView _tasksGrid = new();
    private readonly TextBox _titleInput = new();
    private readonly TextBox _descriptionInput = new();
    private readonly CheckBox _reminderEnabled = new();
    private readonly DateTimePicker _reminderInput = new();
    private readonly Label _status = new();

    public TaskManagerForm()
    {
        Text = "Cybersecurity task assistant";
        MinimumSize = new Size(800, 590);
        Size = new Size(980, 700);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(14, 24, 35);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F);
        _repository = new SqlServerTaskRepository();

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(18),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 225));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        Controls.Add(root);

        root.Controls.Add(BuildEntryPanel(), 0, 0);
        ConfigureGrid();
        root.Controls.Add(_tasksGrid, 0, 1);
        _status.Dock = DockStyle.Fill;
        _status.ForeColor = Color.FromArgb(174, 230, 214);
        _status.TextAlign = ContentAlignment.MiddleLeft;
        root.Controls.Add(_status, 0, 2);

        try
        {
            _repository.EnsureCreated();
            RefreshTasks();
        }
        catch (Exception ex)
        {
            SetStatus(ex.Message, isError: true);
        }
    }

    private Control BuildEntryPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(14),
            BackColor = Color.FromArgb(20, 35, 49)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        panel.Controls.Add(CreateCaption("Task title"), 0, 0);
        ConfigureInput(_titleInput);
        panel.Controls.Add(_titleInput, 1, 0);

        panel.Controls.Add(CreateCaption("Description"), 0, 1);
        ConfigureInput(_descriptionInput);
        _descriptionInput.Multiline = true;
        _descriptionInput.ScrollBars = ScrollBars.Vertical;
        panel.Controls.Add(_descriptionInput, 1, 1);

        panel.Controls.Add(CreateCaption("Reminder"), 0, 2);
        var reminderRow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        _reminderEnabled.Text = "Set a reminder date and time";
        _reminderEnabled.ForeColor = Color.White;
        _reminderEnabled.AutoSize = true;
        _reminderInput.Format = DateTimePickerFormat.Custom;
        _reminderInput.CustomFormat = "dd MMM yyyy  HH:mm";
        _reminderInput.Width = 190;
        _reminderInput.MinDate = DateTime.Now;
        _reminderInput.Enabled = false;
        _reminderEnabled.CheckedChanged += (_, _) => _reminderInput.Enabled = _reminderEnabled.Checked;
        reminderRow.Controls.Add(_reminderEnabled);
        reminderRow.Controls.Add(_reminderInput);
        panel.Controls.Add(reminderRow, 1, 2);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var addButton = CreateButton("Add task", Color.FromArgb(79, 205, 159));
        addButton.Click += (_, _) => AddTask();
        actions.Controls.Add(addButton);
        var completeButton = CreateButton("Mark complete / reopen", Color.FromArgb(47, 86, 106));
        completeButton.Click += (_, _) => ToggleSelectedTask();
        actions.Controls.Add(completeButton);
        var deleteButton = CreateButton("Delete selected", Color.FromArgb(159, 67, 67));
        deleteButton.Click += (_, _) => DeleteSelectedTask();
        actions.Controls.Add(deleteButton);
        var refreshButton = CreateButton("Refresh", Color.FromArgb(47, 86, 106));
        refreshButton.Click += (_, _) => RefreshTasks();
        actions.Controls.Add(refreshButton);
        panel.Controls.Add(actions, 1, 3);
        return panel;
    }

    private void ConfigureGrid()
    {
        _tasksGrid.Dock = DockStyle.Fill;
        _tasksGrid.BackgroundColor = Color.FromArgb(20, 35, 49);
        _tasksGrid.ForeColor = Color.FromArgb(18, 30, 39);
        _tasksGrid.BorderStyle = BorderStyle.None;
        _tasksGrid.AllowUserToAddRows = false;
        _tasksGrid.AllowUserToDeleteRows = false;
        _tasksGrid.ReadOnly = true;
        _tasksGrid.MultiSelect = false;
        _tasksGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _tasksGrid.AutoGenerateColumns = false;
        _tasksGrid.RowHeadersVisible = false;
        _tasksGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Title", DataPropertyName = nameof(SecurityTask.Title), Width = 190 });
        _tasksGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Description", DataPropertyName = nameof(SecurityTask.Description), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        _tasksGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Reminder", DataPropertyName = nameof(SecurityTask.ReminderAt), Width = 160, DefaultCellStyle = { Format = "dd MMM yyyy HH:mm", NullValue = "None" } });
        _tasksGrid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Completed", DataPropertyName = nameof(SecurityTask.IsCompleted), Width = 85 });
        _tasksGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = nameof(SecurityTask.Id), Visible = false });
    }

    private void AddTask()
    {
        string title = _titleInput.Text.Trim();
        string description = _descriptionInput.Text.Trim();
        if (title.Length == 0 || description.Length == 0)
        {
            SetStatus("Enter both a task title and description.", isError: true);
            return;
        }

        if (title.Length > 200)
        {
            SetStatus("Task titles can be at most 200 characters.", isError: true);
            return;
        }

        if (_reminderEnabled.Checked && _reminderInput.Value <= DateTime.Now)
        {
            SetStatus("Choose a reminder time in the future.", isError: true);
            return;
        }

        RunDatabaseAction(() =>
        {
            _repository.Add(title, description, _reminderEnabled.Checked ? _reminderInput.Value : null);
            ActivityRecorded?.Invoke($"Task added: {title}.");
            if (_reminderEnabled.Checked)
                ActivityRecorded?.Invoke($"Reminder set for '{title}' on {_reminderInput.Value:dd MMM yyyy HH:mm}.");
            _titleInput.Clear();
            _descriptionInput.Clear();
            _reminderEnabled.Checked = false;
            RefreshTasks();
            SetStatus("Task saved to the database.");
        });
    }

    private void ToggleSelectedTask()
    {
        if (!TryGetSelectedTask(out SecurityTask? task)) return;
        RunDatabaseAction(() =>
        {
            _repository.SetCompleted(task.Id, !task.IsCompleted);
            ActivityRecorded?.Invoke(task.IsCompleted ? $"Task reopened: {task.Title}." : $"Task marked complete: {task.Title}.");
            RefreshTasks();
            SetStatus(task.IsCompleted ? "Task reopened." : "Task marked complete.");
        });
    }

    private void DeleteSelectedTask()
    {
        if (!TryGetSelectedTask(out SecurityTask? task)) return;
        RunDatabaseAction(() =>
        {
            _repository.Delete(task.Id);
            ActivityRecorded?.Invoke($"Task deleted: {task.Title}.");
            RefreshTasks();
            SetStatus("Task deleted.");
        });
    }

    private bool TryGetSelectedTask(out SecurityTask task)
    {
        if (_tasksGrid.CurrentRow?.DataBoundItem is SecurityTask selected)
        {
            task = selected;
            return true;
        }

        task = null!;
        SetStatus("Select a task first.", isError: true);
        return false;
    }

    private void RefreshTasks()
    {
        RunDatabaseAction(() =>
        {
            _tasksGrid.DataSource = _repository.GetAll().ToList();
            SetStatus($"{_tasksGrid.Rows.Count} task(s) loaded from SQL Server LocalDB.");
        });
    }

    private void RunDatabaseAction(Action action)
    {
        try { action(); }
        catch (Exception ex) { SetStatus(ex.Message, isError: true); }
    }

    private void SetStatus(string message, bool isError = false)
    {
        _status.Text = message;
        _status.ForeColor = isError ? Color.FromArgb(255, 165, 145) : Color.FromArgb(174, 230, 214);
    }

    private static Label CreateCaption(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        ForeColor = Color.White
    };

    private static void ConfigureInput(TextBox input)
    {
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 3, 0, 5);
        input.BackColor = Color.FromArgb(235, 242, 245);
        input.ForeColor = Color.FromArgb(18, 30, 39);
    }

    private static Button CreateButton(string text, Color color) => new()
    {
        Text = text,
        AutoSize = true,
        Height = 34,
        BackColor = color,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        Font = new Font("Segoe UI Semibold", 9.5F),
        Cursor = Cursors.Hand,
        Margin = new Padding(0, 2, 8, 2)
    };
}
