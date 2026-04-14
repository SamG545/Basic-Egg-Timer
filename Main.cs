using Godot;
using System;

public partial class Main : Node2D
{
	private Label timerLabel;
	private Timer timer;
	private Button startButton, resetButton, closeButton, addMinuteButton, pauseButton, stopAlarmButton;
	private AudioStreamPlayer audioStreamPlayer;
	
	private int seconds;
	private int minutes;
	private int DSeconds = 0;
	private int DMinutes = 5;
	private bool extended = false;
	private bool startState = true;
	private bool _dragging = false;
	
	private Vector2I _dragStart;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetWindow().SetFlag(Window.Flags.Transparent, true);
		RenderingServer.SetDefaultClearColor(new Color(0, 0, 0, 0));
		
		CallDeferred(MethodName.GetNodesETC);
	}

	private void GetNodesETC()
	{
		timerLabel = GetNode<Label>("Label");
		timer = GetNode<Timer>("CountdownTimer");
		startButton = GetNode<Button>("StartButton");
		resetButton = GetNode<Button>("ResetButton");
		closeButton = GetNode<Button>("CloseButton");
		addMinuteButton = GetNode<Button>("AddMinuteButton");
		pauseButton = GetNode<Button>("PauseButton");
		stopAlarmButton = GetNode<Button>("StopAlarmButton");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		
		timer.Timeout += _OnTimerTimeout;
		startButton.Pressed += _OnStartButtonPressed;
		resetButton.Pressed += _OnResetButtonPressed;
		closeButton.Pressed += _OnCloseButtonPressed;
		addMinuteButton.Pressed += _OnAddMinuteButtonPressed;
		stopAlarmButton.Pressed += _OnStopAlarmButtonPressed;
		pauseButton.Pressed += _OnPauseButtonPressed;
		audioStreamPlayer.Finished += _AlarmFinished;
		
		startButton.MouseEntered += () => _OnButtonHover(startButton);
		startButton.MouseExited += () => _OnButtonUnhover(startButton);
		resetButton.MouseEntered += () => _OnButtonHover(resetButton);
		resetButton.MouseExited += () => _OnButtonUnhover(resetButton);
		addMinuteButton.MouseEntered += () => _OnButtonHover(addMinuteButton);
		addMinuteButton.MouseExited += () => _OnButtonUnhover(addMinuteButton);
		pauseButton.MouseEntered += () => _OnButtonHover(pauseButton);
		pauseButton.MouseExited += () => _OnButtonUnhover(pauseButton);
		closeButton.MouseEntered += () => _OnButtonHover(closeButton);
		closeButton.MouseExited += () => _OnButtonUnhover(closeButton);
		
		_ResetTimer();
		timerLabel.Text = $"{minutes}:{seconds:D2}";
		resetButton.Hide();
		stopAlarmButton.Hide();
		
		StartState();
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				_dragging = mouseButton.Pressed;
				_dragStart = DisplayServer.MouseGetPosition();
			}
		}
		if (@event is InputEventMouseMotion && _dragging)
		{
			Vector2I mousePos = DisplayServer.MouseGetPosition();
			Vector2I windowPos = DisplayServer.WindowGetPosition();
			DisplayServer.WindowSetPosition(windowPos + mousePos - _dragStart);
			_dragStart = mousePos;
		}

		if (@event.IsActionPressed("SubtractMinute"))
		{
			_SubtractMinute();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateLabel();
	}

	private void _OnTimerTimeout()
	{
		if (seconds == 0)
		{
			if (minutes > 0)
			{
				minutes--;
				seconds = 60;
			}
		}
		seconds--;
		timerLabel.Text = $"{minutes}:{seconds:D2}";
		
		if (minutes == 0 && seconds == 0)
		{
			timer.Stop();
			audioStreamPlayer.Play();
			stopAlarmButton.Show();
			StartState();
		}
	}

	private void _OnAddMinuteButtonPressed()
	{
		minutes++;
		timerLabel.Text = $"{minutes}:{seconds:D2}";
	}

	private void _OnResetButtonPressed()
	{
		timer.Stop();
		_ResetTimer();
		timerLabel.Text = $"{minutes}:{seconds:D2}";
		StartState();
	}

	private void _OnStartButtonPressed()
	{
		if (startState == true)
		{
			RunState();
		}
		else
		{
			pauseButton.Show();
			startButton.Hide();
		}
		timer.Start(1.0f);
		timerLabel.Text = $"{minutes}:{seconds:D2}";
	}

	private void _OnStopAlarmButtonPressed()
	{
		audioStreamPlayer.Stop();
		stopAlarmButton.Hide();
	}

	private void _ResetTimer()
	{
		seconds = DSeconds;
		minutes = DMinutes;
		timerLabel.Text = $"{minutes}:{seconds:D2}";
	}
	
	private void _OnCloseButtonPressed()
	{
		GetTree().Quit();
	}

	private void _OnPauseButtonPressed()
	{
		timer.Stop();
		startButton.Show();
		pauseButton.Hide();
	}

	private void _SubtractMinute()
	{
		if (minutes > 0)
		{
			minutes--;
			timerLabel.Text = $"{minutes}:{seconds:D2}";
		}
	}

	private void StartState()
	{
		resetButton.Hide();
		pauseButton.Hide();
		startButton.Show();
		addMinuteButton.Position = new Vector2(27, 34);
		startButton.Position = new Vector2(11, 34);
		startState = true;
	}

	private void RunState()
	{
		startButton.Hide();
		resetButton.Show();
		pauseButton.Show();
		startButton.Position = new Vector2(6, 34);
		addMinuteButton.Position = new Vector2(19, 34);
		startState = false;
	}
	
	private void UpdateLabel()
	{
		if (minutes >= 10 && extended == false)
		{
			timerLabel.Position = new Vector2(4, 5);
			extended = true;
		}
		else if (minutes < 10 && extended == true)
		{
			timerLabel.Position = new Vector2(9, 5);
			extended = false;
		}
	}

	private void _AlarmFinished()
	{
		stopAlarmButton.Hide();
	}
	
	private void _OnButtonHover(Button button)
	{
		Sprite2D sprite = button.GetNode<Sprite2D>("Sprite2D");
		sprite.Modulate = new Color(1.3f, 1.3f, 1.3f, 1);
	}

	private void _OnButtonUnhover(Button button)
	{
		Sprite2D sprite = button.GetNode<Sprite2D>("Sprite2D");
		sprite.Modulate = new Color(1, 1, 1, 1);
	}
}
