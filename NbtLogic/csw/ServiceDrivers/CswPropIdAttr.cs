using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswPropIdAttr
    {
        private const char PropIdDelim = '_';
        private CswDelimitedString _DelimitedString;

        public CswPropIdAttr( string PropIdAttr )
        {
            _DelimitedString = new CswDelimitedString( PropIdDelim );
            _DelimitedString.FromString( PropIdAttr );
        }

        public CswPropIdAttr( CswPrimaryKey NodeId, Int32 PropId )
        {
            _construct( NodeId, PropId );
        }

        public CswPropIdAttr( CswNbtObjClass AsNode, CswNbtMetaDataNodeTypeProp Prop )
        {
            _construct( AsNode != null ? AsNode.NodeId : null, Prop.PropId );
        }

        public CswPropIdAttr( CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
        {
            _construct( Node != null ? Node.NodeId : null, Prop.PropId );
        }

        private void _construct( CswPrimaryKey NodeId, Int32 PropId )
        {
            _DelimitedString = new CswDelimitedString( PropIdDelim );
            if( NodeId != null )
            {
                _DelimitedString.Add( NodeId.TableName );
                _DelimitedString.Add( NodeId.PrimaryKey.ToString() );
            }
            else
            {
                _DelimitedString.Add( "new" );
                _DelimitedString.Add( "" );
            }
            _DelimitedString.Add( PropId.ToString() );
        }


        public CswPrimaryKey NodeId
        {
            get
            {
                CswPrimaryKey NodePk = new CswPrimaryKey();
                if( _DelimitedString[0] != "new" )
                {
                    NodePk.TableName = _DelimitedString[0];
                    NodePk.PrimaryKey = CswConvert.ToInt32( _DelimitedString[1] );
                }
                return NodePk;
            }
            set
            {
                _DelimitedString[0] = value.ToString();
            }
        } // NodeId


        public Int32 NodeTypePropId
        {
            get
            {
                return CswConvert.ToInt32( _DelimitedString[2] );
            }
            set
            {
                _DelimitedString[2] = value.ToString();
            }
        } // NodeTypePropId

        public override string ToString()
        {
            return _DelimitedString.ToString();
        }

    } // public class CswPropIdAttr

} // namespace ChemSW.Nbt.WebServices
