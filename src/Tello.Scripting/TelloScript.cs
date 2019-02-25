using Newtonsoft.Json;
using System;
using System.Linq;

namespace Tello.Scripting
{
    public class TelloScript
    {
        private TelloScript(Token[] tokens)
        {
            _tokens = tokens;
        }

        private int _currentToken = 0;
        internal readonly Token[] _tokens;

        public static TelloScript FromJson(string json)
        {
            if (String.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            var tokens = JsonConvert.DeserializeObject<Token[]>(json)
                .OrderBy(t => t.Order)
                .ToArray();
            return new TelloScript(tokens);
        }

        public Token NextToken()
        {
            return _currentToken < _tokens.Length
                ? _tokens[_currentToken++]
                : null;
        }

        public void Replay() { _currentToken = 0; }
    }
}
