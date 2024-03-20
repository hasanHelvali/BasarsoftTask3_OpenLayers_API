using BasarSoftTask3_API.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using RTools_NTS.Util;

namespace BasarSoftTask3_API.Services
{
    public static class TokenHandler
    {
        //private static readonly IConfiguration _configuration;

        public static Entities.Token GenerateToken(IConfiguration configuration,  string userName, string userId,string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userName),
                new Claim(ClaimTypes.Role, role),
            };
            /*Claim ler ilgili sifrenin icerisinde tasınan veri parcalarıdır. Ben json olarak tasınacak sifrenin icerisinde bu bilgilerin olmasını istiyorum.*/
            Entities.Token token = new Entities.Token();//kendi yazdıgım token nesnemi bellege cıkarıyorum.
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]));
            /*Jwt ler simetrik sifrelerdir. Yani veriyi sifrelerken ve cozerken kullanılan anahtar aynıdır. Bu sebeple ben kendi dıs kaynaktan anahtarımı
             byte bir array olarak sifreleyip bir nesne üzerine alıyorum.*/
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            /*Burası ise ilgili jwt nin imzasının oluşturuldugu kısımdır. Verilen algritma bilgisi, verinin hangi formatla sifrelenecegini belirler.*/
            token.Expiration = DateTime.Now.AddHours(Convert.ToInt16(configuration["Token:Expiration"]));
            /*Her token in bir gecerlilik süresi olabilir. Bu sebeple olusturulacak jwt token icin bir gecerlilk süresi belirliyorum. Bu veriyi mevcut 
             * zamana eklıyorum.*/

            JwtSecurityToken jwtSecurityToken = new(
                issuer: configuration["Token:Issuer"],//Issuer ozelligi token i olusturan tarafı temsil eder.
                audience: configuration["Token:Audience"],//audience ozelligi token in hedefini temsil eder. 
                expires: token.Expiration,//expires ozelligi token in gecrlilik süresini belirler. Az once bunu tanımlamıstık.
                notBefore: DateTime.Now,//token in kullanılabilecegi anı, zamanı belirler.
                signingCredentials: signingCredentials,//signingCredential az once bahsedildigi gibi imzadır. 
                claims: claims//claims ler ise token a gizlenecek verilerdir. 
            );//Token olusturuldu. Yukarıdaki tum parametreler token in icerisine gomulur. Bu konfigurasyonlar sayısı token in guvenligi ile dogru orantılıdır.

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            //Bu sınıf ve alınan bu ornek jwt leri islemek ve jwt lere ozellik kazandırmak icin bize bazı ozellikler, fonksiyonlar saglar.
            token.AccessToken = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);//ilgili nesnenin writeToken fonksiyonu ile artık jwt nin cıktısını alıyoruz.
            
            //Refresh token 

            byte[] numbers = new byte[32];//32 boyutlu bir byte dizisi tanımlandı.
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();//Random Sayılar ureten bir nesne disposable olarak olusturuldu.
            randomNumberGenerator.GetBytes(numbers);//Dizi bu nesnenin uygun fonksiyonuna verildi ve dolduruldu.
            token.RefreshToken = Convert.ToBase64String(numbers);//burada belirli bir byte dizisi base64 formatına donusturuldu. Bu refreshToken in kendisi oldu.
            //Yani refreshToken i da sifreliyoruz.
            //Refresh token
            return token;//token i fırından sıcak sıcak geri donduruyoruz.
        }

        public static bool JwtValidator(string token)//Burası gelen accessToken ların gecerliligini kontrol ettigim fonksiyonumdur.
        {
            var tokenHandler = new JwtSecurityTokenHandler();//Jwt yi islemek icin yeni bir nesne olusturdum.
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, //token in bir IssuerSigninKey i olamılıdır diyorum.
                //IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["Token:SecurityKey"])),
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                //Burada verilen ozellikler token dogrulaması yapılırken bu ozelliklerin de dikkate alınıp alınmaması gerektigini belirtir.

                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"])),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Sebepsiz Boş Yere Ayrılacaksan")),
                ClockSkew = TimeSpan.Zero,//Token in dogrulanma asamasında gözardı edilecek saat farkını belirler. Yani token a fazladan gecerlilik kazanması degildir.
                ValidIssuer = "https://localhost",
                ValidAudience = "https://localhost",//denetleneip denetlenmeyecegini sorar. Denetlensin diyorum. Bu degerlerle denetle diyorum.
            };
            /*Jwt dogrulama islemlerinde bu jwt nin dogrulugunu anlayabilmek icin bazı parametrelerle TokenValidationParameters nesnesini yapılandırmamız 
             gerekir. */

            try
            {/*Burası token in dogrulanma asamasıdır. Bu asamada hata fırlatma potansiyeli olan kodlar kullanıyoruz. Bu sebeple try blogu icinde calısıyoruz.*/
                SecurityToken validatedToken; //ValidateToken fonksiyonunun out parametresi ile  dondurecegi gecerli token i saklamak icin bir degisken olusturuldu.
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                /*jwt nin islenecegi nesnenin, jwt nin dogrulugunu veren fonksiyonu cagırıldı. Parametre olarak, jwt nin kendisi, validasyon parametreleri verildi
                ve out ile gecerli token i üzerinde tutması istenen nesne  verildi. 
                Bu asamada token dogrulanırsa out uzerine alınır ve validatedToken a verilir. Burada bir hata yoksa true donmek dogaldır.
                Lakin token dogrulama asamasında validatedToken nesnesine dogrulanmıs token atanmaz ise false deger atanır. Bu bir hata fırlatılmasına nede olur. 
                Bu durumda catch bloguna duseriz.*/
                return true;//true deger donulur.
            }
            catch (Exception)
            {
                // Token in validasyonunda hata cıktı yani token gecersizdir.
                return false;//false deger donulur.
            }
        }
    }
}
