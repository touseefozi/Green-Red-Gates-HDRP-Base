using System;
using UnityEngine;

namespace Smart.Formatters
{
    [CreateAssetMenu(fileName = "LongFormatter", menuName = "Formatters/Long Formatter")]
    public class LongFormatter : ScriptableObject
    {
        [Serializable]
        private class Rule
        {
            public long MinValue;
            public float Divider;
            public string Format;
        }
        
        [SerializeField] private Rule[] _rules;
        
        public string Format(long value)
        {
            for (int i = 0; i < _rules.Length; i++)
            {
                var rule = _rules[i];
                
                if (value >= rule.MinValue)
                {
                    return string.Format(rule.Format, value / rule.Divider);
                }
            }
            
        	return value.ToString();
        }
    }
}