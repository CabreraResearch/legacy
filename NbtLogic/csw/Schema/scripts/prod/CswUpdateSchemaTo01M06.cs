using System;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-06
    /// </summary>
    public class CswUpdateSchemaTo01M06 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 06 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 25050

            string newCat = "SI_Example";
            if( null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "ExampleSiInspectionDesign" ) &&
                null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "ExampleSiTargetItem" ) &&
                null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "ExampleSiGroup" ) &&
                null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "ExampleSiScheduler" ) &&
                null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "ExampleSiRoute" ) )
            {
                //  implement nodetypes:
                // ExampleSiRoute (NO user relationship at this time)
                Int32 routentid = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "InspectionRouteClass", "ExampleSiRoute", newCat ).FirstVersionNodeTypeId;
                Int32 schedntid = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "GeneratorClass", "ExampleSiScheduler", newCat ).FirstVersionNodeTypeId;

                // ExampleSiTargetItem
                // ExampleSiTargetItem Group
                // ExampleSiInspectionDesign
                CswNbtActInspectionDesignWiz wiz = _CswNbtSchemaModTrnsctn.getCswNbtActInspectionDesignWiz();
                DataTable qTable = new DataTable();
                qTable.Columns.Add( "SECTION", typeof( string ) );
                qTable.Columns.Add( "QUESTION", typeof( string ) );
                qTable.Columns.Add( "ALLOWED_ANSWERS", typeof( string ) );
                qTable.Columns.Add( "COMPLIANT_ANSWERS", typeof( string ) );
                qTable.Columns.Add( "HELP_TEXT", typeof( string ) );

                // Section1: Q1.1 Is the item present?
                DataRow dr1 = qTable.NewRow();
                dr1["SECTION"] = "Section1";
                dr1["QUESTION"] = "Is the item present?";
                dr1["ALLOWED_ANSWERS"] = "Yes,No,n/a";
                dr1["COMPLIANT_ANSWERS"] = "Yes,n/a";
                dr1["HELP_TEXT"] = "Choose an answer.";
                qTable.Rows.Add( dr1 );

                // Section1: Q1.2 Is the item operable?
                DataRow dr2 = qTable.NewRow();
                dr2["SECTION"] = "Section1";
                dr2["QUESTION"] = "Is the item operable?";
                dr2["ALLOWED_ANSWERS"] = "Yes,No,n/a";
                dr2["COMPLIANT_ANSWERS"] = "Yes,n/a";
                dr2["HELP_TEXT"] = "Choose an answer.";
                qTable.Rows.Add( dr2 );

                // Section1: Q1.3 Item passes inspection?
                DataRow dr3 = qTable.NewRow();
                dr3["SECTION"] = "Section1";
                dr3["QUESTION"] = "Item passes inspection?";
                dr3["ALLOWED_ANSWERS"] = "Yes,No";
                dr3["COMPLIANT_ANSWERS"] = "Yes";
                dr3["HELP_TEXT"] = "Choose an answer.";
                qTable.Rows.Add( dr3 );

                JObject newObj = wiz.createInspectionDesignTabsAndProps( qTable, "ExampleSiInspectionDesign", "ExampleSiTargetItem", newCat );

                //create nodes
                //ExampleRoute
                CswNbtNode routeNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( routentid, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                routeNode.NodeName = "ExampleSiRoute";
                //ExampleGroup
                CswNbtNode groupNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.GroupNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                groupNode.NodeName = "ExampleSiGroup";

                //ExampleMonthlyInspectionSchedule (disabled)
                CswNbtNode schedNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( schedntid, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                schedNode.NodeName = "ExampleSiSchedule";
                CswNbtObjClassGenerator schedNodeAsGenerator = CswNbtNodeCaster.AsGenerator( schedNode );
                schedNodeAsGenerator.Owner.RelatedNodeId = groupNode.NodeId;
                schedNodeAsGenerator.TargetType.SelectedNodeTypeIds.Add( wiz.DesignNtId.ToString() );
                schedNodeAsGenerator.ParentType.SelectedNodeTypeIds.Add( wiz.TargetNtId.ToString() );

                //ExampleItem
                CswNbtNode itemNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.TargetNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                itemNode.NodeName = "ExampleSiInspectionPoint";
                CswNbtObjClassInspectionTarget itemNodeAsTarget = CswNbtNodeCaster.AsInspectionTarget( itemNode );
                //set location, set group
                CswPrimaryKey somelocationid = null;
                foreach( CswNbtNode anode in _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass ).getNodes( false, false ) )
                {
                    if( anode.NodeName == "Room N101" )
                    {
                        somelocationid = anode.NodeId;
                    }
                }
                if( null != somelocationid ) itemNodeAsTarget.Location.SelectedNodeId = somelocationid;
                itemNodeAsTarget.InspectionTargetGroup.RelatedNodeId = groupNode.NodeId;
                //route?
                //itemNodeAsTarget.InspectionTargetGroup.NodeTypeProp.SetFK(CswNbtViewRelationship.PropIdType.NodeTypePropId,groupNode.NodeId);


                //ExampleInspection (manual)
                CswNbtNode inspectNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.DesignNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                inspectNode.NodeName = "ExampleSiInspection";
                //set target
                CswNbtObjClassInspectionDesign inspectNodeAsDesign = CswNbtNodeCaster.AsInspectionDesign( inspectNode );
                inspectNodeAsDesign.Owner.RelatedNodeId= itemNode.NodeId;


            }
            #endregion case 25050

        }//Update()

    }//class CswUpdateSchemaTo01M05

}//namespace ChemSW.Nbt.Schema