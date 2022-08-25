using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MlemApi
{
    /// <summary>
    /// Interface for class witch serialize input values to json string
    /// </summary>
    public interface IRequestValueSerializer
    {
        /// <summary>
        /// Serialize input values to list of json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        IEnumerable<string> Serialize<T>(IEnumerable<T> values);
    }
}
