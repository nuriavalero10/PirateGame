namespace TheAdventure.Models;

public class PlayerObject : GameObject
{
    private const int Speed = 200;

    public PlayerObject(int x, int y) : base(x, y, 20, 20)
    {
    }

    // Lógica de movimiento (Lab 6)
    public void UpdatePosition(double up, double down, double left, double right, double time)
    {
        var pixelsToMove = Speed * (time / 1000.0);

        Y -= (int)(pixelsToMove * up);
        Y += (int)(pixelsToMove * down);
        X -= (int)(pixelsToMove * left);
        X += (int)(pixelsToMove * right);
    }
}