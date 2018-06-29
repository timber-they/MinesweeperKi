using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Minesweeper.Ki;

using static Minesweeper.Game.Coordinate;


// ReSharper disable UnusedMember.Global


namespace Minesweeper.Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private Timer        Timer { get; set; }
        public  ReferenceInt Time;

        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        private Solver Solver { get; set; }

        private bool Solving { get; set; }

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

        public MainWindow () => Init (10, 10);

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
                                       lock (Time)
                                       {
                                           if (Time % 2 == 0 && Solving)
                                               Solver.TakeAction ();
                                           if (Time.Value != -1)
                                               TimeLabel.Content = $"{Time * 20} ms";
                                       }
                                   });
                               }, null, 50,
                               10);
        }

        /// <inheritdoc />
        protected override void OnKeyUp (KeyEventArgs e)
        {
            base.OnKeyUp (e);
            switch (e.Key)
            {
                case Key.Enter:
                    Solver.TakeAction ();
                    break;
                case Key.Escape:
                    Solving = true;
                    break;
            }
        }

        public void SetSize (int x, int y)
        {
            SizeX = x;
            SizeY = y;

            Solver = new Solver (this);

            Time  = new ReferenceInt (0);
            Field = new Field (x, y, 0.85);

            Grid.Children.Clear ();

            Grid.ColumnDefinitions.Clear ();
            Grid.RowDefinitions.Clear ();

            for (var i = 0; i <= x; i++)
                Grid.ColumnDefinitions.Add (new ColumnDefinition {Width = new GridLength (100, GridUnitType.Star)});
            for (var i = 0; i <= y; i++)
                Grid.RowDefinitions.Add (new RowDefinition {Height = new GridLength (100, GridUnitType.Star)});

            for (var yI = 0; yI < y; yI++)
            {
                for (var xI = 0; xI < x; xI++)
                {
                    var button = new Button
                    {
                        BorderThickness = new Thickness (1),
                        BorderBrush     = Brushes.Gray,
                        Foreground      = Brushes.DarkGray,
                        Content         = "",
                        Background      = Brushes.DarkGray,
                        Tag             = C (xI, yI),
                        FontWeight      = FontWeights.Bold,
                        ToolTip         = $"{xI}, {yI}",
                        FontSize = 20
                    };

                    button.Click              += ButtonOnClick;
                    button.MouseRightButtonUp += ButtonOnMouseRightButtonUp;

                    Grid.Children.Add (button);
                    Grid.SetColumn (button, xI);
                    Grid.SetRow (button, yI);

                    if (yI != 0)
                        continue;

                    var labelX = new Label
                    {
                        BorderThickness = new Thickness (0),
                        Foreground      = Brushes.Black,
                        Content         = xI,
                        Background      = Brushes.White
                    };

                    Grid.Children.Add (labelX);
                    Grid.SetColumn (labelX, xI);
                    Grid.SetRow (labelX, y);
                }

                var labelY = new Label
                {
                    BorderThickness = new Thickness (0),
                    Foreground      = Brushes.Black,
                    Content         = yI,
                    Background      = Brushes.White
                };

                Grid.Children.Add (labelY);
                Grid.SetColumn (labelY, x);
                Grid.SetRow (labelY, yI);
            }

            FlagLabel.Content = $"{Field.GetRemainingFlagCount ()} flags";
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
            button.Content    = "💣";
            button.Foreground = Brushes.Black;

            InvalidateArrange ();
            InvalidateMeasure ();
            InvalidateVisual ();
        }

        public void SaveFlag (int x, int y)
        {
            var button = GetButton (x, y);

            button.Content    = "🏁";
            button.Foreground = Brushes.Red;
        }

        public void RemoveFlag (int x, int y)
        {
            var button = GetButton (x, y);

            button.Content    = "";
            button.Foreground = Brushes.DarkGray;
        }

        private Button GetButton (int x, int y) => Grid.Children.OfType <Button> ().
                                                        First (button => Grid.GetColumn (button) == x &&
                                                                         Grid.GetRow (button) == y);

        public void SetFlagCount (int count) => FlagLabel.Content = $"{count} flags";

        private void ButtonOnClick (object sender, RoutedEventArgs e) =>
            Solver.SetField ((Coordinate) ((Button) sender).Tag);

        private void ButtonOnMouseRightButtonUp (object sender, MouseButtonEventArgs e) =>
            Solver.SetFlag ((Coordinate) ((Button) sender).Tag);

        public IEnumerable <(Coordinate, LeftResult)> LeftClickOnField (int x, int y) => Field.OpenField (x, y, this);

        public RightResult RightClickOnField (int x, int y) => Field.SetFlag (x, y, this);
    }
}