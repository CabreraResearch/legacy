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
				_DelimitedString.Add( Node.NodeId.ToString() );
			else
				_DelimitedString.Add( "new" );
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
				// We have to accomodate for the fact that the NodeId actually takes up more than one 'slot' in _DelimitedString
				CswDelimitedString tempDS = new CswDelimitedString( PropIdDelim );
				tempDS.FromDelimitedString( _DelimitedString );
				tempDS.RemoveAt( tempDS.Count - 1 );

				CswPrimaryKey NodePk = new CswPrimaryKey();
				NodePk.FromString( tempDS.ToString() );
				return NodePk;
			}
		} // NodeId


		public Int32 NodeTypePropId
		{
			get
			{
				return CswConvert.ToInt32( _DelimitedString[_DelimitedString.Count - 1] );
			}
		} // NodeTypePropId

		public override string ToString()
		{
			return _DelimitedString.ToString();
		}

	} // public class CswPropIdAttr

} // namespace ChemSW.Nbt.WebServices
