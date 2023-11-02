namespace SocialNetwork
{
    public enum MenuKeyEnum : byte
    {
        LogIn = 1,
        ShowStream = 2,
        ShowPosts = 3,
        CreatePost = 4,
        AddComment = 5,
        AddLikePost = 6,
        RemoveLikePost = 7,
        AddLikeComment = 8,
        RemoveLikeComment = 9,
        FindUsers = 10,
        Follow = 11,
        Unfollow = 12,
        LogOut = 13,
        Exit = 0,
        NoKey = byte.MaxValue,
    }
}