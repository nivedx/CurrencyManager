namespace CurrencyManager.Helpers
{
    public static class DictionaryExtensions
    {
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            IList<TKey> removeValues)
        {
            if (dict.Count > 0)
            {
                var keys = dict.Keys.Where(k => removeValues.Contains(k)).ToList();
                foreach (var key in keys)
                {
                    dict.Remove(key);
                }
            }            
        }
    }
}
