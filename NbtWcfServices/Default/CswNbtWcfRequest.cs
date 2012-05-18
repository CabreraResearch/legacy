
namespace NbtWebAppServices.Response
{
    public class CswNbtWcfRequest : ICswNbtWcfRequest
    {
        public class CswNbtSessionRequest
        {
            public string Password { get; set; }
            public string CustomerId { get; set; }
            public string UserName { get; set; }
            public bool IsMobile { get; set; }
        }
    }
}