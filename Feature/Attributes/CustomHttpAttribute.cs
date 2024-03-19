using BasarSoftTask3_API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace BasarSoftTask3_API.Feature.Attributes
{
    /*Burada custom bir attribute yapısı kullandım. ActionFilterAttribute yapısı http isteklerini yakalamak ve işlemek icin kullanılan bir 
     attribute sınıfıdır. Bu yuzden attribute olmasını istedigim sınıfı ActionFilterAttribute yapısından kalıtıyorum.*/
    public class CustomHttpAttribute : ActionFilterAttribute
    {
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
            base.OnActionExecuting(context);//Bu ise baska attribute lerin ön işlem metotlarının çagrılması icin yazılmıştır.
        }
    }
}
