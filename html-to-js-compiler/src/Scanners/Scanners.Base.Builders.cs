
namespace HTMLToJS {

    public static partial class Scanners {

        public static ScanCharCondition OneOf(params char[] characters) {
            return OneOf(characters.AsEnumerable());
        }

        public static ScanCharCondition OneOf(IEnumerable<char> characters) {
            HashSet<char> chars = new(characters);
            return chars.Contains;
        }

        public static ScanCharCondition Except(this ScanCharCondition baseCondition, params char[] characters) {
            return baseCondition.Except(characters);
        }

        public static ScanCharCondition Except(this ScanCharCondition baseCondition, IEnumerable<char> characters) {
            HashSet<char> forbidden = new(characters);
            return (char c) => baseCondition(c) && !forbidden.Contains(c);
        }
    }
}