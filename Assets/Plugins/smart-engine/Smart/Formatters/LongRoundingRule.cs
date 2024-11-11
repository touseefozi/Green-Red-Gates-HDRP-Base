using System;
using UnityEngine;

namespace Smart.Formatters
{
    [CreateAssetMenu(fileName = "LongRoundingRule", menuName = "Formatters/Long Rounding Rule")]
    public class LongRoundingRule : ScriptableObject
    {
        [Serializable]
        private class Rule
        {
            public long MinValue;
            public long Divider;
        }
        
        [SerializeField] private Rule[] _rules;
        
        public long Round(long value)
        {
            for (int i = 0; i < _rules.Length; i++)
            {
                var rule = _rules[i];
                
                if (value >= rule.MinValue)
                {
                    return (long) Mathf.Round(value / rule.Divider) * rule.Divider;
                }
            }
            
            return value;
        }
    }
}