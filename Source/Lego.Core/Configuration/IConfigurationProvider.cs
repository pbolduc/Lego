using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lego.Configuration
{
    public interface IConfigurationProvider<T>
    {
        T GetConfiguration();
    }
}
