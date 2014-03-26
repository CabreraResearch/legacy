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
            get { return "Add Open button to Containers"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                FieldType = CswEnumNbtFieldType.Button,
                PropName = CswNbtObjClassContainer.PropertyName.Open
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema