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

        public EventHandler<AddPhotoToSetEventArgs> PhotoAddedToSet;
        public EventHandler<AddPhotoToSetExceptionEventArgs> AddPhotoToSetException;
        public EventHandler<RemovePhotoFromSetEventArgs> PhotoRemovedFromSet;
        public EventHandler<RemovePhotoFromSetExceptionEventArgs> RemovePhotoFromSetException;

        // Retrieved photo stream for an user
        public EventHandler<GetPhotoStreamEventArgs> PhotoStreamReturned;

        // Retrieved favourite stream for an user
        public EventHandler<GetFavouriteStreamEventArgs> FavouriteStreamReturned;

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

        // Photo comments
        public EventHandler<GetPhotoCommentsEventArgs> PhotoCommentsReturned;
        public EventHandler<GetPhotoCommentsExceptionEventArgs> GetPhotoCommentsException;
        public EventHandler<AddCommentEventArgs> PhotoCommentAdded;
        public EventHandler<AddCommentExceptionEventArgs> AddPhotoCommentException;

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
        public EventHandler<AddTopicReplyExceptionEventArgs> AddTopicReplyException;
        public EventHandler<AddTopicReplyEventArgs> TopicReplyAdded;

        public EventHandler<AddPhotoToGroupEventArgs> PhotoAddedToGroup;
        public EventHandler<AddPhotoToGroupExceptionEventArgs> AddPhotoToGroupException;
        public EventHandler<RemovePhotoFromGroupEventArgs> PhotoRemovedFromGroup;
        public EventHandler<RemovePhotoFromGroupExceptionEventArgs> RemovePhotoFromGroupException;

        public EventHandler<JoinGroupEventArgs> GroupJoined;
        public EventHandler<JoinGroupExceptionEventArgs> JoinGroupException;
        public EventHandler<JoinGroupRequestEventArgs> JoinGroupRequestComplete;
        public EventHandler<JoinGroupRequestExceptionEventArgs> JoinGroupRequestException;

        // Upload
        public EventHandler<UploadPhotoEventArgs> PhotoUploaded;
        public EventHandler<UploadProgressEventArgs> PhotoUploadProgress;
        public EventHandler<UploadPhotoErrorEventArgs> PhotoUploadError;

        // Favourite
        public EventHandler<AddFavouriteEventArgs> AddedPhotoAsFavourite;
        public EventHandler<AddFavouriteExceptionEventArgs> AddPhotoAsFavouriteException;
        public EventHandler<RemoveFavouriteEventArgs> RemovePhotoFromFavourite;
        public EventHandler<RemoveFavouriteExceptionEventArgs> RemovePhotoFromFavouriteException;

        // User info
        public EventHandler<GetUserInfoEventArgs> UserInfoReturned;
        public EventHandler<GetContactListEventArgs> ContactListReturned;
        public EventHandler<GetContactListExceptionEventArgs> GetContactListException;
        public EventHandler<GetContactPhotosEventArgs> ContactPhotosReturned;
        public EventHandler GetContactPhotosException;

        // Activity stream
        public EventHandler<GetActivityStreamEventArgs> ActivityStreamReturned;
    }
}
