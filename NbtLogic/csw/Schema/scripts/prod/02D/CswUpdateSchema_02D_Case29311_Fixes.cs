using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateSchema_02D_Case29311_Fixes : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
        }

        public override void update()
        {
            // For ChildContents properties, set 'child relationship' options to be all relationships that point to the owner's relational nodetype
            CswNbtMetaDataObjectClass DesignNtpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
            //CswNbtFieldTypeRuleChildContents ChildContentsRule = (CswNbtFieldTypeRuleChildContents) _CswNbtSchemaModTrnsctn.MetaData.getFieldTypeRule( CswEnumNbtFieldType.ChildContents );
            CswNbtMetaDataNodeType ChildContentsPropNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassDesignNodeTypeProp.getNodeTypeName( CswEnumNbtFieldType.ChildContents ) );
            CswNbtMetaDataNodeTypeProp ChildRelationshipNTP = ChildContentsPropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.ChildRelationship );

            // Target is DesignNodeTypeProp nodes
            CswNbtObjClassDesignNodeTypeProp ChildRelationshipNode = _CswNbtSchemaModTrnsctn.Nodes.getNodeByRelationalId( new CswPrimaryKey( "nodetype_props", ChildRelationshipNTP.PropId ) );
            CswNbtMetaDataNodeTypeProp TargetNTP = ChildRelationshipNode.NodeType.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Target );
            ChildRelationshipNode.Node.Properties[TargetNTP].AsMetaDataList.setValue( CswNbtNodePropMetaDataList.ObjectClassPrefix + DesignNtpOC.ObjectClassId );

            ChildRelationshipNode.postChanges( false );
        }

        // update()
    }//class CswUpdateSchema_02D_Case29311_Fixes

}//namespace ChemSW.Nbt.Schema