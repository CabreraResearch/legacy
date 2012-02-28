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
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-10
    /// </summary>
    public class CswUpdateSchemaTo01M10 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 10 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 24975


            CswNbtMetaDataObjectClass ReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( ReportOc.ObjectClass,CswNbtObjClassReport.SqlPropertyName,CswNbtMetaDataFieldType.NbtFieldType.Memo);

            _CswNbtSchemaModTrnsctn.createObjectClassProp( ReportOc.ObjectClass,
                                                                       CswNbtObjClassReport.btnRunPropertyName,
                                                                       CswNbtMetaDataFieldType.NbtFieldType.Button,
                                                                       false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty, Int32.MinValue, Int32.MinValue,
                                                                       CswNbtNodePropButton.ButtonMode.button.ToString(),
                                                                       false,
                                                                       AuditLevel.NoAudit,
                                                                       "Run");

            CswNbtMetaDataNodeType rptNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType("Report");

            if( null != rptNT )
            {
                CswNbtNode rptNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( rptNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassReport rptNodeAsReport = CswNbtNodeCaster.AsReport( rptNode );
                rptNodeAsReport.ReportName.Text = "SQL Report View Dictionary";
                rptNodeAsReport.SQL.Text = "select * from vwAutoViewColNames";                
                rptNodeAsReport.postChanges(true);                
            }
            #endregion case 24975

        }//Update()

    }//class CswUpdateSchemaTo01M10

}//namespace ChemSW.Nbt.Schema