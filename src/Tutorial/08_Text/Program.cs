﻿using System.Runtime.InteropServices;

using SdlSharp;
using SdlSharp.Graphics;
using SdlSharp.Input;
var fullscreen = false;

using Application app =
    (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Major >= 10) ?// windows 10 及以上 使用 dx11
    new(Subsystems.Video, fontSupport: true, hints: new[] { (Hint.RenderDriver, "direct3d11") }) :
    new(Subsystems.Video, fontSupport: true);
Size windowSize = new(640, 480);
Rectangle windowRectangle = new(Window.UndefinedWindowLocation, windowSize);
using var window = Window.Create("Text", windowRectangle, WindowOptions.Shown);
using var renderer = Renderer.Create(window, -1, RendererOptions.Accelerated);
using var font = Font.Create("simsun.ttf", 14);
using var textTexture = font.RenderSolid("图形API：" + renderer.Info.Name + " 中文중국の城門The quick brown fox jumped over the lazy dog", Colors.White, renderer);

Color textColor = new(0xFF, 0xFF, 0xFF, 0xFF);
var inputText = "Some editable text";
var inputTextTexture = font.RenderSolid(inputText, textColor, renderer);
var renderText = false;

Keyboard.StartTextInput();
Keyboard.SetTextInputRectangle(new Rectangle { Location = new Point(12, 42) });
Keyboard.KeyDown += (s, e) =>
{
    switch (e.Keycode)
    {
        case Keycode.Escape:
            fullscreen = !fullscreen;
            Keyboard.StopTextInput();
            window.SetFullscreen(fullscreen, false);
            Keyboard.StartTextInput();
            break;
        case Keycode.Backspace:
            if (inputText.Length > 0)
            {
                inputText = inputText[0..^1];
                renderText = true;
            }
            break;
        case Keycode.c:
            if ((Keyboard.KeyModifierState & KeyModifier.Ctrl) != 0)
            {
                Clipboard.Text = inputText;
            }
            break;
        case Keycode.v:
            if ((Keyboard.KeyModifierState & KeyModifier.Ctrl) != 0)
            {
                inputText = Clipboard.Text ?? string.Empty;
                renderText = true;
            }
            break;
    }
};

Keyboard.TextInput += (s, e) =>
{
    if ((e.Text[0] is not ('c' or 'C' or 'v' or 'V')) || ((Keyboard.KeyModifierState & KeyModifier.Ctrl) is 0))
    {
        inputText += e.Text;
        renderText = true;
    }
};

while (app.DispatchEvents())
{
    renderer.DrawColor = Colors.Black;
    renderer.Clear();

    if (renderText)
    {
        inputTextTexture.Dispose();
        inputTextTexture = font.RenderSolid(string.IsNullOrEmpty(inputText) ? " " : inputText, textColor, renderer);
        renderText = false;
    }

    renderer.Copy(textTexture, null, new(Point.Origin, textTexture.Size));
    renderer.Copy(inputTextTexture, null, new(new(0, textTexture.Size.Height * 2), inputTextTexture.Size));

    renderer.Present();
}

inputTextTexture.Dispose();
