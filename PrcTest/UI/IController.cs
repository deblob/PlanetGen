using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrcTest.UI
{
    public interface IController<T>
    {
        T Value { get; set; }
    }
}
