namespace BoxPacker
{
    public class IndexedStack<T>
    {
        private readonly Dictionary<(int startIndex, int endIndex), T> _ranges = new Dictionary<(int, int), T>();
        private readonly int _maxIndex;

        public IndexedStack(Stack<(T val, int count)> cubes) {
            var index = 0;
            foreach (var item in cubes) {
                _ranges.Add((index, index + item.count - 1), item.val);
                index += item.count;
            }
            _maxIndex = index - 1;
        }

        public int Count => _maxIndex + 1;

        public T this[int i] {
            get {
                if (i < 0 || i > _maxIndex) {
                    throw new IndexOutOfRangeException();
                }

                foreach (var range in _ranges.Keys) {
                    if (i >= range.startIndex && i <= range.endIndex) {
                        return _ranges[range];
                    }
                }
                return default;
            }
        }
    }
}
