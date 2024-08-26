// See https://aka.ms/new-console-template for more information

using Sokoban;

var stageText =
"""
########
###  @##
# $ # ##
# #    #
# *  # #
##$#*  #
##@  ###
########
""";

var view = new SokobanView(stageText,out var tiles);
var stage = new SokobanGame<SokobanEntity>(tiles,view);
Console.WriteLine("--------------------");
Console.WriteLine("Hello! This is a sokoban test!");
Console.WriteLine("You need to move all boxes to goals");
Console.WriteLine("# : wall, $ : box  @ : player * : goal");
Console.WriteLine("! : player on goal + is box on goal");
Console.WriteLine("Use arrow keys to move, Z to undo, R to reset, Q to quit");
Console.WriteLine("--------------------");
Console.WriteLine(stageText);

while (true)
{
    var key = Console.ReadKey();
    Direction? direction=null;
    switch (key.Key)
    {
        case ConsoleKey.UpArrow:
            direction = Direction.Up;
            break;
        case ConsoleKey.DownArrow:
            direction = Direction.Down;
            break;
        case ConsoleKey.LeftArrow:
            direction = Direction.Left;
            break;
        case ConsoleKey.RightArrow:
            direction = Direction.Right;
            break;
        case ConsoleKey.Enter:
        case ConsoleKey.Q:
            Console.WriteLine("Quit");
            return;
        case ConsoleKey.R:
            stage.Reset();
            break;
        case ConsoleKey.Z:
            Console.WriteLine("Undo");
            stage.TryUndo();
            break;
    }

    MoveResult? result = null;
    if(direction!=null)
    {
         result = stage.MovePlayer(direction.Value);
        if (result.Value == MoveResult.Blocked)
        {
            continue;
        }
    }
    Console.Clear();
    Console.WriteLine(view.GetCurrentStateString());
    if (result == MoveResult.Win)
    {
        Console.WriteLine("You win!");
        break;
    }
}