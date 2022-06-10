namespace Helpers
{
    [System.Flags]
    public enum TouchType
    {
        None = 0,

        Right   = 1 << 0,
        Top     = 1 << 1,
        Left    = 1 << 2,
        Bottom  = 1 << 3,

        LeftRight = Left | Right,
        TopBottom = Top | Bottom
    };
}