using Silk.NET.Maths;
using Silk.NET.SDL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TheAdventure.Models;

namespace TheAdventure;

public static class Program
{
    public static void Main()
    {
        var sdl = new Sdl(new SdlContext());

        UInt64 framesRenderedCounter = 0;
        var timer = new Stopwatch();
        timer.Start();

        ReadOnlySpan<byte> keyboardState;
        unsafe
        {
            keyboardState = new(sdl.GetKeyboardState(null), (int)KeyCode.Count);
        }

        Span<byte> mouseButtonStates = stackalloc byte[(int)MouseButton.Count];
        var ev = new Event();

        var sdlInitResult = sdl.Init(Sdl.InitVideo | Sdl.InitAudio | Sdl.InitEvents | Sdl.InitTimer | Sdl.InitGamecontroller | Sdl.InitJoystick);
        if (sdlInitResult < 0) throw new InvalidOperationException("Failed to initialize SDL.");

        IntPtr window;
        unsafe
        {
            window = (IntPtr)sdl.CreateWindow("The Adventure", Sdl.WindowposUndefined, Sdl.WindowposUndefined, 800, 800, (uint)WindowFlags.Resizable | (uint)WindowFlags.AllowHighdpi);
        }

        IntPtr renderer;
        unsafe
        {
            renderer = (IntPtr)sdl.CreateRenderer((Window*)window, -1, (uint)RendererFlags.Accelerated);
            sdl.RenderSetVSync((Renderer*)renderer, 1);
        }

        // Entidades iniciales
        var player = new PlayerObject(400, 400);
        var gameObjects = new List<GameObject>
        {
            new Treasure(200, 200), new Treasure(600, 200),
            new Treasure(400, 600), new Treasure(600, 600),
            new Enemy(100, 100, player), new Enemy(700, 700, player)
        };

        IntPtr texturePlayer = LoadTexture(sdl, renderer, "Assets/pirata.png");
        IntPtr textureTreasure = LoadTexture(sdl, renderer, "Assets/tesoro.png");
        IntPtr textureEnemy = LoadTexture(sdl, renderer, "Assets/enemigo.png");
        IntPtr textureBackground = LoadTexture(sdl, renderer, "Assets/fondo.png");

        bool quit = false;
        bool hasLost = false;
        int playerDirection = 0;

        // Variables para el sistema de récord
        int currentMatchWins = 0;
        var save = SaveManager.LoadGame();
        int highScore = save.TotalWins;

        Console.WriteLine($"--- GAME STARTED ---");
        Console.WriteLine($"Current High Score to beat: {highScore}");
        Console.WriteLine($"----------------------");

        timer.Restart();

        // Bucle del juego
        while (!quit)
        {
            while (sdl.PollEvent(ref ev) != 0)
            {
                if (ev.Type == (uint)EventType.Quit) { quit = true; break; }
            }

            var elapsed = timer.Elapsed;
            timer.Restart();
            var elapsedMs = elapsed.TotalMilliseconds;

            if (!hasLost)
            {
                // Controles
                double up = 0, down = 0, left = 0, right = 0;
                if (keyboardState[(byte)KeyCode.W] > 0 || keyboardState[(byte)KeyCode.Up] > 0) { up = 1; playerDirection = 3; }
                if (keyboardState[(byte)KeyCode.S] > 0 || keyboardState[(byte)KeyCode.Down] > 0) { down = 1; playerDirection = 0; }
                if (keyboardState[(byte)KeyCode.A] > 0 || keyboardState[(byte)KeyCode.Left] > 0) { left = 1; playerDirection = 2; }
                if (keyboardState[(byte)KeyCode.D] > 0 || keyboardState[(byte)KeyCode.Right] > 0) { right = 1; playerDirection = 1; }

                player.UpdatePosition(up, down, left, right, elapsedMs);

                // Colisiones y actualización
                for (int i = gameObjects.Count - 1; i >= 0; i--)
                {
                    var obj = gameObjects[i];

                    if (obj is Treasure t)
                    {
                        if (Math.Abs(player.X - t.X) < 20 && Math.Abs(player.Y - t.Y) < 20) t.OnInteract(player);
                    }
                    if (obj is Enemy e)
                    {
                        if (Math.Abs(player.X - e.X) < 15 && Math.Abs(player.Y - e.Y) < 15) hasLost = true;
                    }

                    if (!obj.Update(elapsedMs)) gameObjects.RemoveAt(i);
                }

                // Siguiente nivel
                if (!gameObjects.OfType<Treasure>().Any())
                {
                    Random rnd = new Random();
                    for (int i = 0; i < 4; i++) gameObjects.Add(new Treasure(rnd.Next(50, 750), rnd.Next(50, 750)));

                    int wall = rnd.Next(0, 4);
                    int enemyX = 0;
                    int enemyY = 0;

                    if (wall == 0) { enemyX = rnd.Next(0, 800); enemyY = -50; }
                    else if (wall == 1) { enemyX = rnd.Next(0, 800); enemyY = 850; }
                    else if (wall == 2) { enemyX = -50; enemyY = rnd.Next(0, 800); }
                    else { enemyX = 850; enemyY = rnd.Next(0, 800); }

                    gameObjects.Add(new Enemy(enemyX, enemyY, player));

                    currentMatchWins++;

                    timer.Restart();

                    Console.WriteLine($"Level completed! Wins in this match: {currentMatchWins}");

                    // Si las victorias de esta partida superan el récord, actualizamos el JSON
                    if (currentMatchWins > highScore)
                    {
                        highScore = currentMatchWins;
                        save = save with { TotalWins = highScore };
                        SaveManager.SaveGame(save);
                        Console.WriteLine($"NEW HIGH SCORE ACHIEVED: {highScore}!");
                    }
                }
            }

            // Renderizado
            unsafe
            {
                var r = (Renderer*)renderer;

                if (hasLost)
                {
                    // Pantalla roja de Game Over
                    sdl.SetRenderDrawColor(r, 200, 0, 0, 255);
                    sdl.RenderClear(r);
                }
                else
                {
                    sdl.RenderClear(r);

                    // Fondo 
                    var bgRect = new Rectangle<int>(0, 0, 800, 800);
                    sdl.RenderCopy(r, (Silk.NET.SDL.Texture*)textureBackground, null, &bgRect);

                    // Enemigos y tesoros
                    foreach (var obj in gameObjects)
                    {
                        var visualRect = new Rectangle<int>(obj.X - 30, obj.Y - 30, 80, 80);

                        if (obj is TheAdventure.Models.Enemy)
                        {
                            sdl.RenderCopy(r, (Silk.NET.SDL.Texture*)textureEnemy, null, &visualRect);
                        }
                        else if (obj is TheAdventure.Models.Treasure)
                        {
                            sdl.RenderCopy(r, (Silk.NET.SDL.Texture*)textureTreasure, null, &visualRect);
                        }
                    }

                    // Dibujar jugador
                    var pVisualRect = new Rectangle<int>(player.X - 30, player.Y - 30, 80, 80);

                    // Obtenemos las dimensiones reales pra no depender del JSON (Lab 9)
                    var texPtr = (Silk.NET.SDL.Texture*)texturePlayer;
                    int texWidth, texHeight, texFormat, texAccess;
                    sdl.QueryTexture(texPtr, (uint*)&texFormat, &texAccess, &texWidth, &texHeight);

                    int quadWidth = texWidth / 2;
                    int quadHeight = texHeight / 2;

                    // Recorte según la tecla pulsada
                    int srcX = (playerDirection % 2) * quadWidth;
                    int srcY = (playerDirection / 2) * quadHeight;

                    var pSrcRect = new Rectangle<int>(srcX, srcY, quadWidth, quadHeight);

                    sdl.RenderCopy(r, texPtr, &pSrcRect, &pVisualRect);
                }

                sdl.RenderPresent(r);
            }
            framesRenderedCounter++;
        }

        Console.WriteLine("\n==============================");
        Console.WriteLine("          GAME OVER           ");
        Console.WriteLine("==============================");
        Console.WriteLine($"Total wins: {currentMatchWins}");
        if (currentMatchWins >= highScore && currentMatchWins > 0)
        {
            Console.WriteLine($"Amazing! You set the new high score to {highScore}.");
        }
        Console.WriteLine("==============================\n");

        unsafe { sdl.DestroyWindow((Window*)window); }
        sdl.Quit();
    }

    // Carga de texturas (Lab 7)
    private static unsafe IntPtr LoadTexture(Sdl sdl, IntPtr renderer, string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Error: File not found at {path}");
            return IntPtr.Zero;
        }

        using (var fStream = new FileStream(path, FileMode.Open))
        {
            // Usamos ImageSharp para las transparencias
            var image = Image.Load<Rgba32>(fStream);

            var imageRAWData = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(imageRAWData.AsSpan());

            // Punteros para la superficie SDL
            fixed (byte* data = imageRAWData)
            {
                var imageSurface = sdl.CreateRGBSurfaceWithFormatFrom(data, image.Width,
                    image.Height, 8, image.Width * 4, (uint)PixelFormatEnum.Rgba32);

                if (imageSurface == null)
                {
                    Console.WriteLine("Error creating the RGB surface.");
                    return IntPtr.Zero;
                }

                var imageTexture = sdl.CreateTextureFromSurface((Renderer*)renderer, imageSurface);
                sdl.FreeSurface(imageSurface);

                return (IntPtr)imageTexture;
            }
        }
    }
}