using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace Indulged.API.Cinderella.Models
{
    public class ModelBase
    {
        public string ResourceId { get; set; }
        /*
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ModelBase))
                return false;

            ModelBase other = (ModelBase)obj;
            return (ResourceId == other.ResourceId);
        }

        public override int GetHashCode()
        {
            if (ResourceId != null)
                return ResourceId.GetHashCode();

            return base.GetHashCode();
        }
         * */
    }
}
