using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Timer Timer { get; set; }
        public  int   Time;

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

        public MainWindow ()
        {
            InitializeComponent ();

            SetSize (20, 20);
            Timer = new Timer (state =>
                               {
                                   if (Time == -1)
                                       return;
                                   Time++;
                                   TimeLabel.Dispatcher.Invoke (() => TimeLabel.Content = $"{Time} ms");
                               }, null, 1,
                               1);
        }

        public void SetSize (int x, int y)
        {
            Time  = 0;
            Field = new Field (x, y, 0.8);

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
                        FontWeight      = FontWeights.Bold
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
            var sender = GetButton (x, y);
            Debug.WriteLine ($"    saved empty {sender.Tag}");
            sender.Background = Brushes.SlateGray;
            sender.IsEnabled  = false;

            InvalidateArrange ();
            InvalidateMeasure ();
            InvalidateVisual ();
        }

        public void SaveClosing (int x, int y, int count)
        {
            var button = GetButton (x, y);
            Debug.WriteLine ($"    saved closing {button.Tag}");
            button.Background = Brushes.SlateGray;
            button.IsEnabled  = false;
            button.Content    = count;
            button.Foreground = NumberColors [count];

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

        public LeftResult LeftClickOnField (int x, int y) => Field.OpenField (x, y, this);

        public RightResult RightClickOnField (int x, int y) => Field.SetFlag (x, y, this);
    }
}