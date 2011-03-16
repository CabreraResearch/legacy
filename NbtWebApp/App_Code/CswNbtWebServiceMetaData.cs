using System;
using System.Collections.Generic;
using System.Reflection;
using ChemSW.Core;
using ChemSW.Exceptions;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceMetaData
	{
		private readonly CswNbtResources _CswNbtResources;

		public CswNbtWebServiceMetaData( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		} //ctor

		public XElement getNodeTypes()
		{
			XElement ReturnVal = new XElement( "nodetypes" );
			foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
			{

				ReturnVal.Add( new XElement( "nodetype", 
												new XAttribute( "id", NodeType.NodeTypeId ),
												new XAttribute( "name", NodeType.NodeTypeName ),
												new XAttribute( "objectclass", NodeType.ObjectClass.ObjectClass.ToString() ) ) );
			} // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
			
			return ReturnVal;
		} // getNodeTypes()
	
	} // class CswNbtWebServiceMetaData

} // namespace ChemSW.Nbt.WebServices
