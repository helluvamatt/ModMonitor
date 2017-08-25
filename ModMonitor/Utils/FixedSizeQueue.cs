using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Utils
{
    internal class FixedSizeQueue<T> : IEnumerable<T>
    {
        private Queue<T> queue = new Queue<T>();

        public int Limit { get; private set; }

        public FixedSizeQueue(int limit)
        {
            Limit = limit;
        }

        public void Enqueue(T obj)
        {
            queue.Enqueue(obj);
            while (queue.Count > Limit)
            {
                queue.Dequeue();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }
}
