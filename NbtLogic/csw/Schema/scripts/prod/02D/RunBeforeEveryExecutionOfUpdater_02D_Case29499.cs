using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29499
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case29499B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29499; }
        }

        public override void update()
        {
            CswNbtMetaDataPropertySet DocumentPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.DocumentSet );
            foreach( CswNbtMetaDataObjectClass DocOC in DocumentPS.getObjectClasses() )
            {
                _createProps( DocOC );
            }
        } // update()

        private void _createProps( CswNbtMetaDataObjectClass DocumentOC )
        {
            CswNbtMetaDataObjectClassProp LastModifiedOnOCP = DocumentOC.getObjectClassProp( CswNbtPropertySetDocument.PropertyName.LastModifiedOn );
            if( null == LastModifiedOnOCP )
            {
                LastModifiedOnOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( DocumentOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DocumentOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.LastModifiedOn,
                    FieldType = CswEnumNbtFieldType.DateTime,
                    ServerManaged = true,
                    Extended = CswEnumNbtDateDisplayMode.DateTime.ToString(),
                } );
            }

            CswNbtMetaDataObjectClassProp LastModifiedByOCP = DocumentOC.getObjectClassProp( CswNbtPropertySetDocument.PropertyName.LastModifiedBy );
            if( null == LastModifiedByOCP )
            {
                CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
                LastModifiedByOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( DocumentOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DocumentOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.LastModifiedBy,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    ServerManaged = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOC.ObjectClassId
                } );
            }
        }

    }//class CswUpdateSchema_02C_Case29499

}//namespace ChemSW.Nbt.Schema