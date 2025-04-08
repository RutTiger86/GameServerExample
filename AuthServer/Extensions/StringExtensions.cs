using System.Text;

namespace AuthServer.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Snake_Case 문자열을 PascalCase로 변환
        /// </summary>
        /// <param name="input">Snake_Case 문자열</param>
        /// <returns>PascalCase 문자열</returns>
        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var words = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();

            foreach (var word in words)
            {
                if (word.Length > 0)
                {
                    sb.Append(char.ToUpper(word[0])); // 첫 글자 대문자
                    if (word.Length > 1)
                        sb.Append(word.Substring(1).ToLower()); // 나머지는 소문자 (선택)
                }
            }

            return sb.ToString();
        }
    }

}
