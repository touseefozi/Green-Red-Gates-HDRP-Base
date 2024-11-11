using System;
using UnityEngine;

namespace Smart.Formatters
{
    [CreateAssetMenu(fileName = "IntRoundingRule", menuName = "Formatters/Integer Rounding Rule")]
    public class IntRoundingRule : ScriptableObject
    {
        [Serializable]
        private class Rule
        {
            public int MinValue;
            public int Divider;
        }
        
        [SerializeField] private Rule[] _rules;
        
        public int Round(int value)
        {
            for (int i = 0; i < _rules.Length; i++)
            {
                var rule = _rules[i];
                
                if (value >= rule.MinValue)
                {
                    return Mathf.RoundToInt(value / rule.Divider) * rule.Divider;
                }
            }
            
            return value;
        }
    }
}