using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case29446A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29446; }
        }

        public override string Title
        {
            get { return "Add new props to Chemicals and Containers"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                FieldType = CswEnumNbtFieldType.TimeInterval,
                PropName = CswNbtObjClassChemical.PropertyName.OpenExpirationInterval
            } );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                FieldType = CswEnumNbtFieldType.Button,
                PropName = CswNbtObjClassContainer.PropertyName.Open
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema