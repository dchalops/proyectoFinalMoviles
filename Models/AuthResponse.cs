using System;
using System.Collections.Generic;

namespace proyectoFinalMoviles.Models
{
    public class Logo
    {
        public LogoDetails _100x100 { get; set; }
        public LogoDetails _400x400 { get; set; }
        public LogoDetails _700x700 { get; set; }
    }

    public class LogoDetails
    {
        public string path { get; set; }
        public string bucket { get; set; }
        public string hostname { get; set; }
    }

    public class Attributes
    {
        public List<Phone> phone { get; set; }
        public Location location { get; set; }
        public string companyPublicId { get; set; }
    }

    public class Phone
    {
        public string office { get; set; }
    }

    public class Location
    {
        public string city { get; set; }
        public string address { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string zipCode { get; set; }
    }

    public class Storage
    {
        public string accessKey { get; set; }
        public string projectId { get; set; }
        public string bucketName { get; set; }
        public string storageName { get; set; }
        public string secretAccessKey { get; set; }
    }

    public class Company
    {
        public string id { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public Logo logo { get; set; }
        public string state { get; set; }
        public Attributes attributes { get; set; }
        public Storage storage { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class AuthResponse
    {
        public int status { get; set; }
        public string token { get; set; }
        public List<Company> companies { get; set; }
        public string redirectUrl { get; set; }
        public string message { get; set; }
    }
}
