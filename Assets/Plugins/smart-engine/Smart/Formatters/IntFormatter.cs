using System;
using UnityEngine;

namespace Smart.Formatters
{
    [CreateAssetMenu(fileName = "NumberFormatter", menuName = "Formatters/Int Formatter")]
    public class IntFormatter : ScriptableObject
    {
        [Serializable]
        private class Rule
        {
            public int MinValue;
            public float Divider;
            public string Format;
        }
        
        [SerializeField] private Rule[] _rules;
        
        public string Format(int value)
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