using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public static string DOMAIN = "http://64.202.189.210";
    //public static string DOMAIN = "http://localhost:7292";
    public static int screenID = 1; //1: Home, 2: Login, 3: Signup, 4: Question, 5: Categories, 6: Products(Home), 7: My Items, 8: Friend Finder, 9: Calendar, 10: Notifications, 11: Settings
    public static bool isMenuShowed = false;

    public static User m_user = null;
    public static List<Category> categoryList = new List<Category>();
    public static List<Product> productList = null;

    public static List<Product> myItemList = new List<Product>();
    public static int selectedItemIndex = -1;

    public static List<User> friendList = new List<User>();

    public static void SaveUserInfo(User user)
    {
        PlayerPrefs.SetString("user.id", user.id.ToString());
        PlayerPrefs.SetString("user.email", user.email);
        PlayerPrefs.SetString("user.password", user.password);
        PlayerPrefs.SetString("user.firstname", user.firstname);
        PlayerPrefs.SetString("user.lastname", user.lastname);
        PlayerPrefs.SetString("user.avatar", user.avatar);
        PlayerPrefs.SetString("user.question_list", user.question_list);
        PlayerPrefs.SetInt("user.is_fb_login", user.is_fb_login);
        PlayerPrefs.SetString("user.fb_user_id", user.fb_user_id.ToString());
        PlayerPrefs.SetString("user.fb_access_token", user.fb_access_token);
        PlayerPrefs.SetInt("user.is_use", user.is_use);
        PlayerPrefs.SetString("user.last_login", user.last_login.ToString());
        PlayerPrefs.SetInt("user.device_type", user.device_type);
        PlayerPrefs.SetString("user.reg_date", user.reg_date.ToString());
        PlayerPrefs.Save();
    }

    public static User GetUserInfo()
    {
        User user = new User();
        user.id = long.Parse(PlayerPrefs.GetString("user.id"));
        user.email = PlayerPrefs.GetString("user.email");
        user.password = PlayerPrefs.GetString("user.password");
        user.firstname = PlayerPrefs.GetString("user.firstname");
        user.lastname = PlayerPrefs.GetString("user.lastname");
        user.avatar = PlayerPrefs.GetString("user.avatar");
        user.question_list = PlayerPrefs.GetString("user.question_list");
        user.is_fb_login = int.Parse(PlayerPrefs.GetString("user.is_fb_login"));
        user.fb_user_id = long.Parse(PlayerPrefs.GetString("user.fb_user_id"));
        user.fb_access_token = PlayerPrefs.GetString("user.fb_access_token");
        user.is_use = int.Parse(PlayerPrefs.GetString("user.is_use"));
        user.last_login = DateTime.Parse(PlayerPrefs.GetString("user.last_login"));
        user.device_type = int.Parse(PlayerPrefs.GetString("user.device_type"));
        user.reg_date = DateTime.Parse(PlayerPrefs.GetString("user.reg_date"));

        return user;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}

public class User
{
    public long id;
    public string email;
    public string password;
    public string firstname;
    public string lastname;
    public string avatar;
    public string question_list;
    public int is_fb_login;
    public long fb_user_id;
    public string fb_access_token;
    public int is_use;
    public DateTime last_login;
    public int device_type;
    public DateTime reg_date;

    public float point_amount;

    public User() { }
}

public class Category
{
    public string id;
    public string name;
    public int market_id;

    public Category(string id, string name,int market_id)
    {
        this.id = id;
        this.name = name;
        this.market_id = market_id;
    }

    public Category() { }
}

public class Product
{
    public string product_id;
    public string title;
    public string description;
    public string image;
    public string price;
    public string url;
    public int market_id;


    public Product(string product_id, string title, string description, int market_id, string image, string price, string url)
    {
        this.product_id = product_id;
        this.title = title;
        this.description = description;
        this.price = price;
        this.image = image;
        this.url = url;
    }

    public Product() { }
}


