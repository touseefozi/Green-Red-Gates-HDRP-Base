using System;

namespace Smart.Essence
{
	public interface IEvent
	{
		int Hash { get; }
	}

	public class SmartEventBase : IEvent
	{
		public int Hash { get; }
		public bool IsCrossContext { get; protected set; }

		public SmartEventBase()
		{
			Hash = Guid.NewGuid().GetHashCode();
		}
	}

	public class SmartEvent : SmartEventBase
	{
        public SmartEvent CrossContext()
        {
            IsCrossContext = true;
            return this;
        }
	}

	public class SmartEvent<T> : SmartEventBase 
	{
        public SmartEvent<T> CrossContext()
        {
            IsCrossContext = true;
            return this;
        }
	}
}