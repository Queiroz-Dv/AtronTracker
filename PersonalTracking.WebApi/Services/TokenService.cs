namespace PersonalTracking.WebApi.Services
{
    public static class TokenService
    {
        //public static string GeneratToken(User user)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    var key = Encoding.ASCII.GetBytes(Settings.Secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor //descricao do token
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, user.UserId.ToString()),
        //            new Claim(ClaimTypes.Role, user.UsersInRoles.ToString())
        //        }),

        //        Expires = DateTime.UtcNow.AddHours(2),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
        //        SecurityAlgorithms.HmacSha256Signature)

        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
    }
}
