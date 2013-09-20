using ChemSW.Nbt.csw.Dev;
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
            
        }

        #endregion
    }

}//namespace ChemSW.Nbt.Schema