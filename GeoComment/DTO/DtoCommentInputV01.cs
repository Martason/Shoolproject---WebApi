﻿namespace GeoComment.Models
{
    public class DtoCommentInputV01
    {
        public string Message { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Author { get; set; }
        public string? Title { get; set; }
    }
}
