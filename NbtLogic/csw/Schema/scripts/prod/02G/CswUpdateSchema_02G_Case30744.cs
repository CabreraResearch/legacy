using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30744: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30744; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30744"; }
        }

        public override string Title
        {
            get { return "CAF Import - PackDetails -> Sizes"; }
        }

        public override void update()
        {
            _makeNewSizeProps();
            _createImportBindings();

        } // update()


        #region new size props

        private void _makeNewSizeProps()
        {
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( SizeOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassSize.PropertyName.Description,
                FieldType = CswEnumNbtFieldType.Text,
                IsFk = false,
                ServerManaged = false,
                ReadOnly = false,
                IsUnique = false,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( SizeOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassSize.PropertyName.Barcode,
                FieldType = CswEnumNbtFieldType.Barcode,
                IsFk = false,
                ServerManaged = false,
                ReadOnly = false,
                IsUnique = false,
            } );

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps(); //since we'll be using them later in the same script
        }

        #endregion



        #region import script

        private void _createImportBindings()
        {
            CswNbtSchemaUpdateImportMgr sizeImporter = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "packdetail", "Size", "packdetail_view" );

            sizeImporter.importBinding( "packdetailid", "Legacy Id", "" ); //needed because of view
            sizeImporter.importBinding( "packagedescription", CswNbtObjClassSize.PropertyName.Description, "" );
            sizeImporter.importBinding( "packageid", CswNbtObjClassSize.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
            sizeImporter.importBinding( "catalogno", CswNbtObjClassSize.PropertyName.CatalogNo, "" );
            sizeImporter.importBinding( "capacity", CswNbtObjClassSize.PropertyName.InitialQuantity, "Quantity" );
            sizeImporter.importBinding( "unitofmeasureid", CswNbtObjClassSize.PropertyName.InitialQuantity, "UnitId" );
            sizeImporter.importBinding( "dispenseonly", CswNbtObjClassSize.PropertyName.Dispensable, "" );
            sizeImporter.importBinding( "upc", CswNbtObjClassSize.PropertyName.Barcode, "" );
            sizeImporter.importBinding( "containertype", CswNbtObjClassSize.PropertyName.ContainerType, "" );

            sizeImporter.finalize(UseView: true);
        }

        #endregion
    }

}//namespace ChemSW.Nbt.Schema