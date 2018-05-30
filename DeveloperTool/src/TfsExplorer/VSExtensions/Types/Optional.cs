using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Types
{
    public class Optional<T> : IEnumerable<T>
    {
        private IEnumerable<T> Content { get; }
        private Optional(IEnumerable<T> content)
        {
            this.Content = content;
        }


        public static Optional<T> Some(T value)
        {
            return new Optional<T>(new T[] { value });
        }

        public static Optional<T> None()
        {
            return new Optional<T>(new T[0]);
        }

        public void Do(Action<T> action)
        {
            foreach(var item in this.Content)
            {
                action(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Content.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
