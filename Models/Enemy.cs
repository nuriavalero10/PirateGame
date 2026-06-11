namespace TheAdventure.Models;

public class Enemy : GameObject
{
    private PlayerObject _targetPlayer;
    private double _currentSpeed;

    public Enemy(int x, int y, PlayerObject player) : base(x, y, 20, 20)
    {
        _targetPlayer = player;

        // Velocidad aleatoria para que los enemigos no se amontonen
        Random rnd = new Random();
        _currentSpeed = rnd.Next(150, 170);
    }

    public override bool Update(double msSinceLastFrame)
    {
        // Conversión de velocidad
        int movimiento = (int)(_currentSpeed * (msSinceLastFrame / 1000.0));

        if (this.X < _targetPlayer.X) this.X += movimiento;
        else if (this.X > _targetPlayer.X) this.X -= movimiento;

        if (this.Y < _targetPlayer.Y) this.Y += movimiento;
        else if (this.Y > _targetPlayer.Y) this.Y -= movimiento;

        return base.Update(msSinceLastFrame);
    }
}