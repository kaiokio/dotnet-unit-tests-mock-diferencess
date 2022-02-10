using System;
using System.Collections.Generic;
using System.Text;

namespace MockTests.Extensions
{
    public static class IEnumerableExcentions
    {
        public static string ToPipedMessage(this IEnumerable<string> messages)
            => messages != null
                ? string.Join(" | ", messages)
                : string.Empty;
    }
}
