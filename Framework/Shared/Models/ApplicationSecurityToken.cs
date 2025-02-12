﻿using System.Security.Claims;

namespace Shared.Models
{
    public class ApplicationSecurityToken
    {
        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
        public Claim[] Claims   { get; set; }
    }
}