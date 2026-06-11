using System.IO;
using System.Text.Json;

namespace TheAdventure.Models;

public record GameSave(int TotalWins);

public static class SaveManager
{
    private const string FilePath = "savegame.json";

    // AI-generated
    public static void SaveGame(GameSave save)
    {
        var json = JsonSerializer.Serialize(save);
        File.WriteAllText(FilePath, json);
    }

    public static GameSave LoadGame()
    {
        if (!File.Exists(FilePath))
        {
            return new GameSave(0);
        }

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<GameSave>(json) ?? new GameSave(0);
    }
    // end AI-generated
}