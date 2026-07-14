using System;

internal class Program {
    private static void Main() {
        using var game = new TestArea.OBBVsCirc();
        game.Run();
    }
}