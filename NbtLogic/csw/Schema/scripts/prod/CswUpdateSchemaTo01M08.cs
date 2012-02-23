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
    /// Updates the schema to version 01M-08
    /// </summary>
    public class CswUpdateSchemaTo01M08 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 08 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 25050

            string newCat = "SI_Example";
            if( null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SI_Protocol" ) &&
                null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SI_Target" ) &&
                null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SI_Group" ) &&
                null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SI_Scheduler" ))
            {
                //  implement nodetypes:                
                
                Int32 schedntid = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "GeneratorClass", "SI_Scheduler", newCat ).FirstVersionNodeTypeId;

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
                dr1["SECTION"] = "";
                dr1["QUESTION"] = "Is the item present?";
                dr1["ALLOWED_ANSWERS"] = "Yes,No,n/a";
                dr1["COMPLIANT_ANSWERS"] = "Yes,n/a";
                dr1["HELP_TEXT"] = "Choose an answer.";
                qTable.Rows.Add( dr1 );

                // Section1: Q1.2 Is the item operable?
                DataRow dr2 = qTable.NewRow();
                dr2["SECTION"] = "";
                dr2["QUESTION"] = "Is the item operable?";
                dr2["ALLOWED_ANSWERS"] = "Yes,No,n/a";
                dr2["COMPLIANT_ANSWERS"] = "Yes,n/a";
                dr2["HELP_TEXT"] = "Choose an answer.";
                qTable.Rows.Add( dr2 );

                // Section1: Q1.3 Item passes inspection?
                DataRow dr3 = qTable.NewRow();
                dr3["SECTION"] = "";
                dr3["QUESTION"] = "Item passes inspection?";
                dr3["ALLOWED_ANSWERS"] = "Yes,No";
                dr3["COMPLIANT_ANSWERS"] = "Yes";
                dr3["HELP_TEXT"] = "Choose an answer.";
                qTable.Rows.Add( dr3 );
                
                JObject newObj = wiz.createInspectionDesignTabsAndProps( qTable, "SI_protocol", "SI_target", newCat );

                //create nodes

                //routeNode.NodeName = "Example_SI_Route";
                //ExampleGroup
                CswNbtNode groupNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.GroupNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                //groupNode.NodeName = "Example_SI_Group";
                CswNbtObjClassInspectionTargetGroup groupNodeAsGroup = CswNbtNodeCaster.AsInspectionTargetGroup( groupNode );
                groupNodeAsGroup.Name.Text = "Example Group";
                groupNodeAsGroup.postChanges(true);

                //ExampleMonthlyInspectionSchedule (disabled)
                CswNbtNode schedNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( schedntid, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassGenerator schedNodeAsGenerator = CswNbtNodeCaster.AsGenerator( schedNode );
                schedNodeAsGenerator.Owner.RelatedNodeId = groupNode.NodeId;
                schedNodeAsGenerator.TargetType.SelectedNodeTypeIds.Add( wiz.DesignNtId.ToString() );
                schedNodeAsGenerator.ParentType.SelectedNodeTypeIds.Add( wiz.TargetNtId.ToString() );
                schedNodeAsGenerator.Description.Text = "Example Schedule";
                schedNodeAsGenerator.postChanges(true);

                //ExampleItem
                CswNbtNode itemNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.TargetNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                //itemNode.NodeName = "Example_SI_InspectionPoint";
                CswNbtObjClassInspectionTarget itemNodeAsTarget = CswNbtNodeCaster.AsInspectionTarget( itemNode );
                itemNodeAsTarget.Description.Text = "Example Target";
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
                itemNodeAsTarget.postChanges(true);
                //route (don't have this relationship on the objclass yet, save for future release)
                //itemNodeAsTarget.InspectionTargetGroup.NodeTypeProp.SetFK(PropIdType.NodeTypePropId,groupNode.NodeId);


                //ExampleInspection (manual)
                CswNbtNode inspectNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.DesignNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                //inspectNode.NodeName = "Example_SI_Inspection";
                //set target
                CswNbtObjClassInspectionDesign inspectNodeAsDesign = CswNbtNodeCaster.AsInspectionDesign( inspectNode );
                inspectNodeAsDesign.Owner.RelatedNodeId= itemNode.NodeId;
                inspectNodeAsDesign.postChanges(true);

            }
            #endregion case 25050

        }//Update()

    }//class CswUpdateSchemaTo01M08

}//namespace ChemSW.Nbt.Schema