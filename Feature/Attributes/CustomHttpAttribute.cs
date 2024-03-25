using BasarSoftTask3_API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace BasarSoftTask3_API.Feature.Attributes
{
    /*Burada custom bir attribute yapısı kullandım. ActionFilterAttribute yapısı http isteklerini yakalamak ve işlemek icin kullanılan bir 
     attribute sınıfıdır. Bu yuzden attribute olmasını istedigim sınıfı ActionFilterAttribute yapısından kalıtıyorum.*/
    public class CustomHttpAttribute : ActionFilterAttribute
    {
        public string Permission { get; set; } = "";
        //public CustomHttpAttribute(string permission)
        //{
        //    Permission = permission;
        //}

        public CustomHttpAttribute(string Permission)
        {
            this.Permission = Permission;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            /*Bu metot ise ilgili sınıfın sanal ve ozellestirilebilir metodudur. Her httpisteginden once calısan bu metot ilgili isteklerde 
             bazı önişlemler icin kullanılıyor. Ben her istegin icerisine token vermistim. Burada her istekten once token i kontrol etmek istiyorum.*/

            string ?token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").TrimStart().TrimEnd();
            //token elde edildi.

            if (string.IsNullOrEmpty(token))
            {//eger bir token yoksa 
                context.Result = new UnauthorizedResult(); //istek yapan kisinin bilgiye erisim hakkı yok. Bu yuzden login olmalıdır. Istegin icine hata veriyorum.
                return;//Attribute u sonlandırıyorum.
            }
            bool isJwtValid = TokenHandler.JwtValidator(token);//Eger bir token gelmis ise bu token i ilgili fonksiyonda kontrol ediyorum.
            if (!isJwtValid)//token gecersiz ise
            {
                context.Result = new UnauthorizedResult();//istegin icerisine hata gömüyorum.
                return;//akışı sonlandırıyorum.
            }
            
            string role= TokenHandler.GetRole(token);

            //int a = (int)(role == "User" ? UserRole.User : role == "Admin" ? UserRole.Admin :role=="SuperAdmin"? UserRole.SuperAdmin : 1);
            UserRole userRole;
            if (role == "User")
                userRole = UserRole.User;
            else if (role == "Admin")
                userRole = UserRole.Admin;
            else if (role == "SuperAdmin")
                userRole = UserRole.SuperAdmin;
            else
                userRole = UserRole.User; // Varsayılan olarak User atanabilir

            UserRole permission;
            if (Permission == "User")
                permission = UserRole.User;
            else if (Permission == "Admin")
                permission = UserRole.Admin;
            else if (Permission == "SuperAdmin")
                permission = UserRole.SuperAdmin;
            else
                permission = UserRole.User; // Varsayılan olarak User atanabilir

            if (userRole< permission)
            {
                context.Result = new BadRequestObjectResult("Yetkisiz İşlem");
            }

            base.OnActionExecuting(context);//Bu ise baska attribute lerin ön işlem metotlarının çagrılması icin yazılmıştır.
        }
    }

    public enum UserRole
    {
        User = 1,
        Admin = 2,
        SuperAdmin = 3
    }
}

