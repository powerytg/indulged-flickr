using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Common.PhotoGroupRenderers
{
    public class CommonPhotoGroupFactory
    {
        public string Context { get; set; }
        public string ContextType { get; set; }

        // Random generator
        private Random randomGenerator = new Random();

        public PhotoGroup GenerateHeadlinePhotoGroup(Photo photo)
        {
            PhotoGroup group = new PhotoGroup();
            group.IsHeadline = true;
            group.Photos = new List<Photo> { photo };
            group.context = Context;
            group.contextType = ContextType;

            return group;
        }

        public List<PhotoGroup> GeneratePhotoGroups(List<Photo> photos)
        {
            List<PhotoGroup> result = new List<PhotoGroup>();

            // Randomly slice the photo into groups
            int min = 1;
            int max = 4;
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

                    result.Add(new PhotoGroup(group, Context, ContextType));
                    break;
                }

                for (int i = position; i < position + ranNum; i++)
                {
                    group.Add(photos[i]);
                }

                result.Add(new PhotoGroup(group, Context, ContextType));
                position += ranNum;
            }

            return result;
        }

        public List<PhotoGroup> GeneratePhotoGroupsWithHeadline(List<Photo> photos)
        {
            if (photos.Count == 0)
            {
                return new List<PhotoGroup>();
            }

            List<PhotoGroup> results = new List<PhotoGroup>();
            PhotoGroup headlineGroup = GenerateHeadlinePhotoGroup(photos[0]);
            results.Add(headlineGroup);

            if (photos.Count > 1)
            {
                List<Photo> otherPhotos = new List<Photo>();
                otherPhotos.AddRange(photos);
                otherPhotos.RemoveAt(0);

                List<PhotoGroup> groups = GeneratePhotoGroups(otherPhotos);
                results.AddRange(groups);
            }

            return results;            
        }
    }
}
