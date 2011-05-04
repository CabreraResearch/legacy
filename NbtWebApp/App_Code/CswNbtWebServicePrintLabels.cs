using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServicePrintLabels
	{
		private CswNbtResources _CswNbtResources;
		public CswNbtWebServicePrintLabels( CswNbtResources Resources )
		{
			_CswNbtResources = Resources;
		}

		public JObject getLabels( string PropIdAttr )
		{
			JObject ret = new JObject();
			JArray Labels = new JArray();
			ret.Add( new JProperty("labels", Labels ));

			CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
			CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
			Int32 NodeTypeId = MetaDataProp.NodeType.NodeTypeId;

			string PrintLabelNodeTypesPropertyName = "NodeTypes";
			CswNbtMetaDataObjectClass PrintLabelObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
			CswNbtMetaDataObjectClassProp NodeTypesProperty = PrintLabelObjectClass.getObjectClassProp( PrintLabelNodeTypesPropertyName );

			CswNbtView PrintLabelView = new CswNbtView( _CswNbtResources );
			PrintLabelView.ViewName = "getPrintLabelsForNodeType(" + NodeTypeId.ToString() + ")";
			CswNbtViewRelationship PrintLabelRelationship = PrintLabelView.AddViewRelationship( PrintLabelObjectClass, true );
			CswNbtViewProperty PrintLabelNodeTypesProperty = PrintLabelView.AddViewProperty( PrintLabelRelationship, NodeTypesProperty );
			CswNbtViewPropertyFilter PrintLabelNodeTypesPropertyFilter = PrintLabelView.AddViewPropertyFilter( PrintLabelNodeTypesProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Contains, NodeTypeId.ToString(), false );

			ICswNbtTree PrintLabelsTree = _CswNbtResources.Trees.getTreeFromView( PrintLabelView, true, true, false, false ); 
			
			PrintLabelsTree.goToRoot();
			for( int i = 0; i < PrintLabelsTree.getChildNodeCount(); i++ )
			{
				PrintLabelsTree.goToNthChild( i );
				Labels.Add(new JObject(
					new JProperty("name", PrintLabelsTree.getNodeNameForCurrentPosition()),
					new JProperty("nodeid", PrintLabelsTree.getNodeIdForCurrentPosition().ToString())
				));
				PrintLabelsTree.goToParentNode();
			}
			return ret;
		} // getLabels()


	} // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
