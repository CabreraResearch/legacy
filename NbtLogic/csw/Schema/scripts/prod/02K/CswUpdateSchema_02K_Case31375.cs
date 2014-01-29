using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31375 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31375; }
        }

        public override string Title
        {
            get { return "Make File Type readonly"; }
        }

        public override void update()
        {
            CswNbtMetaDataPropertySet DocumentPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.DocumentSet );
            foreach( CswNbtMetaDataObjectClass DocumentOC in DocumentPS.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp FileTypeOCP = DocumentOC.getObjectClassProp( CswNbtPropertySetDocument.PropertyName.FileType );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FileTypeOCP, CswEnumNbtObjectClassPropAttributes.readOnly, true );
            }
        } // update()

    } // class CswUpdateSchema_02K_Case31749
} // namespace