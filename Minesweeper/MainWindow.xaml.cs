using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Minesweeper.Ki;


namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Timer Timer { get; set; }
        public  int   Time;

        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        private Solver Solver { get; set; }

        private Dictionary <int, Brush> NumberColors { get; set; } = new Dictionary <int, Brush>
        {
            {1, Brushes.Blue},
            {2, Brushes.Green},
            {3, Brushes.Red},
            {4, Brushes.DarkBlue},
            {5, Brushes.Brown},
            {6, Brushes.DarkCyan},
            {7, Brushes.Black},
            {8, Brushes.Aqua}
        };

        private Field Field { get; set; }

        public MainWindow () => Init (20, 20);

        public MainWindow (int sizeX, int sizeY) => Init (sizeX, sizeY);

        private void Init (int sizeX, int sizeY)
        {
            InitializeComponent ();

            SetSize (sizeX, sizeY);
            Timer = new Timer (state =>
                               {
                                   if (Time == -1)
                                       return;
                                   Time++;
                                   TimeLabel.Dispatcher.Invoke (() =>
                                   {
                                       if (Time % 2 == 0)
                                            Solver.TakeAction ();
                                       return TimeLabel.Content = $"{Time * 50} ms";
                                   });
                               }, null, 50,
                               10);
        }

        /// <inheritdoc />
        protected override void OnKeyUp (KeyEventArgs e)
        {
            base.OnKeyUp (e);
            if (e.Key == Key.Enter)
                Solver.TakeAction ();
        }

        public void SetSize (int x, int y)
        {
            SizeX = x;
            SizeY = y;

            Solver = new Solver (this);

            Time  = 0;
            Field = new Field (x, y, 0.85);

            Grid.Children.Clear ();

            Grid.ColumnDefinitions.Clear ();
            Grid.RowDefinitions.Clear ();

            for (var i = 0; i < x; i++)
                Grid.ColumnDefinitions.Add (new ColumnDefinition {Width = new GridLength (100, GridUnitType.Star)});
            for (var i = 0; i < y; i++)
                Grid.RowDefinitions.Add (new RowDefinition {Height = new GridLength (100, GridUnitType.Star)});

            for (var yI = 0; yI < y; yI++)
                for (var xI = 0; xI < x; xI++)
                {
                    var button = new Button
                    {
                        BorderThickness = new Thickness (1),
                        BorderBrush     = Brushes.Gray,
                        Foreground      = Brushes.DarkGray,
                        Content         = "",
                        Background      = Brushes.DarkGray,
                        Tag             = (xI, yI),
                        FontWeight      = FontWeights.Bold,
                        ToolTip         = $"{xI}, {yI}"
                    };

                    button.Click              += ButtonOnClick;
                    button.MouseRightButtonUp += ButtonOnMouseRightButtonUp;

                    Grid.Children.Add (button);
                    Grid.SetColumn (button, xI);
                    Grid.SetRow (button, yI);
                }

            FlagLabel.Content = $"{Field.GetRemainingFlagCount ()} flags";
        }

        private void ButtonOnMouseRightButtonUp (object sender, MouseButtonEventArgs e)
        {
            var (x, y) = ((int, int)) ((Button) sender).Tag;
            RightClickOnField (x, y);
        }

        public void SaveEmpty (int x, int y)
        {
            var button = GetButton (x, y);
            button.Background = Brushes.SlateGray;
            button.IsEnabled  = false;

            InvalidateArrange ();
            InvalidateMeasure ();
            InvalidateVisual ();
        }

        public void SaveClosing (int x, int y, int count)
        {
            var button = GetButton (x, y);
            button.Background = Brushes.SlateGray;
            button.IsEnabled  = false;
            button.Content    = count;
            button.Foreground = NumberColors [count];

            InvalidateArrange ();
            InvalidateMeasure ();
            InvalidateVisual ();
        }

        public void SaveBomb (int x, int y)
        {
            var button = GetButton (x, y);
            button.Background = Brushes.Red;
            button.Content = "O";

            InvalidateArrange ();
            InvalidateMeasure ();
            InvalidateVisual ();
        }

        public void SaveFlag (int x, int y)
        {
            var button = GetButton (x, y);

            button.Content    = "I";
            button.Foreground = Brushes.Red;
        }

        public void RemoveFlag (int x, int y)
        {
            var button = GetButton (x, y);

            button.Content    = "";
            button.Foreground = Brushes.DarkGray;
        }

        private Button GetButton (int x, int y) => (Button) Grid.Children [x + y * Field.SizeX];

        public void SetFlagCount (int count) => FlagLabel.Content = $"{count} flags";

        private void ButtonOnClick (object sender, RoutedEventArgs e)
        {
            var (x, y) = ((int, int)) ((Button) sender).Tag;
            LeftClickOnField (x, y);
        }

        public List <((int, int), LeftResult)> LeftClickOnField (int x, int y) => Field.OpenField (x, y, this);

        public RightResult RightClickOnField (int x, int y) => Field.SetFlag (x, y, this);

        public int GetFlagCount () => Field.GetRemainingFlagCount ();
    }
}