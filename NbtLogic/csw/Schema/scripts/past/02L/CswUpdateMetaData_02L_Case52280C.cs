using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52280C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 52280; }
        }

        public override string Title
        {
            get { return "Add property reference to Container"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            // Add property reference property to Container
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            CswNbtMetaDataObjectClassProp ObsoleteOCP = MaterialPS.getObjectClasses().FirstOrDefault().getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.Obsolete );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp MaterialOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
            CswNbtMetaDataObjectClassProp MaterialObsoleteOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.MaterialObsolete );
            if( null == MaterialObsoleteOCP )
            {
                MaterialObsoleteOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
                {
                    PropName = CswNbtObjClassContainer.PropertyName.MaterialObsolete,
                    FieldType = CswEnumNbtFieldType.PropertyReference,
                    IsFk = true,
                    FkType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                    FkValue = MaterialOCP.PropId,
                    ValuePropId = ObsoleteOCP.PropId,
                    ValuePropType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString()
                } );
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema