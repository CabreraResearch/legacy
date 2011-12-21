using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.WebServices
{

    public class wsTools : System.Web.Services.WebService
    {
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
                //CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( CswNbtResources, FromSafeJavaScriptParam( NodeKey ) );
                CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( CswNbtResources, NodeKey );
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

        public Stream getFileInputStream( string ParamName = "" )
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

        public string cacheInputStream( Stream InputStream, string Path )
        {
            string RetPath = string.Empty;
            if( null != InputStream &&
                false == string.IsNullOrEmpty( Path ) )
            {
                RetPath = _TempPath + "\\" + Path;
                using( FileStream OutputFile = File.Create( RetPath ) )
                {
                    InputStream.CopyTo( OutputFile );
                }
            }
            return RetPath;
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
