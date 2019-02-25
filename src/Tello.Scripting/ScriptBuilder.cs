using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tello.Messaging;

namespace Tello.Scripting
{
    public class ScriptBuilder
    {
        public ScriptBuilder() { _tokens = new Dictionary<string, Token>(); }
        public ScriptBuilder(int size) { _tokens = new Dictionary<string, Token>(size); }

        public ScriptBuilder FromJson(string json)
        {
            if (String.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            var tokens = JsonConvert.DeserializeObject<Token[]>(json);
            var result = new ScriptBuilder();
            for (var i = 0; i < tokens.Length; ++i)
            {
                var token = tokens[i];
                _tokens.Add(token.Id, token);
            }
            return result;
        }

        private int _order = 0;
        private readonly Dictionary<string, Token> _tokens;

        public Token AddToken(string id, TelloCommands command, params object[] args)
        {
            if (_tokens.ContainsKey(id))
            {
                throw new TokenAlreadyExistsException($"{nameof(id)}: '{id}'");
            }

            var token = new Token
            {
                Id = id,
                Command = command,
                Args = args.Length == 0 ? null : args,
                Order = ++_order
            };

            _tokens.Add(id, token);

            return token;
        }

        public void RemoveToken(string id)
        {
            _tokens.Remove(id);
        }

        /// <summary>
        /// left and right refer to the timeline. left is sooner, right is later. given two commands, the one on the left will execute first
        /// </summary>
        /// <param name="id"></param>
        public void MoveLeft(string id)
        {
            if (!_tokens.ContainsKey(id))
            {
                throw new TokenNotFoundException($"{nameof(id)}: '{id}'");
            }

            if (_tokens.Count > 1)
            {
                if (_tokens.TryGetValue(id, out var rightToken))
                {
                    if (rightToken.Order > 0)
                    {
                        var leftToken = _tokens.Values
                           .Where((t) => t.Order == rightToken.Order - 1)
                           .First();

                        rightToken.Order -= 1;
                        leftToken.Order += 1;
                    }
                }
            }
        }

        /// <summary>
        /// left and right refer to the timeline. left is sooner, right is later. given two commands, the one on the left will execute first
        /// </summary>
        /// <param name="id"></param>
        public void MoveRight(string id)
        {
            if (!_tokens.ContainsKey(id))
            {
                throw new TokenNotFoundException($"{nameof(id)}: '{id}'");
            }

            if (_tokens.Count > 1)
            {
                if (_tokens.TryGetValue(id, out var leftToken))
                {
                    if (leftToken.Order < _tokens.Count)
                    {
                        var rightToken = _tokens.Values
                           .Where((t) => t.Order == leftToken.Order + 1)
                           .First();

                        rightToken.Order -= 1;
                        leftToken.Order += 1;
                    }
                }
            }
        }

        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public string ToJson()
        {
            var tokens = _tokens
                .Values
                .OrderBy((t) => t.Order)
                .ToArray();
            return JsonConvert.SerializeObject(tokens, _settings);
        }
    }
}
