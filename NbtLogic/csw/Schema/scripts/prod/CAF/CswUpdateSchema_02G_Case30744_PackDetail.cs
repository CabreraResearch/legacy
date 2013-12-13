﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30744_PackDetail : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30744; }
        }

        public override string Title
        {
            get { return "CAF Import - PackDetails -> Sizes"; }
        }

        public override void update()
        {
            _createImportBindings();

        } // update()


        #region import script

        private void _createImportBindings()
        {
            CswNbtSchemaUpdateImportMgr sizeImporter = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            sizeImporter.CAFimportOrder( "Size", "packdetail", "packdetail_view", "packdetailid" );

            sizeImporter.importBinding( "packagedescription", CswNbtObjClassSize.PropertyName.Description, "" );
            sizeImporter.importBinding( "packageid", CswNbtObjClassSize.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
            sizeImporter.importBinding( "catalogno", CswNbtObjClassSize.PropertyName.CatalogNo, "" );
            sizeImporter.importBinding( "capacity", CswNbtObjClassSize.PropertyName.InitialQuantity, CswEnumNbtSubFieldName.Value.ToString() );
            sizeImporter.importBinding( "unitofmeasureid", CswNbtObjClassSize.PropertyName.InitialQuantity, CswEnumNbtSubFieldName.NodeID.ToString() );
            //sizeImporter.importBinding( "upc", "UPC Barcode", "" );//Case 31264 invalidates this binding
            sizeImporter.importBinding( "containertype", CswNbtObjClassSize.PropertyName.ContainerType, "" );

            sizeImporter.finalize();
        }

        #endregion
    }

}//namespace ChemSW.Nbt.Schema