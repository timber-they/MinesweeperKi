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
            var won = Field.SetFlag (x, y);
            switch (won)
            {
                case 0:
                    ((Button) sender).Content    = "I";
                    ((Button) sender).Foreground = Brushes.Red;
                    break;
                case -1:
                    ((Button) sender).Content    = "";
                    ((Button) sender).Foreground = Brushes.DarkGray;
                    break;
                case 1:
                    Time = -1;
                    MessageBox.Show ("You won.");
                    SetSize (Field.SizeX, Field.SizeY);
                    break;
                case -2:
                case -3:
                    break;
            }

            FlagLabel.Content = $"{Field.GetRemainingFlagCount ()} flags";
        }

        public void SaveEmpty (int x, int y)
        {
            var sender = (Button) Grid.Children [x + y * Field.SizeX];
            Debug.WriteLine ($"    saved empty {sender.Tag}");
            sender.Background = Brushes.SlateGray;
            sender.IsEnabled  = false;

            InvalidateArrange ();
            InvalidateMeasure ();
            InvalidateVisual ();
        }

        public void SaveClosing (int x, int y, int count)
        {
            var button = (Button) Grid.Children [x + y * Field.SizeX];
            Debug.WriteLine ($"    saved closing {button.Tag}");
            button.Background = Brushes.SlateGray;
            button.IsEnabled  = false;
            button.Content    = count;
            button.Foreground = NumberColors [count];

            InvalidateArrange ();
            InvalidateMeasure ();
            InvalidateVisual ();
        }

        private void ButtonOnClick (object sender, RoutedEventArgs e)
        {
            var (x, y) = ((int, int)) ((Button) sender).Tag;
            var result = Field.OpenField (x, y, this);
            Debug.WriteLine ($"----Score: {result}");
        }
    }
}