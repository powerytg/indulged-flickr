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

        public static List<List<Photo>> GeneratePhotoGroup(List<Photo> photos)
        {
            List<List<Photo>> result = new List<List<Photo>>();

            // Randomly slice the photo into groups
            int min = 3;
            int max = 6;
            int position = 0;

            while (position < photos.Count)
            {
                int ranNum = randomGenerator.Next(min, max);
                List<Photo> group = new List<Photo>();

                if (position + ranNum >= photos.Count)
                {
                    for (int i = position; i < photos.Count; i++)
                    {
                        group.Add(photos[i]);
                    }

                    result.Add(group);
                    break;
                }

                for(int i = position; i < position + ranNum; i++)
                {
                    group.Add(photos[i]);
                }

                result.Add(group);
                position += ranNum;
            }

            return result;
        }
    }
}
