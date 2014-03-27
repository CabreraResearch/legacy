using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52307: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52307; }
        }

        public override string Title
        {
            get { return "Create Testing Lab ObjClass attached to the MLM module"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            //Create the new ObjClass
            CswNbtMetaDataObjectClass TestingLabOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.TestingLabClass, "door.png", false );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabOC, new CswNbtWcfMetaDataModel.ObjectClassProp( TestingLabOC )
                {
                    PropName = CswNbtObjClassTestingLab.PropertyName.Name,
                    FieldType = CswEnumNbtFieldType.Text,
                    IsUnique = true,
                    IsRequired = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabOC, new CswNbtWcfMetaDataModel.ObjectClassProp( TestingLabOC )
            {
                PropName = CswNbtObjClassTestingLab.PropertyName.SampleDeliveryRequired,
                FieldType = CswEnumNbtFieldType.Logical
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( TestingLabOC, new CswNbtWcfMetaDataModel.ObjectClassProp( TestingLabOC )
            {
                PropName = CswNbtObjClassTestingLab.PropertyName.SampleDeliveryLocation,
                FieldType = CswEnumNbtFieldType.Location
            } );

            //Tie it to MLM module
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, TestingLabOC.ObjectClassId );

            //Create the default NT
            _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( TestingLabOC )
                {
                    Category = "MLM",
                    IconFileName = "door.png",
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassTestingLab.PropertyName.Name ),
                    NodeTypeName = "Testing Lab",
                    ObjectClass = TestingLabOC,
                    ObjectClassId = TestingLabOC.ObjectClassId,
                    Searchable = true
                } );
        }
    }
}