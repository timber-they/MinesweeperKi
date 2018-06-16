namespace Minesweeper.Ki
{
    public static class Main
    {
        public static void Solve (MainWindow mainWindow)
        {
            var solver = new Solver (mainWindow);
            while (true)
                solver.TakeAction ();
        }
    }
}