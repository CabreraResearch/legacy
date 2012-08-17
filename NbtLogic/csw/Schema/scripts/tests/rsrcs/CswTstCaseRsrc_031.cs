using System;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

//using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// </summary>
    public class CswTstCaseRsrc_031
    {

        public CswTstCaseRsrc_031()
        { }

        private CswTestCaseRsrc _CswTestCaseRsrc;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
        {
            set
            {
                _CswNbtSchemaModTrnsctn = value;
                _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            }
        }
        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();

        public const string TestNodeTypeName = "Equipment";
        public const string TestNodeName = "Test Equipment";
        public const string Purpose = "Generate audit records for nodetype " + TestNodeTypeName;

        private string _TestNodeTypePropName = "Description";
        private string _PropValInitial = "Initial Value";
        private string _PropValChanged = "Updated Value";
        Int32 _NodeTypeId = Int32.MinValue;
        public void enableAuditing()
        {
            //CswNbtObjClassEquipment CswNbtObjClassEquipment = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtMetaDataNodeType TestNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( TestNodeTypeName );
            _NodeTypeId = TestNodeType.NodeTypeId;
            foreach( CswNbtMetaDataNodeTypeProp CurrentProp in TestNodeType.getNodeTypeProps() )
            {
                CurrentProp.AuditLevel = AuditLevel.PlainAudit;
            }

        }

        private CswPrimaryKey _NodePrimaryKey = null;
        public void makeNodeInstance()
        {
            CswNbtMetaDataNodeType TestNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( _NodeTypeId );
            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( _NodeTypeId, _TestNodeTypePropName );


            CswNbtNode NodeInstance = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( TestNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            NodeInstance.Properties[NodeTypeProp].AsMemo.Text = _PropValInitial;
            _NodePrimaryKey = NodeInstance.NodeId;
            NodeInstance.postChanges( true );


        }

        public void setChangedValue()
        {
            CswNbtNode NodeInstance = _CswNbtSchemaModTrnsctn.Nodes[_NodePrimaryKey];
            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( _NodeTypeId, _TestNodeTypePropName );
            NodeInstance.Properties[NodeTypeProp].AsMemo.Text = _PropValChanged;
            NodeInstance.postChanges( true );

        }//setChagnedValue


    }//CswTstCaseRsrc_031

}//ChemSW.Nbt.Schema
