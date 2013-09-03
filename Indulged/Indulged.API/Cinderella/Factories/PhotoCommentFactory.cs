using Indulged.API.Utils;
using Indulged.API.Cinderella.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Factories
{
    public class PhotoCommentFactory
    {
        public static PhotoComment PhotoCommentWithJObject(JObject json, Photo photo)
        {
            // Get comment id
            string commentId = json["id"].ToString();
            PhotoComment comment = null;
            if (photo.CommentCache.ContainsKey(commentId))
            {
                comment = photo.CommentCache[commentId];
            }
            else
            {
                comment = new PhotoComment();
                comment.ResourceId = commentId;
                photo.CommentCache[commentId] = comment;
            }


            // Parse user
            comment.Author = UserFactory.UserWithPhotoCommentJObject(json);
            comment.CreationDate = json["datecreate"].ToString().ToDateTime();

            // Content
            comment.Message = json["_content"].ToString();

            return comment;
        }
    }
}
