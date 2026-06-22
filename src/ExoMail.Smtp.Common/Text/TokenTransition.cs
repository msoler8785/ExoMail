using System.Collections.Generic;

namespace ExoMail.Smtp.Common.Text
{
    public class TokenTransition
    {
        public Dictionary<int, TokenState> Transition { get; set; }

        public TokenTransition()
        {
            Transition = new Dictionary<int, TokenState>();
        }

        public void Add(TokenState current, char character, TokenState next)
        {
            int currentState = (int)character << (int)current;

            Transition.Add(currentState, next);
        }

        public TokenState MoveNext(char character, TokenState current)
        {
            int currentState = (int)character << (int)current;

            if (Transition.ContainsKey(currentState))
            {
                TokenState next = Transition[currentState];
                return next;
            }

            switch (current)
            {
                case TokenState.Escaped:
                    return TokenState.Quote;

                case TokenState.Delimeter:
                    return TokenState.InitVal;

                case TokenState.Quote:
                    return TokenState.Quote;

                default:
                    return current;
            }
        }
    }
}