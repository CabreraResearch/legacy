using System.Data;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Batch;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25978
    /// </summary>
    public class CswUpdateSchemaCase25978 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Make the Batch Operation Object Class and default NodeType
            CswNbtMetaDataObjectClass BatchOpOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, "task.gif", false, false );

            CswNbtMetaDataObjectClassProp BatchDataOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.BatchDataPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Memo );
            CswNbtMetaDataObjectClassProp EndDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.EndDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.DateTime, ServerManaged: true );
            CswNbtMetaDataObjectClassProp LogOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.LogPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Comments );
            CswNbtMetaDataObjectClassProp OpNameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.OpNamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.List, ServerManaged: true );
            CswNbtMetaDataObjectClassProp PriorityOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.PriorityPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Number );
            CswNbtMetaDataObjectClassProp StartDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.StartDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.DateTime, ServerManaged: true );
            CswNbtMetaDataObjectClassProp StatusOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List, ServerManaged: true );
            CswNbtMetaDataObjectClassProp UserOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, CswNbtObjClassBatchOp.UserPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship );

            CswNbtMetaDataNodeType BatchOpNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( BatchOpOC.ObjectClassId, "Batch Operation", "System" );

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();


            CswNbtView BatchOpView = _CswNbtSchemaModTrnsctn.makeView();
            BatchOpView.makeNew( "Batch Operations", NbtViewVisibility.Global );
            BatchOpView.Category = "System";
            BatchOpView.SetViewMode( NbtViewRenderingMode.Tree );
            
            CswNbtViewRelationship BatchOpViewRel = BatchOpView.AddViewRelationship( BatchOpOC, true );
            BatchOpView.AddViewPropertyAndFilter( BatchOpViewRel, 
                                                  StatusOCP, 
                                                  Value: NbtBatchOpStatus.Completed.ToString(), 
                                                  FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            BatchOpView.save();

        }//Update()

    }//class CswUpdateSchemaCase25978

}//namespace ChemSW.Nbt.Schema