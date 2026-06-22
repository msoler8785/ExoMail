using System;
using System.Collections.Generic;

namespace ExoMail.Smtp.Common.Text
{
    public class TokenEnumerator
    {
        private TokenTransition _tokenTransition;

        public TokenEnumerator()
        {
            _tokenTransition = new TokenTransition();
            _tokenTransition.Add(TokenState.InitKey, ' ', TokenState.Finished);
            _tokenTransition.Add(TokenState.InitKey, ':', TokenState.Delimeter);
            _tokenTransition.Add(TokenState.InitKey, '=', TokenState.Delimeter);
            _tokenTransition.Add(TokenState.Delimeter, '=', TokenState.Delimeter);
            //_tokenTransition.Add(TokenState.Delimeter, ' ', TokenState.Finished);
            _tokenTransition.Add(TokenState.InitVal, ' ', TokenState.Finished);
            _tokenTransition.Add(TokenState.InitVal, '"', TokenState.Quote);
            _tokenTransition.Add(TokenState.Quote, '"', TokenState.InitVal);
            _tokenTransition.Add(TokenState.Quote, '\\', TokenState.Escaped);
        }

        public Dictionary<string, string> GetTokens(string line)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            int index = 0;

            for (int cursor = 0; cursor < line.Length;)
            {
                TokenState state = TokenState.InitKey;

                while (state != TokenState.Finished && ++cursor < line.Length)
                {
                    state = _tokenTransition.MoveNext(line[cursor], state);

                    if (state == TokenState.InitVal)
                    {
                        cursor--;
                        break;
                    }
                }

                string key = line.Substring(index, cursor - index);

                // Skip through any extraneous spaces.
                while (cursor + 1 < line.Length && line[cursor + 1] == ' ')
                    cursor++;

                index = cursor + 1;

                while (state != TokenState.Finished && ++cursor < line.Length)
                {
                    state = _tokenTransition.MoveNext(line[cursor], state);
                }

                string value = null;
                if (index < cursor)
                {
                    value = line.Substring(index, cursor - index);
                    index = cursor + 1;
                }

                while (cursor + 1 < line.Length && line[cursor + 1] == ' ')
                    cursor++;

                result.Add(key, value);
            }

            return result;
        }
    }
}