using System.Web.Script.Serialization;

namespace ChemSW.Nbt.WebServices
{

	public class wsTools : System.Web.Services.WebService
	{
		#region Conversion

        /// <summary>
        /// Serializes a string into a safe JavaScript string 
        /// </summary>
        public static string ToSafeJavaScriptParam( string ParamText )
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            string ReturnText = Serializer.Serialize(ParamText);
            return ReturnText;
        }

        /// <summary>
        /// Deserializes a string into from a safe JavaScript string 
        /// </summary>
        public static string FromSafeJavaScriptParam( string ParamText )
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            string ReturnText = Serializer.Deserialize<string>( ParamText );
            return ReturnText;
        }

        #endregion

    }//wsNBT

} // namespace ChemSW.WebServices
