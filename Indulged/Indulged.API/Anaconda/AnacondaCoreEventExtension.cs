using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Anaconda.Events;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        // Granted request token from Flickr
        public EventHandler RequestTokenGranted; 

        // Access token retrieved
        public EventHandler AccessTokenGranted;

        // Access token failure
        public EventHandler AccessTokenFailed;

        // Photo set
        public EventHandler<PhotoSetListEventArgs> PhotoSetListReturned;
        public EventHandler<GetPhotoSetPhotosEventArgs> PhotoSetPhotosReturned;

        // Retrieved photo stream for an user
        public EventHandler<GetPhotoStreamEventArgs> PhotoStreamReturned;

        // Retrieved group list for an user
        public EventHandler<GetGroupListEventArgs> GroupListReturned;

        // Retrieved discovery stream
        public EventHandler<GetDiscoveryStreamEventArgs> DiscoveryStreamReturned;

        // Retrieved EXIF info
        public EventHandler<GetEXIFEventArgs> EXIFReturned;
        public EventHandler<GetEXIFExceptionEventArgs> EXIFException;

        // Photo info
        public EventHandler<GetPhotoInfoEventArgs> PhotoInfoReturned;
        public EventHandler<GetPhotoInfoExceptionEventArgs> PhotoInfoException;


        // Popular tag list returned
        public EventHandler<GetPopularTagListEventArgs> PopularTagListReturned;

        // Seach results
        public EventHandler<PhotoSearchEventArgs> PhotoSearchReturned;
        public EventHandler<GroupSearchEventArgs> GroupSearchReturned;

        // Group 
        public EventHandler<GetGroupInfoEventArgs> GroupInfoReturned;
        public EventHandler<GetGroupPhotosEventArgs> GroupPhotoReturned;
        public EventHandler<GetGroupTopicsEventArgs> GroupTopicsReturned;
        public EventHandler<AddTopicExceptionEventArgs> AddTopicException;
        public EventHandler<AddTopicEventArgs> TopicAdded;
        public EventHandler<GetTopicRepliesEventArgs> TopicRepliesReturned;

        public EventHandler<AddPhotoToGroupEventArgs> PhotoAddedToGroup;
        public EventHandler<AddPhotoToGroupExceptionEventArgs> AddPhotoToGroupException;
        public EventHandler<RemovePhotoFromGroupEventArgs> PhotoRemovedFromGroup;
        public EventHandler<RemovePhotoFromGroupExceptionEventArgs> RemovePhotoFromGroupException;

        // Upload
        public EventHandler<UploadPhotoEventArgs> PhotoUploaded;
        public EventHandler<UploadProgressEventArgs> PhotoUploadProgress;
        public EventHandler<UploadPhotoErrorEventArgs> PhotoUploadError;
    }
}
