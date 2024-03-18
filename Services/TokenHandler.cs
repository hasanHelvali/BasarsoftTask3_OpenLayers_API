using BasarSoftTask3_API.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BasarSoftTask3_API.Services
{
    public static class TokenHandler
    {
        //private readonly string _secretKey;
        //private readonly string _issuer;

        //public TokenService(string secretKey, string issuer)
        //{
        //    _secretKey = secretKey;
        //    _issuer = issuer;
        //}

        public static Token GenerateToken(IConfiguration configuration)
        {
            //var bytes= Encoding.ASCII.GetBytes("abcdefghijklmnoprstu");
            //SymmetricSecurityKey key= new SymmetricSecurityKey(bytes);
            //SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //JwtSecurityToken token = new JwtSecurityToken(issuer: "http://localhost", audience: "http://localhost",
            //    notBefore: DateTime.Now, expires: DateTime.Now.AddMinutes(1),signingCredentials:credentials);

            //JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            //return handler.WriteToken(token);


            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.Name, userId)
            //    }),
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            //    Issuer = _issuer
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);

            Token token = new Token();
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]));

            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);
            token.Expiration =   DateTime.Now.AddMinutes(Convert.ToInt16(configuration["Token:Expiration"]));

            JwtSecurityToken jwtSecurityToken = new (
                issuer:configuration["Token:Issuer"],
                audience: configuration["Token:Audience"],
                expires: token.Expiration,
                notBefore:DateTime.Now,
                signingCredentials:signingCredentials
            );
            JwtSecurityTokenHandler jwtSecurityTokenHandler=new JwtSecurityTokenHandler();
            token.AccessToken = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);


            //Refresh token 
            byte[] numbers=new byte[32];
            using RandomNumberGenerator randomNumberGenerator=RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(numbers);
            token.RefreshToken = Convert.ToBase64String(numbers);
            //Refresh token



            return token;
        }
    }
}
