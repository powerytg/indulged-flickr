using Indulged.API.Cinderella.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Factories
{
    public class FlickrGroupFactory
    {
        public static FlickrGroup GroupWithJObject(JObject json)
        {
            FlickrGroup group = null;
            string groupId = json["nsid"].ToString();
            if (Cinderella.CinderellaCore.GroupCache.ContainsKey(groupId))
                group = Cinderella.CinderellaCore.GroupCache[groupId];
            else
            {
                group = new FlickrGroup();
                group.ResourceId = groupId;
                Cinderella.CinderellaCore.GroupCache[group.ResourceId] = group;
            }

            group.Farm = json["iconfarm"].ToString();
            group.Server = json["iconserver"].ToString();

            JToken nameValue;
            if (json.TryGetValue("name", out nameValue))
            {
                if (nameValue.GetType() == typeof(JValue))
                {
                    group.Name = json["name"].ToString();
                }
                else
                {
                    group.Name = json["name"]["_content"].ToString();
                }
               
            }

            JToken descValue;
            if (json.TryGetValue("description", out descValue))
            {
                if(descValue.GetType() == typeof(JValue))
                {
                    group.Description = json["description"].ToString();
                }
                else
                {
                    group.Description = json["description"]["_content"].ToString();
                }
                
            }

            JToken rulesValue;
            if (json.TryGetValue("rules", out rulesValue))
            {
                group.Rules = json["rules"]["_content"].ToString();
            }

            JToken poolValue;
            if (json.TryGetValue("pool_count", out poolValue))
            {
                if(poolValue.GetType() == typeof(JValue))
                    group.PhotoCount = int.Parse(json["pool_count"].ToString());
                else
                    group.PhotoCount = int.Parse(json["pool_count"]["_content"].ToString());
            }

            JToken topicValue;
            if (json.TryGetValue("topic_count", out topicValue))
            {
                if (topicValue.GetType() == typeof(JValue))
                    group.TopicCount = int.Parse(json["topic_count"].ToString());
                else
                    group.TopicCount = int.Parse(json["topic_count"]["_content"].ToString());
            }

            JToken membersValue;
            if (json.TryGetValue("members", out membersValue))
            {
                if (membersValue.GetType() == typeof(JValue))
                    group.MemberCount = int.Parse(json["members"].ToString());

                else
                    group.MemberCount = int.Parse(json["members"]["_content"].ToString());
            }

            JToken privacyValue;
            if (json.TryGetValue("privacy", out privacyValue))
            {
                int privacyId = int.Parse(json["privacy"]["_content"].ToString());
                group.Privacy = (FlickrGroupPrivicy)privacyId;
            }


            return group;
        }
    }
}
