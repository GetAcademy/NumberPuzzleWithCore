namespace Core
{
    public class Game
    {
        private readonly int[] _numbers;

        public int PlayCount { get; private set; }

        // Brettet er løst når det er 1,2,3,4,5,6,7,8,0
        public bool IsSolved =>
            Enumerable.Range(0, _numbers.Length - 1)
                .All(i => _numbers[i] == i + 1);

        // Representasjon for UI: '1'..'8' og ' ' for 0
        public char[] Numbers =>
            Enumerable.Range(0, _numbers.Length)
                .Select(i => this[i])
                .ToArray();

        // Indexer for å lese brettet som tegn
        public char this[int i] =>
            _numbers[i].ToString().Replace('0', ' ')[0];

        /// <summary>
        /// Lager et nytt spill med enten gitt tallrekke,
        /// eller standard 0–8 (0 = blank).
        /// Ingen automatisk shuffle – det tar UI seg av.
        /// </summary>
        public Game(int[] numbers = null)
        {
            _numbers = numbers != null
                ? (int[])numbers.Clone()
                : Enumerable.Range(0, 9).ToArray();

            PlayCount = 0;
        }

        /// <summary>
        /// Brukes når vi laster et spill fra database:
        /// vi får med både PlayCount og tallrekka.
        /// </summary>
        public Game(int playCount, int[] numbers)
        {
            PlayCount = playCount;
            _numbers = (int[])numbers.Clone();
        }

        /// <summary>
        /// Forsøker å flytte brikken på gitt indeks.
        /// Returnerer true hvis trekket er gyldig og utført,
        /// ellers false.
        /// </summary>
        public bool Play(int index)
        {
            var blankNeighbourIndex = GetBlankNeighbourIndex(index);
            if (blankNeighbourIndex == null) return false;

            Swap(index, blankNeighbourIndex.Value);
            PlayCount++;
            return true;
        }

        /// <summary>
        /// Blander brettet ved å gjøre en enkel shuffle.
        /// Random leveres utenfra (fra UI/skall) for å gjøre koden
        /// enklere å teste og mer forutsigbar ved behov.
        /// </summary>
        public void Shuffle(Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));

            var n = _numbers.Length - 1;
            while (n > 1)
            {
                Swap(n, random.Next(n + 1));
                n--;
            }

            PlayCount = 0;
        }

        private int? GetBlankNeighbourIndex(int index)
        {
            var row = index / 3;
            var col = index % 3;

            if (col < 2 && IsBlank(index + 1)) return index + 1;
            if (col > 0 && IsBlank(index - 1)) return index - 1;
            if (row < 2 && IsBlank(index + 3)) return index + 3;
            if (row > 0 && IsBlank(index - 3)) return index - 3;

            return null;
        }

        private bool IsBlank(int i)
        {
            return i >= 0 && i < _numbers.Length && _numbers[i] == 0;
        }

        private void Swap(int n, int k)
        {
            var temp = _numbers[n];
            _numbers[n] = _numbers[k];
            _numbers[k] = temp;
        }

        /// <summary>
        /// Henter en kopi av den interne talltabellen.
        /// Nyttig for lagring i database.
        /// </summary>
        public int[] GetNumbersSnapshot()
        {
            return (int[])_numbers.Clone();
        }
    }
}