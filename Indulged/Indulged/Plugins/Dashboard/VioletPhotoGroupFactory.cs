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

        public static List<PhotoGroup> GeneratePhotoGroup(List<Photo> photos)
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

                    result.Add(new PhotoGroup(group));
                    break;
                }

                for(int i = position; i < position + ranNum; i++)
                {
                    group.Add(photos[i]);
                }

                result.Add(new PhotoGroup(group));
                position += ranNum;
            }

            return result;
        }
    }
}
