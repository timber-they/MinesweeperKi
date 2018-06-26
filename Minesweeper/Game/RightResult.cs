namespace Minesweeper.Game
{
    public enum RightResult
    {
        Won = 1,
        AlreadyFlagged = -1,
        AlreadyOpened = -2,
        NoFlagsLeft = -3,
        Valid = 0
    }
}