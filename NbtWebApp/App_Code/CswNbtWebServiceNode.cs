using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Statistics;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceNode
	{
		private readonly CswNbtResources _CswNbtResources;
		private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
		public CswNbtWebServiceNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
		{
			_CswNbtResources = CswNbtResources;
			_CswNbtStatisticsEvents = CswNbtStatisticsEvents;
		}

		public CswPrimaryKey CopyNode( CswPrimaryKey NodePk )
		{
			CswNbtNode OriginalNode = _CswNbtResources.Nodes[NodePk];

			CswNbtNode NewNode = null;
			CswNbtActCopyNode CswNbtActCopyNode = new CswNbtActCopyNode( _CswNbtResources );
			switch( OriginalNode.ObjectClass.ObjectClass )
			{
				case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass:
					NewNode = CswNbtActCopyNode.CopyEquipmentNode( OriginalNode );
					break;
				case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass:
					NewNode = CswNbtActCopyNode.CopyEquipmentAssemblyNode( OriginalNode );
					break;
				default:
					NewNode = CswNbtActCopyNode.CopyNode( OriginalNode );
					break;
			}

			if( NewNode != null )
			{
				_CswNbtStatisticsEvents.OnCopyNode( OriginalNode, NewNode );
			}
			return NewNode.NodeId;
		}

		public bool DeleteNode( CswPrimaryKey NodePk )
		{
			bool ret = false;

			CswNbtNode NodeToDelete = _CswNbtResources.Nodes[NodePk];
			NodeToDelete.delete();
			ret = true;

			return ret;
		}



	} // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
