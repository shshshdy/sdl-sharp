﻿using System.Runtime.InteropServices;

using SdlSharp;
using SdlSharp.Graphics;

using Application app =
    (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Major >= 10) ?// windows 10 及以上 使用 dx11
    new(Subsystems.Video, ImageFormats.Png, hints: new[] { (Hint.RenderDriver, "direct3d11") }) :
    new(Subsystems.Video, ImageFormats.Png);

//using Application app = new(Subsystems.Video, ImageFormats.Png);
Size windowSize = new(640, 480);
Rectangle windowRectangle = new(Window.UndefinedWindowLocation, windowSize);
using var window = Window.Create("Sprites", windowRectangle, WindowOptions.Shown);
using var renderer = Renderer.Create(window, -1, RendererOptions.Accelerated | RendererOptions.PresentVSync);
window.Title += $" -{renderer.Info.Name}";
using var floor = Image.Load("Floor.png", renderer);
using var player0 = Image.Load("Player0.png", renderer);
using var player1 = Image.Load("Player1.png", renderer);
Size spriteSize = new(16, 16);

Point upperLeftFloor = new(7, 15);
Point upperRightFloor = new(9, 15);
Point lowerLeftFloor = new(7, 17);
Point lowerRightFloor = new(9, 17);

var next = DateTime.Now.AddMilliseconds(500);
var current = 0;
var rotation = 0;

while (app.DispatchEvents())
{
    if (DateTime.Now > next)
    {
        next = DateTime.Now.AddMilliseconds(500);
        current = (current + 1) % 2;
        rotation = (rotation + 90) % 360;
    }

    renderer.DrawColor = Colors.Black;
    renderer.Clear();
    renderer.DrawColor = Colors.Red;
    renderer.DrawRectangle(new Rectangle(new Point(200, 20), new Size(100, 100)));

    renderer.Copy(floor, new(upperLeftFloor * 16, spriteSize), new(Point.Origin, spriteSize * 4));
    renderer.Copy(floor, new(upperRightFloor * 16, spriteSize), new(new(spriteSize.Width * 4, 0), spriteSize * 4));
    renderer.Copy(floor, new(lowerLeftFloor * 16, spriteSize), new(new(0, spriteSize.Height * 4), spriteSize * 4));
    renderer.Copy(floor, new(lowerRightFloor * 16, spriteSize), new(new(spriteSize.Width * 4, spriteSize.Height * 4), spriteSize * 4));

    var currentPlayer = current == 0 ? player0 : player1;
    renderer.Copy(currentPlayer, new(Point.Origin, spriteSize), new(Point.Origin, spriteSize * 4));

    currentPlayer.ColorMod = (0x8F, 0x8F, 0x8F);
    renderer.Copy(currentPlayer, new(Point.Origin, spriteSize), new(new(spriteSize.Width * 4, 0), spriteSize * 4));
    currentPlayer.ColorMod = (0xFF, 0xFF, 0xFF);

    currentPlayer.AlphaMod = 0x8F;
    renderer.Copy(currentPlayer, new(Point.Origin, spriteSize), new(new(0, spriteSize.Height * 4), spriteSize * 4));
    currentPlayer.AlphaMod = 0xFF;

    renderer.Copy(currentPlayer, new(Point.Origin, spriteSize), new(new(spriteSize.Width * 4, spriteSize.Height * 4), spriteSize * 4), rotation, null, RendererFlip.None);

    renderer.Present();
}
