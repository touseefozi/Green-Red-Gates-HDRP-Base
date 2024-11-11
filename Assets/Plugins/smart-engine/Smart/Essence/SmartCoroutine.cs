using System.Collections;

namespace Smart.Essence
{
    public class SmartCoroutine
    {
        private static int _lastId;
        
        public readonly int Id;
        public readonly IEnumerator Routine;

        public SmartCoroutine(IEnumerator routine)
        {
            Id =  _lastId++;
            Routine = routine;
        }
    }
}