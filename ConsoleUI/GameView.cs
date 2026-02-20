using Core;

namespace ConsoleUI
{
    public static class GameView
    {
        private static readonly Random _random = new Random();

        public static void Run()
        {
            while (true)
            {
                var game = new Game();
                game.Shuffle(_random);

                bool quit = RunSingleGame(game);
                if (quit)
                    break;

                Console.WriteLine();
                Console.Write("Vil du spille en gang til? (y/n): ");
                var answer = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (answer != "y" && answer != "ja")
                    break;
            }
        }

        private static bool RunSingleGame(Game game)
        {
            while (true)
            {
                Console.Clear();
                PrintGame(game);

                if (game.IsSolved)
                {
                    Console.WriteLine();
                    Console.WriteLine("Gratulerer! Du løste puslespillet på {0} trekk.", game.PlayCount);
                    return false; // ikke avslutt hele programmet, bare dette spillet
                }

                Console.WriteLine();
                Console.WriteLine("Skriv rad og kolonne (1-3) for brikken du vil flytte.");
                Console.WriteLine("Eksempel: 2 3");
                Console.WriteLine("Skriv Q for å avslutte programmet.");
                Console.Write("Valg: ");

                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
                    return true; // avslutt hele programmet

                if (!TryParseRowCol(input, out int row, out int col))
                {
                    ShowMessage("Ugyldig input. Bruk formatet 'rad kolonne' med tall mellom 1 og 3 (f.eks. '2 3').");
                    continue;
                }

                int index = (row - 1) * 3 + (col - 1);

                bool moved = game.Play(index);
                if (!moved)
                {
                    ShowMessage("Ugyldig trekk. Brikken du valgte kan ikke flyttes inn i den tomme ruten.");
                }
            }
        }

        private static void PrintGame(Game game)
        {
            Console.WriteLine("Number Puzzle (8-puslespill)");
            Console.WriteLine("----------------------------");
            Console.WriteLine("Trekk brukt: {0}", game.PlayCount);
            Console.WriteLine();

            // Printer 3x3-brettet
            var numbers = game.Numbers; // char[]

            for (int row = 0; row < 3; row++)
            {
                Console.Write("   ");
                for (int col = 0; col < 3; col++)
                {
                    int index = row * 3 + col;
                    char c = numbers[index];

                    // viser blank som mellomrom
                    string symbol = c == ' ' ? " " : c.ToString();

                    Console.Write(" {0} ", symbol);
                }
                Console.WriteLine();
            }
        }

        private static bool TryParseRowCol(string input, out int row, out int col)
        {
            row = 0;
            col = 0;

            // Tillat f.eks. "2 3" eller "2,3"
            var parts = input
                .Replace(",", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out row) || !int.TryParse(parts[1], out col))
                return false;

            if (row < 1 || row > 3 || col < 1 || col > 3)
                return false;

            return true;
        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine("Trykk en tast for å fortsette...");
            Console.ReadKey(true);
        }
    }
}