using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Cinderella.Models;

namespace Indulged.Plugins.Dashboard
{
    public class VioletPhotoGroupFactory
    {
        // Random generator
        private static Random randomGenerator = new Random();

        public static List<PhotoGroup> GeneratePhotoGroup(List<Photo> photos, string context = null, string contextType = null)
        {
            List<PhotoGroup> result = new List<PhotoGroup>();

            // Randomly slice the photo into groups
            int min = 1;
            int max = 6;
            int position = 0;
            bool headlineProcessed = false;

            while (position < photos.Count)
            {
                if (!headlineProcessed)
                {
                    headlineProcessed = true;

                    PhotoGroup headGroup = new PhotoGroup();
                    headGroup.context = context;
                    headGroup.contextType = contextType;
                    headGroup.IsHeadline = true;
                    headGroup.Photos.Add(photos[0]);
                    result.Add(headGroup);
                    position = 1;
                    continue;
                }

                int ranNum = randomGenerator.Next(min, max);
                List<Photo> group = new List<Photo>();

                if (position + ranNum >= photos.Count)
                {
                    for (int i = position; i < photos.Count; i++)
                    {
                        group.Add(photos[i]);
                    }

                    result.Add(new PhotoGroup(group, context, contextType));
                    break;
                }

                for(int i = position; i < position + ranNum; i++)
                {
                    group.Add(photos[i]);
                }

                result.Add(new PhotoGroup(group, context, contextType));
                position += ranNum;
            }

            return result;
        }
    }
}
