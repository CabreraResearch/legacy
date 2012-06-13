using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.WebServices
{

    public class wsTools //: System.Web.Services.WebService
    {
        private CswNbtResources _CswNbtResources;
        /// <summary>
        /// These are files we do NOT want to keep around after temporarily using them.  There is a function that purges old files.  
        /// </summary>
        private string _TempPath
        {
            get
            {
                // ApplicationPhysicalPath already has \\ at the end
                return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "temp" );
            }
        }

        public static string TempPath
        {
            get
            {
                return ( System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath + "/temp/" );
            }
        }

        private static char _Delimiter = '_';
        public wsTools( CswNbtResources CswNbtResources, char Delimiter = '_' )
        {
            _CswNbtResources = CswNbtResources;
            _Delimiter = Delimiter;
        } //ctor

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
                throw new CswDniException( ErrorType.Warning, "Name is required.", "Node Type name cannot be null" );
            }

            Ret = ( null == CswNbtResources.MetaData.getNodeType( NodeTypeName ) );

            if( true == ThrowOnError &&
                    false == Ret )
            {
                throw new CswDniException( ErrorType.Warning, "The provided name is not unique.", "A NodeType with the name " + NodeTypeName + " already exists." );
            }
            return Ret;
        }

        public static void ReturnCSV( HttpContext Context, DataTable DT )
        {
            Context.Response.ClearContent();
            //Context.Response.ContentType = "application/vnd.ms-excel";

            // Headers
            Int32 idx = 0;
            foreach( DataColumn dc in DT.Columns )
            {
                if( idx > 0 ) Context.Response.Write( "," );
                Context.Response.Write( "\"" + _csvSafe( dc.ColumnName.ToString() ) + "\"" );
                idx++;
            }
            Context.Response.Write( "\r\n" );

            // Rows
            foreach( DataRow dr in DT.Rows )
            {
                idx = 0;
                foreach( DataColumn dc in DT.Columns )
                {
                    if( idx > 0 ) Context.Response.Write( "," );
                    Context.Response.Write( "\"" + _csvSafe( dr[dc].ToString() ) + "\"" );
                    idx++;
                }
                Context.Response.Write( "\r\n" );
            }

            Context.Response.AddHeader( "Content-Disposition", "attachment; filename=export.csv;" );
            Context.Response.End();
        }
        // Need to double " in string values
        private static string _csvSafe( string str )
        {
            return str.Replace( "\"", "\"\"" );
        }

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

        #region Temporary Files

        public FileInfo getTempFile( string FileName )
        {
            FileInfo RetInfo = null;
            DirectoryInfo myDirectoryInfo = new DirectoryInfo( _TempPath );
            FileInfo[] myFileInfoArray = myDirectoryInfo.GetFiles();
            foreach( FileInfo Info in myFileInfoArray )
            {
                if( Info.Name == FileName )
                {
                    RetInfo = Info;
                }
            }

            return RetInfo;
        }

        /// <summary>  Purge files in the temporary directory  </summary>
        /// <param name="FileExtension">  Optional extension type of files to purge.  Default is to purge all files  </param>
        /// <param name="HoursToKeepFiles">  Optional number of hours to keep temporary files around.  Default is 0 hours  </param>
        public void purgeTempFiles( string FileExtension = ".*", int HoursToKeepFiles = 0 )
        {
            DirectoryInfo myDirectoryInfo = new DirectoryInfo( _TempPath );
            FileInfo[] myFileInfoArray = myDirectoryInfo.GetFiles();

            FileExtension = FileExtension.ToLower().Trim();
            if( !FileExtension.StartsWith( "." ) )
            {
                FileExtension = "." + FileExtension;
            }
            foreach( FileInfo myFileInfo in myFileInfoArray )
            {
                if( ( FileExtension == "*" ) || ( myFileInfo.Extension.ToString().ToLower() == FileExtension ) )
                {
                    if( DateTime.Now.Subtract( myFileInfo.CreationTime ).TotalHours > HoursToKeepFiles )
                    {
                        myFileInfo.Delete();
                    }
                }
            }
        }

        private string _getFileNameForSchema( string UniqueFileId )
        {
            return _CswNbtResources.AccessId + "_" + UniqueFileId;
        }

        public string getFullFilePath( string UniqueFileId )
        {
            return Path.Combine( new string[] { _TempPath, _getFileNameForSchema( UniqueFileId ) } );
        }

        public Stream getFileInputStream( HttpContext Context, string ParamName = "" )
        {
            Stream RetStream = null;

            //This is the IE case
            if( false == string.IsNullOrEmpty( ParamName ) &&
                string.IsNullOrEmpty( Context.Request[ParamName] ) )
            {
                HttpPostedFile File = Context.Request.Files[0];
                RetStream = File.InputStream;
            }
            else
            {
                if( 0 == Context.Request.InputStream.Length || false == Context.Request.InputStream.CanRead )
                {
                    throw new CswDniException( ErrorType.Warning, "Cannot read the loaded file.", "File was empty or corrupt" );
                }
                RetStream = Context.Request.InputStream;
            }
            return RetStream;
        }

        private void _getFileStream( string RelativePath, out FileStream FileStream, out string FullPath )
        {
            FileStream = null;
            FullPath = string.Empty;
            if( false == string.IsNullOrEmpty( RelativePath ) )
            {
                FullPath = getFullFilePath( RelativePath );
                FileStream = File.Create( FullPath );
            }
        } // _getFileStream

        public string cacheInputStream( Stream InputStream, string Path )
        {
            FileStream OutputStream = null;
            string FullPath = string.Empty;
            _getFileStream( Path, out OutputStream, out FullPath );

            if( null != InputStream && null != OutputStream )
            {
                InputStream.CopyTo( OutputStream );
                InputStream.Close();
                OutputStream.Close();
            }
            return FullPath;
        }

        public void saveToTempFile( string SaveString, string Path )
        {
            FileStream OutputStream = null;
            string FullPath = string.Empty;
            _getFileStream( Path, out OutputStream, out FullPath );

            if( false == string.IsNullOrEmpty( SaveString ) && null != OutputStream )
            {
                StreamWriter w = new StreamWriter( OutputStream );
                w.Write( SaveString );
                w.Close();
            }
        }

        #endregion Temporary Files

        #region Client

        public static string makeId( string Prefix, string ID, string Suffix )
        {
            CswDelimitedString ElementId = new CswDelimitedString( _Delimiter ) { { Prefix, false }, { ID, false }, { Suffix, false } };

            return ElementId.ToString( false );
        }
        #endregion
    }//wsNBT

} // namespace ChemSW.WebServices
