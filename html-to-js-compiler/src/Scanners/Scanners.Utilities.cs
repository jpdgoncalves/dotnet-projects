
namespace HTMLToJS {

    public static partial class Scanners {
        
        /// <summary>
        /// Creates a list of characters
        /// within the range of start and end (both inclusive)
        /// </summary>
        /// <param name="start">The start character</param>
        /// <param name="end">The end character</param>
        /// <returns>A list containing characters from start to end (start and end included)</returns>
        public static List<char> createCharRange(char start, char end)
        {
            if (end < start) return new List<char>();
            var list = new List<char>(end - start);

            for (var i = start; i <= end; i++)
            {
                list.Add(i);
            }

            return list;
        }

        public static HashSet<char> createCharSet(params IEnumerable<char>[] charLists) {
            var combined = new List<char>();
            foreach (var charList in charLists) {
                combined.AddRange(charList);
            }
            return new(combined);
        }
    }
}