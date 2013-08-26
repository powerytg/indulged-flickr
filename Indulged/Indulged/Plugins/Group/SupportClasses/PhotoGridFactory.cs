using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Group.SupportClasses
{
    public class PhotoGridFactory
    {
        public static List<PhotoGroup> GeneratePhotoGroup(List<Photo> photos, string context = null, string contextType = null)
        {
            List<PhotoGroup> result = new List<PhotoGroup>();
            int position = 0;
            int rowCount = 5;

            while (position < photos.Count)
            {
                List<Photo> group = new List<Photo>();

                if (position + rowCount >= photos.Count)
                {
                    for (int i = position; i < photos.Count; i++)
                    {
                        group.Add(photos[i]);
                    }

                    result.Add(new PhotoGroup(group, context, contextType));
                    break;
                }

                for (int i = position; i < position + rowCount; i++)
                {
                    group.Add(photos[i]);
                }

                result.Add(new PhotoGroup(group, context, contextType));
                position += rowCount;
            }

            return result;
        }
    }
}
