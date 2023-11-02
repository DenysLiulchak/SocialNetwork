using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _id;

        [BsonElement("authorID")]
        private string _authorID;

        [BsonElement("dateTime")]
        private DateTime _dateTime;

        [BsonElement("title")]
        private string? _title;

        [BsonElement("body")]  
        private string _body;

        [BsonElement("likes")]
        private List<string> _likes;

        [BsonIgnore]
        public string ID
        {
            get
            {
                return _id;
            }
        }

        [BsonIgnore]
        public string AuthorID
        {
            get
            {
                return _authorID;
            }
        }

        [BsonIgnore]
        public DateTime DateTime
        {
            get
            {
                return _dateTime;
            }
        }

        [BsonIgnore]
        public string Title
        {
            get
            {
                return _title ?? "";
            }
            set
            {
                _title = value;
            }
        }

        [BsonIgnore]
        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        public Post(in DateTime dateTime,
            string? title = "",
            string body = "",
            string authorID = "",
            string id = "") 
        {
            _dateTime = dateTime;
            _id = id;
            _authorID = authorID;
            _title = title;
            _body = body;
            _likes = new List<string>();
        }

        public override string ToString()
        {
            return $"{_title ?? "", 75}\n{_body}\n\t{_likes.Count} {'♥', -25} {_dateTime, -40} {_id}";
        }
    }
}
