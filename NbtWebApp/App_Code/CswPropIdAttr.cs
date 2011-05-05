using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.WebServices
{
	public class CswPropIdAttr
	{
		private const char PropIdDelim = '_';
		private CswDelimitedString _DelimitedString;

		public CswPropIdAttr( CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
		{
			_DelimitedString = new CswDelimitedString( PropIdDelim );
			if( Node != null )
			{
				_DelimitedString.Add( Node.NodeId.TableName );
				_DelimitedString.Add( Node.NodeId.PrimaryKey.ToString() );
			}
			else
			{
				_DelimitedString.Add( "new" );
				_DelimitedString.Add( "" );
			}
			_DelimitedString.Add( Prop.PropId.ToString() );
		}

		public CswPropIdAttr( string PropIdAttr )
		{
			_DelimitedString = new CswDelimitedString( PropIdDelim );
			_DelimitedString.FromString( PropIdAttr );
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
