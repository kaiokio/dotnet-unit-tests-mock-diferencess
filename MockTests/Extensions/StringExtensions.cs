using System;
using System.Collections.Generic;
using System.Text;

namespace MockTests.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string text)
            => string.IsNullOrWhiteSpace(text);
    }
}
