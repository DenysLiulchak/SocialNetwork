using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SocialNetwork
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _id;

        [BsonElement("body")]
        private string _body;

        [BsonElement("authorID")]
        private string _authorID;

        [BsonElement("postID")]
        private string _postID;

        [BsonElement("dateTime")]
        private DateTime _dateTime;

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

        [BsonIgnore]
        public string AuthorID
        {
            get
            {
                return _authorID;
            }
        }

        [BsonIgnore]
        public string PostID
        {
            get
            {
                return _postID;
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

        public Comment(in DateTime dateTime,
            string body = "",
            string authorID = "",
            string postID = "",
            string id = "")
        {
            _id = id;
            _authorID = authorID;
            _postID = postID;
            _body = body;
            _dateTime = dateTime;
            _likes = new List<string>();
        }

        public override string ToString()
        {
            return $"{_body}\n\t{_likes.Count} ♥";
        }
    }
}
