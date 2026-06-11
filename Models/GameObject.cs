namespace TheAdventure.Models;

public abstract class GameObject
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    protected GameObject(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public virtual bool Update(double msSinceLastFrame)
    {
        return true;
    }
}