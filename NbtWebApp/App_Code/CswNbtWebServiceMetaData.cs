using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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

        public JObject getNodeTypes( CswNbtMetaDataObjectClass ObjectClass, string ExcludeNodeTypeIds )
        {
            JObject ReturnVal = new JObject();
            CswCommaDelimitedString ExcludedNodeTypes = new CswCommaDelimitedString();
            ExcludedNodeTypes.FromString( ExcludeNodeTypeIds );
            Collection<Int32> ExcludedIds = ExcludedNodeTypes.ToIntCollection();

            if( null == ObjectClass )
            {
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() )
                {
                    if( false == ExcludedIds.Contains( NodeType.NodeTypeId ) )
                    {
                        _addNodeTypeAttributes( NodeType, ReturnVal );
                    }
                } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
            }
            else
            {
                foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
                {
                    if( NodeType.IsLatestVersion() && false == ExcludedIds.Contains( NodeType.NodeTypeId ) )
                    {
                        _addNodeTypeAttributes( NodeType, ReturnVal );
                    }
                }
            }
            return ReturnVal;
        } // getNodeTypes()

        private void _addNodeTypeAttributes( CswNbtMetaDataNodeType NodeType, JObject ReturnVal )
        {
            CswNbtMetaDataObjectClass ObjectClass = NodeType.getObjectClass();
            string NtName = "nodetype_" + NodeType.NodeTypeId;
            ReturnVal[NtName] = new JObject();
            ReturnVal[NtName]["id"] = NodeType.NodeTypeId;
            ReturnVal[NtName]["name"] = NodeType.NodeTypeName;
            ReturnVal[NtName]["objectclass"] = ObjectClass.ObjectClass.ToString();
            ReturnVal[NtName]["objectclassid"] = ObjectClass.ObjectClassId.ToString();

            switch( ObjectClass.ObjectClass )
            {
                case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass:
                    CswNbtMetaDataNodeTypeProp InspectionTargetNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );
                    ReturnVal[NtName]["targetnodetypeid"] = InspectionTargetNTP.FKValue.ToString();
                    if( InspectionTargetNTP.FKType == NbtViewPropIdType.NodeTypePropId.ToString() )
                    {
                        ReturnVal[NtName]["targetnodetypeid"] = InspectionTargetNTP.FKValue.ToString();
                    }
                    break;
            }

        }

    } // class CswNbtWebServiceMetaData

} // namespace ChemSW.Nbt.WebServices
