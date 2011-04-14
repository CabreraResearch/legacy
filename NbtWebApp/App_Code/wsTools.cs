using System.Web.Script.Serialization;

namespace ChemSW.Nbt.WebServices
{

	public class wsTools : System.Web.Services.WebService
	{
        private static string _Delimiter = "_";
		public wsTools( string Delimiter)
		{
		    _Delimiter = Delimiter;
		} //ctor

        public wsTools()
        {
            
        } //ctor

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

        #region Client
        
        public static string makeId(string Prefix, string ID, string Suffix)
        {
            string ElementId = string.Empty;
            if( !string.IsNullOrEmpty( ID ) )
            {
                ElementId = ID;
            }
            if( !string.IsNullOrEmpty( ElementId ) && !string.IsNullOrEmpty( Prefix ) )
            {
                ElementId = Prefix + _Delimiter + ElementId;
            }
            if( !string.IsNullOrEmpty( ElementId ) && !string.IsNullOrEmpty( Suffix ) )
            {
                ElementId += ( _Delimiter + Suffix );
            }
            return ElementId;
        }
        #endregion
    }//wsNBT

} // namespace ChemSW.WebServices
