﻿namespace Shared.DTO
{
    public class ValidationErrorResponse
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public int Status { get; set; }

        public string TraceId { get; set; }

        public Dictionary<string, List<string>> Errors { get; set; }
    }
}