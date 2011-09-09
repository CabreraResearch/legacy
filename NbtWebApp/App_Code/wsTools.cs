using System.Web.Script.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.WebServices
{

    public class wsTools : System.Web.Services.WebService
    {
        private static char _Delimiter = '_';
        public wsTools( char Delimiter )
        {
            _Delimiter = Delimiter;
        } //ctor

        public wsTools()
        {

        } //ctor

		public static CswNbtNode getNode( CswNbtResources CswNbtResources, string NodeId, string NodeKey, CswDateTime Date )
		{
			CswNbtNode Node = null;
			if( !string.IsNullOrEmpty( NodeKey ) )
			{
				CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( CswNbtResources, FromSafeJavaScriptParam( NodeKey ) );
				Node = CswNbtResources.getNode( RealNodeKey, Date.ToDateTime() );
			}
			else if( !string.IsNullOrEmpty( NodeId ) )
			{
				CswPrimaryKey RealNodeId = new CswPrimaryKey();
				if( CswTools.IsInteger( NodeId ) )
				{
					RealNodeId.TableName = "nodes";
					RealNodeId.PrimaryKey = CswConvert.ToInt32( NodeId );
				}
				else
				{
					RealNodeId.FromString( NodeId );
				}
				Node = CswNbtResources.getNode( RealNodeId, Date.ToDateTime() );
			}
			return Node;
		} // getNode()

        #region Conversion

        /// <summary>
        /// Serializes a string into a safe JavaScript string 
        /// </summary>
        public static string ToSafeJavaScriptParam( object Param )
        {
            string ParamText = CswConvert.ToString( Param );
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            string ReturnText = Serializer.Serialize( ParamText );
            return ReturnText;
        }

        /// <summary>
        /// Deserializes a string into from a safe JavaScript string 
        /// </summary>
        public static string FromSafeJavaScriptParam( object Param )
        {
            string ParamText = CswConvert.ToString( Param );
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            string ReturnText = Serializer.Deserialize<string>( ParamText );
            return ReturnText;
        }

        #endregion

        #region Client

        public static string makeId( string Prefix, string ID, string Suffix )
        {
            CswDelimitedString ElementId = new CswDelimitedString( _Delimiter ) { { Prefix, false }, { ID, false }, { Suffix, false } };

            return ElementId.ToString( false );
        }
        #endregion
    }//wsNBT

} // namespace ChemSW.WebServices
