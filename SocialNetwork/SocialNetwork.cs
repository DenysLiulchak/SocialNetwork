using DnsClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork
{
    public class SocialNetwork
    {
        private User? _mainUser;
        private IMongoDatabase _database;
        private IMongoCollection<User> _usersCollection;
        private IMongoCollection<Post> _postsCollection;
        private IMongoCollection<Comment> _commentsCollection;

        public SocialNetwork()
        {
            const string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);

            _database = client.GetDatabase("SocialNetwork");
            _usersCollection = _database.GetCollection<User>("Users");
            _postsCollection = _database.GetCollection<Post>("Posts");
            _commentsCollection = _database.GetCollection<Comment>("Comments");
        }

        private void ShowMenu()
        {
            Console.WriteLine(
@" ________________________________________________________________
|                                                                |
|                              Меню                              |
|________________________________________________________________|

1 — Увійти
2 — Переглянути потік
3 — Переглянути пости користувача
4 — Написати пост
5 — Прокоментувати пост
6 — Лайкнути пост
7 — Видалити лайк з поста
8 — Лайкнути коментар
9 — Видалити лайк з коментаря
10 — Пошук користувачів
11 — Стежити за користувачем
12 — Перестати стежити за користувачем
13 — Вийти
0 — Закінчити роботу");
        }

        public void Run()
        {
            ShowMenu();

            string?[] input = new string?[2];
            StringBuilder stringBuilder = new StringBuilder();
            List<User> foundUsers;
            List<Post> foundPosts;
            int pageCounter;
            int postsCount;
            MenuKeyEnum key;

            while (true) 
            {
                Console.Write("\nВиберіть настпупну дію: ");

                if (!Enum.TryParse<MenuKeyEnum>(Console.ReadLine(), out key) || key > MenuKeyEnum.LogOut)
                    key = MenuKeyEnum.NoKey;

                Console.WriteLine();

                switch (key) 
                {
                    case MenuKeyEnum.LogIn:
                        Console.Write("Введіть email: ");
                        input[0] = Console.ReadLine();

                        Console.Write("Введіть пароль: ");
                        input[1] = Console.ReadLine();

                        try
                        {
                            LogIn(input[0], input[1]);
                            Console.WriteLine("Успішний вхід");

                            goto case MenuKeyEnum.ShowStream;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.ShowStream:
                        pageCounter = 0;

                        while (true)
                        {
                            try
                            {
                                foundPosts = GetStream(pageCounter);
                                postsCount = foundPosts.Count;

                                foreach (Post post in foundPosts)
                                {
                                    Console.WriteLine(post);
                                    Console.WriteLine();
                                }

                                if (postsCount != (int) PagePropertiesEnum.PostsPerPage)
                                    break;
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message);
                                break;
                            }

                            Console.WriteLine("Якщо бажаєте побачити більше постів, натисність \"Enter\"");

                            if (Console.ReadLine() != "")
                                break;

                            ++pageCounter;
                        }

                        break;

                    case MenuKeyEnum.ShowPosts:
                        Console.Write("Введть ID користувача, пости якого ви хочете переглянути: ");
                        input[0] = Console.ReadLine();

                        pageCounter = 0;

                        while (true)
                        {
                            try
                            {
                                foundPosts = FindPosts(input[0], pageCounter);
                                postsCount = foundPosts.Count;

                                Console.WriteLine($"Кількість знайдених постів — {postsCount}: ");

                                foreach (Post post in foundPosts)
                                {
                                    Console.WriteLine(post);
                                    Console.WriteLine();
                                }

                                if (postsCount != (int) PagePropertiesEnum.PostsPerPage)
                                    break;
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message);
                                break;
                            }

                            Console.WriteLine("Якщо бажаєте побачити більше постів, натисність \"Enter\"");

                            if (Console.ReadLine() != "")
                                break;

                            ++pageCounter;
                        }

                        break;

                    case MenuKeyEnum.CreatePost:
                        Console.Write("Введіть заголовок | для пустого заголовку, натисність \"Ctrl + Z, Enter\": ");
                        input[0] = Console.ReadLine();

                        Console.Write("Введіть опис поста | щоб закінчити ввід, натисність \"Ctrl + Z, Enter\": ");

                        while ((input[1] = Console.ReadLine()) != null)
                            stringBuilder.AppendLine(input[1]);

                        if (stringBuilder.Length > 0)
                            stringBuilder.Length -= 2;

                        try
                        {
                            CreatePost(input[0], stringBuilder.ToString());
                            Console.WriteLine("Пост створено");
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        stringBuilder.Clear();

                        break;

                    case MenuKeyEnum.AddComment:
                        Console.Write("Введіть ID поста, який ви хочете прокоментувати: ");
                        input[0] = Console.ReadLine();

                        Console.Write("Введіть коментар | щоб закінчити ввід, натисність \"Ctrl + Z, Enter\": ");

                        while ((input[1] = Console.ReadLine()) != null)
                            stringBuilder.AppendLine(input[1]);
                        
                        if (stringBuilder.Length > 0)
                            stringBuilder.Length -= 2;

                        try
                        {
                            AddComment(input[0], stringBuilder.ToString());
                            Console.WriteLine("Коментар додано");
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        stringBuilder.Clear();

                        break;

                    case MenuKeyEnum.AddLikePost:
                        Console.Write("Введіть ID поста, який ви хочете лайкнути: ");
                        input[0] = Console.ReadLine();

                        try
                        {
                            AddLikeToPost(input[0]);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.RemoveLikePost:
                        Console.Write("Введіть ID поста, з якого ви хочете видалити лайк: ");
                        input[0] = Console.ReadLine();

                        try
                        {
                            RemoveLikeFromPost(input[0]);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.AddLikeComment:
                        Console.Write("Введіть ID коментаря, який ви хочете лайкнути: ");
                        input[0] = Console.ReadLine();

                        try
                        {
                            AddLikeToComment(input[0]);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.RemoveLikeComment:
                        Console.Write("Введіть ID коментаря, з якого ви хочете видалити лайк: ");
                        input[0] = Console.ReadLine();

                        try
                        {
                            RemoveLikeFromComment(input[0]);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.FindUsers:
                        Console.Write("Ім'я та(або) прізвище користувача для пошуку: ");
                        input[0] = Console.ReadLine();

                        try
                        {
                            foundUsers = FindUsers(input[0]);

                            Console.WriteLine($"Кількість знайдених користувачів — {foundUsers.Count}: ");

                            foreach (User user in foundUsers)
                                Console.WriteLine(user);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.Follow:
                        Console.Write("Введіть ID користувача, за яким бажаєте слідкувати: ");
                        input[0] = Console.ReadLine();

                        try
                        {
                            Follow(input[0]);
                            Console.WriteLine($"Тепер ви слідкуєте за користувачем з ID: {input[0]}");
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.Unfollow:
                        Console.Write("Введіть ID користувача, за яким більше не бажаєте слідкувати: ");
                        input[0] = Console.ReadLine();

                        try
                        {
                            Unfollow(input[0]);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.LogOut:
                        try
                        {
                            LogOut();
                            Console.WriteLine("Успішний вихід");
                        }
                        catch (Exception exception) 
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;

                    case MenuKeyEnum.Exit:
                        return;

                    case MenuKeyEnum.NoKey:
                        Console.WriteLine("Неправильний ввід, перегляньте меню ще раз!");

                        ShowMenu();

                        break;
                }
            }
        }
        public void LogIn(string? email, string? password)
        {
            if (_mainUser != null)
                throw new Exception("Ви вже ввійшли на свій акаунт!");

            var filter = Builders<User>.Filter.Eq("email", email);

            _mainUser = _usersCollection.Find(filter)
                .Project<User>("{_id: 1, email: 1, password: 1, firstName: 1, lastName: 1, interests: 1, following: 1}")
                .FirstOrDefault();

            if (_mainUser == null)
                throw new ArgumentException("Користувача з такою електронною поштою не існує");

            if (_mainUser.Password != password) 
            {
                _mainUser = null;

                throw new ArgumentException("Неправильний пароль");
            }
        }
        public List<Post> GetStream(int pageNumber)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            var filter = Builders<Post>.Filter.In("authorID", _mainUser.Following);
            var sort = Builders<Post>.Sort.Descending("dateTime");

            return _postsCollection.Find(filter)
                .Project<Post>("{_id: 1, dateTime: 1, title: 1, body: 1, likes: 1}")
                .Sort(sort)
                .Skip(pageNumber * (int) PagePropertiesEnum.PostsPerPage)
                .Limit((int) PagePropertiesEnum.PostsPerPage)
                .ToList();
        }
        public List<Post> FindPosts(string? authorID, int pageNumber)
        {
            if (authorID == null)
                throw new ArgumentNullException("Не введено дані для пошуку");

            var filter = Builders<Post>.Filter.Eq("authorID", authorID);
            var sort = Builders<Post>.Sort.Descending("dateTime");

            return _postsCollection.Find(filter)
                .Project<Post>("{_id: 1, dateTime: 1, title: 1, body: 1, likes: 1}")
                .Sort(sort)
                .Skip(pageNumber * (int) PagePropertiesEnum.PostsPerPage)
                .Limit((int) PagePropertiesEnum.PostsPerPage)
                .ToList();
        }
        public void CreatePost(string? title, string? body)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            if (body == null || body == "")
                throw new Exception("Неможливо створити пустий пост");

            Post post = new Post(DateTime.Now, title, body, _mainUser.ID);

            _postsCollection.InsertOne(post);
        }
        public void AddComment(string? postID, string? body)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            if (postID == null || postID == "")
                throw new ArgumentException("Не введено ID");

            if (body == null || body == "")
                throw new ArgumentException("Неможливо створити пустий коментар!");

            var filter = Builders<Post>.Filter.Eq("_id", postID);
            var post = _postsCollection.Find(filter)
                .Project<Post>("{_id: 1}")
                .FirstOrDefault();

            if (post == null)
                throw new ArgumentException("Немає поста з таким ID!");

            Comment comment = new Comment(DateTime.Now, body, _mainUser.ID, postID);

            _commentsCollection.InsertOne(comment);
        }
        public void AddLikeToPost(string? postID)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            var filterBuilder = Builders<Post>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", postID),
                filterBuilder.Ne("likes", _mainUser.ID)
                );
            var update = Builders<Post>.Update.Push("likes", _mainUser.ID);

            _postsCollection.UpdateOne(filter, update);
        }
        public void RemoveLikeFromPost(string? postID)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            var filter = Builders<Post>.Filter.Eq("_id", postID);
            var update = Builders<Post>.Update.Pull("likes", _mainUser.ID);

            _postsCollection.UpdateOne(filter, update);
        }
        public void AddLikeToComment(string? commentID)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            var filterBuilder = Builders<Comment>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", commentID),
                filterBuilder.Ne("likes", _mainUser.ID)
                );
            var update = Builders<Comment>.Update.Push("likes", _mainUser.ID);

            _commentsCollection.UpdateOne(filter, update);
        }
        public void RemoveLikeFromComment(string? commentID)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            var filter = Builders<Comment>.Filter.Eq("_id", commentID);
            var update = Builders<Comment>.Update.Pull("likes", _mainUser.ID);

            _commentsCollection.UpdateOne(filter, update);
        }
        public List<User> FindUsers(string? nameAndSurname)
        {
            if (nameAndSurname == null)
                throw new ArgumentNullException("Не введено дані для пошуку");

            var filter = Builders<User>.Filter.Text(nameAndSurname);

            return _usersCollection.Find(filter)
                .Project<User>("{_id: 1, firstName: 1, lastName: 1}")
                .Limit(20)
                .ToList();
        }
        public void Follow(string? userID)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            var filterUserID = Builders<User>.Filter.Eq("_id", userID); 
            var user = _usersCollection.Find(filterUserID)
                .Project<User>("{_id: 1}")
                .FirstOrDefault();

            if (user == null)
                throw new ArgumentException("Немає користувача з таким ID!");

            if (!_mainUser.Follow(userID))
                return;

            var filter = Builders<User>.Filter.Eq("_id", _mainUser.ID);
            var update = Builders<User>.Update.Push("following", userID);

            _usersCollection.UpdateOne(filter, update);
        }
        public void Unfollow(string? userID)
        {
            if (_mainUser == null)
                throw new Exception("Спершу потрібно ввійти на свій акаунт!");

            if (!_mainUser.Unfollow(userID))
                return;

            var filter = Builders<User>.Filter.Eq("_id", _mainUser.ID);
            var update = Builders<User>.Update.Pull("following", userID);

            _usersCollection.UpdateOne(filter, update);
        }
        public void LogOut()
        {
            if (_mainUser == null)
                throw new Exception("Ви вже вийшли з акаунту!");

            _mainUser = null;
        }
    }
}
