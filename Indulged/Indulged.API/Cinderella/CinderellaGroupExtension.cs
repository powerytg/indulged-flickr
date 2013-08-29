using Indulged.API.Anaconda.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Cinderella.Factories;
using Newtonsoft.Json.Linq;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;
using Indulged.API.Utils;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        private void OnGroupInfoReturned(object sender, GetGroupInfoEventArgs e)
        {
            
            JObject rootJson = JObject.Parse(e.Response);
            JObject json = (JObject)rootJson["group"];
            FlickrGroup group = FlickrGroupFactory.GroupWithJObject(json);
            if (group == null)
                return;

            group.IsInfoRetrieved = true;

            var evt = new GroupInfoUpdatedEventArgs();
            evt.GroupId = group.ResourceId;

            GroupInfoUpdated.DispatchEvent(this, evt);

        }

        private void OnGroupPhotosReturned(object sender, GetGroupPhotosEventArgs e)
        {
            if (!Cinderella.CinderellaCore.GroupCache.ContainsKey(e.GroupId))
                return;

            FlickrGroup group = Cinderella.CinderellaCore.GroupCache[e.GroupId];

            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["photos"];
            int TotalCount = int.Parse(rootJson["total"].ToString());
            int page = int.Parse(rootJson["page"].ToString());
            int numPages = int.Parse(rootJson["pages"].ToString());
            int perPage = int.Parse(rootJson["perpage"].ToString());

            List<Photo> newPhotos = new List<Photo>();
            foreach (var entry in rootJson["photo"])
            {
                JObject json = (JObject)entry;
                Photo photo = PhotoFactory.PhotoWithJObject(json);

                if (!group.Photos.Contains(photo))
                {
                    group.Photos.Add(photo);
                    newPhotos.Add(photo);
                }
            }

            // Dispatch event
            GroupPhotoListUpdatedEventArgs evt = new GroupPhotoListUpdatedEventArgs();
            evt.GroupId = group.ResourceId;
            evt.Page = page;
            evt.PageCount = numPages;
            evt.PerPage = perPage;
            evt.NewPhotos = newPhotos;
            GroupPhotoListUpdated.DispatchEvent(this, evt);
        }

        private void OnGroupTopicsReturned(object sender, GetGroupTopicsEventArgs e)
        {
            if (!Cinderella.CinderellaCore.GroupCache.ContainsKey(e.GroupId))
                return;

            FlickrGroup group = Cinderella.CinderellaCore.GroupCache[e.GroupId];

            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["topics"];
            int TotalCount = int.Parse(rootJson["total"].ToString());
            int page = int.Parse(rootJson["page"].ToString());
            int numPages = int.Parse(rootJson["pages"].ToString());
            int perPage = int.Parse(rootJson["per_page"].ToString());

            List<Topic> newTopics = new List<Topic>();
            if (TotalCount > 0)
            {
                foreach (var entry in rootJson["topic"])
                {
                    JObject json = (JObject)entry;
                    Topic topic = TopicFactory.TopicWithJObject(json, group);

                    if (!group.Topics.Contains(topic))
                    {
                        group.Topics.Add(topic);
                        newTopics.Add(topic);
                    }
                }

            }

            // Dispatch event            
            GroupTopicsUpdatedEventArgs evt = new GroupTopicsUpdatedEventArgs();
            evt.GroupId = group.ResourceId;
            evt.Page = page;
            evt.PageCount = numPages;
            evt.PerPage = perPage;
            evt.NewTopics = newTopics;
            GroupTopicsUpdated.DispatchEvent(this, evt);
        }

        private void OnTopicAdded(object sender, AddTopicEventArgs e)
        {
            FlickrGroup group = Cinderella.CinderellaCore.GroupCache[e.GroupId];
            
            JObject rawJson = JObject.Parse(e.Response);
            string newTopicId = rawJson["topic"]["id"].ToString();

            Topic newTopic = new Topic();
            newTopic.ResourceId = newTopicId;
            newTopic.Subject = e.Subject;
            newTopic.Message = e.Message;
            newTopic.Author = CurrentUser;

            group.TopicCache[newTopicId] = newTopic;
            group.Topics.Insert(0, newTopic);
            group.TopicCount++;

            AddTopicCompleteEventArgs evt = new AddTopicCompleteEventArgs();
            evt.SessionId = e.SessionId;
            evt.GroupId = group.ResourceId;
            evt.newTopic = newTopic;
            AddTopicCompleted.DispatchEvent(this, evt);
        }

        private void OnPhotoAddedToGroup(object sender, AddPhotoToGroupEventArgs e)
        {
            // Update group throttle info
            FlickrGroup group = GroupCache[e.GroupId];
            Photo photo = PhotoCache[e.PhotoId];

            if (group.ThrottleMode != "none")
                group.ThrottleRemainingCount--;

            if (!group.Photos.Contains(photo))
            {
                group.Photos.Insert(0, photo);
                group.PhotoCount++;

                // Dispatch event
                AddPhotoToGroupCompleteEventArgs ae = new AddPhotoToGroupCompleteEventArgs();
                ae.PhotoId = photo.ResourceId;
                ae.GroupId = group.ResourceId;
                AddPhotoToGroupCompleted.DispatchEvent(this, ae);
            }

        }

        private void OnPhotoRemovedFromGroup(object sender, RemovePhotoFromGroupEventArgs e)
        {
            // Update group throttle info
            FlickrGroup group = GroupCache[e.GroupId];
            Photo photo = PhotoCache[e.PhotoId];

            if (group.ThrottleMode != "none")
                group.ThrottleRemainingCount++;

            if (group.Photos.Contains(photo))
            {
                group.Photos.Remove(photo);
                group.PhotoCount--;

                // Dispatch event
                RemovePhotoFromGroupCompleteEventArgs evt = new RemovePhotoFromGroupCompleteEventArgs();
                evt.PhotoId = photo.ResourceId;
                evt.GroupId = group.ResourceId;
                RemovePhotoFromGroupCompleted.DispatchEvent(this, evt);
            }

        }

        private void OnTopicRepliesReturned(object sender, GetTopicRepliesEventArgs e)
        {
            if (!Cinderella.CinderellaCore.GroupCache.ContainsKey(e.GroupId))
                return;

            FlickrGroup group = Cinderella.CinderellaCore.GroupCache[e.GroupId];

            if (!group.TopicCache.ContainsKey(e.TopicId))
                return;

            Topic topic = group.TopicCache[e.TopicId];

            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["replies"];
            JObject topicJson = (JObject)rootJson["topic"];

            int TotalCount = int.Parse(topicJson["total"].ToString());
            int page = int.Parse(topicJson["page"].ToString());
            int numPages = int.Parse(topicJson["pages"].ToString());
            int perPage = int.Parse(topicJson["per_page"].ToString());

            List<TopicReply> newReplies = new List<TopicReply>();
            if (TotalCount > 0)
            {
                foreach (var entry in rootJson["reply"])
                {
                    JObject json = (JObject)entry;
                    TopicReply reply = TopicReplyFactory.TopicReplyWithJObject(json, topic);

                    if (!topic.Replies.Contains(reply))
                    {
                        topic.Replies.Add(reply);
                        newReplies.Add(reply);
                    }
                }

            }

            // Dispatch event            
            TopicRepliesUpdatedEventArgs evt = new TopicRepliesUpdatedEventArgs();
            evt.GroupId = group.ResourceId;
            evt.TopicId = topic.ResourceId;
            evt.Page = page;
            evt.PageCount = numPages;
            evt.PerPage = perPage;
            evt.NewReplies = newReplies;
            TopicRepliesUpdated.DispatchEvent(this, evt);
        }
    }
}
