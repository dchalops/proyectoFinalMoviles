using System;
using System.Collections.Generic;

namespace proyectoFinalMoviles.Models
{
    public class AuthResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public string redirect_url { get; set; }
        public SessionData session_data { get; set; }
        public string token { get; set; }
        public List<Company> companies { get; set; }
    }

    public class SessionData
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public bool Superuser { get; set; }
        public int SessionExpiry { get; set; }
        public int NumberOfCompanies { get; set; }
        public string UserId { get; set; }
        public string Avatar { get; set; }
    }

    public class Company
    {
        public string name { get; set; }
        public string key { get; set; }
        public Logo logo { get; set; }
        public string id { get; set; }
        public string state { get; set; }
        public Storage storage { get; set; }
        public Attributes attributes { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Logo
    {
        public LogoResolution _400x400 { get; set; }
        public LogoResolution _100x100 { get; set; }
        public LogoResolution _700x700 { get; set; }
    }

    public class LogoResolution
    {
        public string path { get; set; }
        public string hostname { get; set; }
        public string bucket { get; set; }
    }

    public class Storage
    {
        public string access_key { get; set; }
        public string storage_name { get; set; }
        public string project_id { get; set; }
        public string bucket_name { get; set; }
        public string secret_access_key { get; set; }
    }

    public class Attributes
    {
        public string company_puclic_id { get; set; }
        public Location location { get; set; }
        public List<Phone> phone { get; set; }
    }

    public class Location
    {
        public string address { get; set; }
        public string country { get; set; }
        public string zip_code { get; set; }
        public string city { get; set; }
        public string province { get; set; }
    }

    public class Phone
    {
        public string office { get; set; }
    }
}
