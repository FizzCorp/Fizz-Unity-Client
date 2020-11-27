using Fizz.Common.Json;

namespace Fizz.Chat.Impl 
{
    public class FizzJsonGroup: IFizzGroup 
    {
        private static readonly string KEY_ID = "id";
        private static readonly string KEY_CHANNEL_ID = "channel_id";
        public static readonly string KEY_TITLE = "title";
        public static readonly string KEY_IMAGE_URL = "image_url";
        public static readonly string KEY_DESCRIPTION = "description";
        public static readonly string KEY_TYPE = "type";
        public static readonly string KEY_MEMBERS = "members";
        private static readonly string KEY_CREATED = "created";
        private static readonly string KEY_UPDATED = "updated";

        public FizzJsonGroup(string json): this(JSONNode.Parse(json))
        {
        }

        public FizzJsonGroup(JSONNode json)
        {
            Id = json[KEY_ID];
            ChannelId = json[KEY_CHANNEL_ID];
            Title = json[KEY_TITLE];
            ImageURL = json[KEY_IMAGE_URL];
            Description = json[KEY_DESCRIPTION];
            Type = json[KEY_TYPE];
            Created = (long)json[KEY_CREATED].AsDouble;
            Updated = (long)json[KEY_UPDATED].AsDouble;
        }

        public string Id { get; private set; }
        public string ChannelId { get; private set; }
        public string CreatedBy { get; private set; }
        public string Title { get; private set; }
        public string ImageURL { get; private set; }
        public string Description { get; private set; }
        public string Type { get; private set; }
        public long Created { get; private set; }
        public long Updated { get; private set; }
    }
}