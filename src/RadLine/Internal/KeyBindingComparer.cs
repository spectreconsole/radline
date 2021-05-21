using System;
using System.Collections.Generic;

namespace RadLine
{
    internal sealed class KeyBindingComparer : IEqualityComparer<KeyBinding>
    {
        public bool Equals(KeyBinding? x, KeyBinding? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Key == y.Key && x.Modifiers == y.Modifiers;
        }

        public int GetHashCode(KeyBinding obj)
        {
#if NET5_0
            return HashCode.Combine(obj.Key, obj.Modifiers);
#else
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + (int)obj.Key;
                hash = (hash * 23) + (obj?.Modifiers != null ? (int)obj.Modifiers : 0);
                return hash;
            }
#endif
        }
    }
}
