namespace TheAdventure.Models;

public interface IInteractable
{
    bool IsCollected { get; }
    void OnInteract(PlayerObject player);
}