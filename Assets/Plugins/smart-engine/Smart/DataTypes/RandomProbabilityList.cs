using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Smart.DataTypes
{
	public class RandomProbabilityList<T>
	{
		private readonly List<ListItem<T>> _values;
		
		public float CumulativeProbability { get; private set; }

		public RandomProbabilityList()
		{
			_values = new List<ListItem<T>>();
		}
		
		public void Clear()
		{
			CumulativeProbability = 0;
			_values.Clear();
		}
		
		public void Add(T value, float probability)
		{
			CumulativeProbability += probability;
			var item = new ListItem<T>(value, probability, CumulativeProbability);
			_values.Add(item);
		}
        
        public T GetRandomValue(int seed)
        {
            Random.InitState(seed);
        	var probability = Random.Range(0, CumulativeProbability);
            return GetValue(probability);
        }
        
        public T GetRandomValue()
        {
        	var probability = Random.Range(0, CumulativeProbability);
            return GetValue(probability);
        }
		
		public T GetValue(float probability)
		{
			for (int i = 0; i < _values.Count; i++)
			{
				var item = _values[i];

				if (item.cumulativeProbability > probability)
				{
					return item.value;
				}
			}
			
			return _values.Last().value;
		}
		
		public void SetProbability(T value, int probability)
		{
			for (int i = 0; i < _values.Count; i++)
			{
				var item = _values[i];

				if (item.value.Equals(value))
				{
					item.probability = probability;
				}
			}
			
			RecalculateCumulativeProbability();
		}

		private void RecalculateCumulativeProbability()
		{
			CumulativeProbability = 0;
			
			for (int i = 0; i < _values.Count; i++)
			{
				var item = _values[i];
				CumulativeProbability += item.probability;
				item.cumulativeProbability = CumulativeProbability;
			}
		}

		public float GetProbability(T value)
		{
			for (int i = 0; i < _values.Count; i++)
			{
				var item = _values[i];

				if (item.value.Equals(value))
				{
					return item.probability;
				}
			}
			
			throw new Exception("");
		}

		public T this[int i]
		{
			get { return _values[i].value; }
		}

		private class ListItem<TItem>
		{
			public readonly TItem value;
			public float probability;
			public float cumulativeProbability;
			
			public ListItem(TItem value, float probability, float cumulativeProbability)
			{
				this.value = value;
				this.probability = probability;
				this.cumulativeProbability = cumulativeProbability;
			}
		}
	}
}