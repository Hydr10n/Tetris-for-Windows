using Hydr10n.InputUtils;
using SharpDX.XInput;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tetris.Data;
using Tetris.Game;
using Tetris.Game.BasicDataTypes;

namespace Tetris.Pages
{
    public partial class GamePage : Page
    {
        private enum GameAction { NewGame, StopGame, MoveLeft, MoveRight, MoveDown, Drop, RotateClockwise, RotateCounterclockwise }

        private readonly ControllerUtil ControllerUtil = new ControllerUtil(UserIndex.One);
        private readonly GameViewModelEx GameViewModelEx;
        private readonly Manager Manager;

        public GamePage()
        {
            InitializeComponent();
            GameViewModelEx = DataContext as GameViewModelEx;
            Manager = new Manager(MainTetrisMatrix, PreviewTetrisMatrix, GameViewModelEx);
            Manager.LineClear += (sender, lineCount) =>
            {
                if (GameViewModelEx.IsGamepadActive)
                {
                    ushort speed = (ushort)(0xff00 * lineCount / 4.0);
                    ControllerUtil.Vibrate(new Vibration() { LeftMotorSpeed = speed, RightMotorSpeed = speed }, 300);
                }
            };
            ControllerUtil.Poll += ControllerUtil_Poll;
            ControllerUtil.ConnectionChanged += (sender, isConnected) => GameViewModelEx.IsGamepadActive = isConnected;
        }

        private void StartGame()
        {
            Manager.StartGame(false);
            Application.Current.MainWindow.PreviewKeyDown += MainWindow_PreviewKeyDown;
            ControllerUtil.EnablePolling = true;
            GameViewModelEx.IsGamepadActive = ControllerUtil.Controller.IsConnected;
        }

        private void PauseGame()
        {
            Manager.PauseGame();
            Application.Current.MainWindow.PreviewKeyDown -= MainWindow_PreviewKeyDown;
            ControllerUtil.EnablePolling = false;
        }

        private void ControlGame(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.NewGame: Manager.StartGame(true); break;
                case GameAction.StopGame: Manager.StopGame(); break;
                case GameAction.MoveLeft: Manager.MoveTetromino(Direction.Left); break;
                case GameAction.MoveRight: Manager.MoveTetromino(Direction.Right); break;
                case GameAction.MoveDown: Manager.MoveTetromino(Direction.Down); break;
                case GameAction.Drop: Manager.DropTetromino(); break;
                case GameAction.RotateClockwise: Manager.RotateTetromino(true); break;
                case GameAction.RotateCounterclockwise: Manager.RotateTetromino(false); break;
            }
        }

        private void Page_Loaded(object sender, EventArgs e)
        {
            StartGame();
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;
        }

        private void Page_Unloaded(object sender, EventArgs e)
        {
            PauseGame();
            Application.Current.MainWindow.StateChanged -= MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            switch ((sender as Window).WindowState)
            {
                case WindowState.Normal: StartGame(); break;
                case WindowState.Minimized: PauseGame(); break;
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e) => ControlGame(GameAction.NewGame);

        private void StopGame_Click(object sender, RoutedEventArgs e) => ControlGame(GameAction.StopGame);

        private void MoveLeft_Click(object sender, RoutedEventArgs e) => ControlGame(GameAction.MoveLeft);

        private void MoveRight_Click(object sender, RoutedEventArgs e) => ControlGame(GameAction.MoveRight);

        private void MoveDown_Click(object sender, RoutedEventArgs e) => ControlGame(GameAction.MoveDown);

        private void Drop_Click(object sender, RoutedEventArgs e) => ControlGame(GameAction.Drop);

        private void RotateCounterclockwise_Click(object sender, RoutedEventArgs e) => ControlGame(GameAction.RotateCounterclockwise);

        private void RotateClockwise_Click(object sender, RoutedEventArgs e) => ControlGame(GameAction.RotateClockwise);

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            GameViewModelEx.IsGamepadActive = false;
            GameAction gameAction;
            switch (e.Key)
            {
                case Key.Enter: gameAction = GameAction.NewGame; break;
                case Key.Escape: gameAction = GameAction.StopGame; break;
                case Key.Left: gameAction = GameAction.MoveLeft; break;
                case Key.Right: gameAction = GameAction.MoveRight; break;
                case Key.Down: gameAction = GameAction.MoveDown; break;
                case Key.Space: gameAction = GameAction.Drop; break;
                case Key.Z: gameAction = GameAction.RotateCounterclockwise; break;
                case Key.Up:
                case Key.X: gameAction = GameAction.RotateClockwise; break;
                default: return;
            }
            ControlGame(gameAction);
        }

        private void ControllerUtil_Poll(ControllerUtil sender)
        {
            if (sender.Controller.GetKeystroke(DeviceQueryType.Gamepad, out Keystroke keystroke).Failure || (keystroke.Flags & (KeyStrokeFlags.KeyDown | KeyStrokeFlags.Repeat)) == 0)
                return;
            GameViewModelEx.IsGamepadActive = true;
            GameAction gameAction;
            switch (keystroke.VirtualKey)
            {
                case GamepadKeyCode.Start: gameAction = GameAction.NewGame; break;
                case GamepadKeyCode.Back: gameAction = GameAction.StopGame; break;
                case GamepadKeyCode.DPadLeft: gameAction = GameAction.MoveLeft; break;
                case GamepadKeyCode.DPadRight: gameAction = GameAction.MoveRight; break;
                case GamepadKeyCode.DPadDown: gameAction = GameAction.MoveDown; break;
                case GamepadKeyCode.A: gameAction = GameAction.Drop; break;
                case GamepadKeyCode.X: gameAction = GameAction.RotateCounterclockwise; break;
                case GamepadKeyCode.DPadUp:
                case GamepadKeyCode.B: gameAction = GameAction.RotateClockwise; break;
                default: return;
            }
            Dispatcher.Invoke(() => ControlGame(gameAction));
        }
    }
}
