namespace TheAdventure.Models;

public class Treasure : GameObject, IInteractable
{
    public bool IsCollected { get; private set; } = false;
    public int ScoreValue { get; } = 100;

    public Treasure(int x, int y) : base(x, y, 15, 15)
    {
    }

    public void OnInteract(PlayerObject player)
    {
        IsCollected = true;
    }

    public override bool Update(double msSinceLastFrame)
    {
        if (IsCollected)
        {
            return false;
        }

        return base.Update(msSinceLastFrame);
    }
}