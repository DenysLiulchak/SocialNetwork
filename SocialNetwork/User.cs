using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Amazon.Runtime;
using System.Collections.ObjectModel;

namespace SocialNetwork
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string _id;

        [BsonElement("email")]
        private string _email;

        [BsonElement("password")]
        private string _password;

        [BsonElement("firstName")]
        private string _firstName;

        [BsonElement("lastName")]
        private string _lastName;

        [BsonElement("interests")]
        private string[] _interests;

        [BsonElement("following")]
        private List<string> _following;

        [BsonIgnore]
        public string ID
        {
            get
            {
                return _id;
            }
        }

        [BsonIgnore]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }

        [BsonIgnore]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        [BsonIgnore]
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }

        [BsonIgnore]
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }

        [BsonIgnore]
        public IEnumerable<string> Following
        {
            get 
            {
                return _following;
            }
        }

        public User(string email = "",
            string password = "",
            string firstName = "",
            string lastName = "",
            string id = "")
        {
            _id = id;
            _email = email;
            _password = password;
            _firstName = firstName;
            _lastName = lastName;
            _interests = new string[0];
            _following = new List<string>();
        }

        public User(string[] interests,
            List<string> following,
            string email = "",
            string password = "",
            string firstName = "",
            string lastName = "",
            string id = "") : this(id, email, password, firstName, lastName)
        {
            int interestsLen = interests.Length;
            _interests = new string[interestsLen];

            for (int i = 0; i < interestsLen; ++i)
                _interests[i] = interests[i];

            _following = new List<string>(following);
        }

        public bool Follow(string? userID)
        {
            if (userID == null)
                throw new ArgumentNullException("Немає ID користувача!");

            if (userID == _id)
                throw new ArgumentException("Не можна стежити за своїм акаунтом!");

            if (_following.Contains(userID))
                return false;

            _following.Add(userID);

            return true;
        }
        public bool Unfollow(string? userID)
        {
            if (userID == null)
                return false;

            return _following.Remove(userID);
        }

        public override string ToString()
        {
            return $"{_firstName} {_lastName, -50} {_id}";
        }
    }
}
