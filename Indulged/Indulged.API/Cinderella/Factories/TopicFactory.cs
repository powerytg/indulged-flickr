using Indulged.API.Cinderella.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indulged.API.Utils;

namespace Indulged.API.Cinderella.Factories
{
    public class TopicFactory
    {
        public static Topic TopicWithJObject(JObject json, FlickrGroup group)
        {
            // Get topic id
            string topicId = json["id"].ToString();
            Topic topic = null;
            if (group.TopicCache.ContainsKey(topicId))
            {
                topic = group.TopicCache[topicId];
            }
            else
            {
                topic = new Topic();
                topic.ResourceId = topicId;
                group.TopicCache[topicId] = topic;
            }


            // Parse user
            topic.Author = UserFactory.UserWithTopicJObject(json);
            topic.IsAdmin = (json["role"].ToString() == "admin");
            topic.CreationDate = json["datecreate"].ToString().ToDateTime();

            // Subject
            topic.Subject = json["subject"].ToString();

            // Message
            topic.Message = json["message"]["_content"].ToString();

            // Replies
            topic.CanReply = (json["can_reply"].ToString() == "1");
            topic.LastReplyDate = json["datelastpost"].ToString().ToDateTime();
            topic.ReplyCount = int.Parse(json["count_replies"].ToString());

            return topic;
        }
    }
}
