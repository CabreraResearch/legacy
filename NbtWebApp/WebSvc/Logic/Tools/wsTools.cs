using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.WebServices
{

    public class wsTools //: System.Web.Services.WebService
    {
        private CswNbtResources _CswNbtResources;

        private static char _Delimiter = '_';
        public wsTools( CswNbtResources CswNbtResources, char Delimiter = '_' )
        {
            _CswNbtResources = CswNbtResources;
            _Delimiter = Delimiter;
        } //ctor

        #region Nodes

        public static CswNbtNode getNode( CswNbtResources CswNbtResources, string NodeId, string NodeKey, CswDateTime Date )
        {
            return CswNbtResources.Nodes.GetNode( NodeId, NodeKey, Date );
        } // getNode()

        /// <summary>
        /// Detertimes if a Node Type name exists.
        /// </summary>
        /// <returns>True if the node type name is unique (available for use).</returns>
        public static bool isNodeTypeNameUnique( string NodeTypeName, CswNbtResources CswNbtResources, bool ThrowOnError )
        {
            bool Ret = false;
            if( true == ThrowOnError &&
                    string.IsNullOrEmpty( NodeTypeName ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Name is required.", "Node Type name cannot be null" );
            }

            Ret = ( null == CswNbtResources.MetaData.getNodeType( NodeTypeName ) );

            if( true == ThrowOnError &&
                    false == Ret )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "The provided name is not unique.", "A NodeType with the name " + NodeTypeName + " already exists." );
            }
            return Ret;
        }

        #endregion

        #region CSV

        public static string DataTableToCSV( DataTable DT )
        {
            StringBuilder ret = new StringBuilder();

            // Headers
            Int32 idx = 0;
            foreach( DataColumn dc in DT.Columns )
            {
                if( idx > 0 ) ret.Append( "," );
                ret.Append( "\"" + _csvSafe( dc.ColumnName.ToString() ) + "\"" );
                idx++;
            }
            ret.Append( "\r\n" );

            // Rows
            foreach( DataRow dr in DT.Rows )
            {
                idx = 0;
                foreach( DataColumn dc in DT.Columns )
                {
                    if( idx > 0 ) ret.Append( "," );
                    ret.Append( "\"" + _csvSafe( dr[dc].ToString() ) + "\"" );
                    idx++;
                }
                ret.Append( "\r\n" );
            }
            return ret.ToString();
        }

        public static void ReturnCSV( HttpContext Context, DataTable DT )
        {
            Context.Response.ClearContent();

            Context.Response.Write( DataTableToCSV( DT ) );

            Context.Response.AddHeader( "Content-Disposition", "attachment; filename=export.csv;" );
            Context.Response.End();
        }

        // Need to double " in string values
        private static string _csvSafe( string str )
        {
            return str.Replace( "\"", "\"\"" );
        }

        public static Stream ReturnCSVStream( DataTable DT )
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter sw = new StreamWriter( stream );

            sw.Write( DataTableToCSV( DT ) );

            //sw.Flush();
            stream.Position = 0;
            return stream;
        }

        #endregion

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
            string ReturnText = CswConvert.ToString( Param );
            try
            {
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                ReturnText = Serializer.Deserialize<string>( ReturnText );
            }
            catch( Exception ) { }
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
