using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30046 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {                
            get { return 30046; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "New Container Props Needed for CAF Imports"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            if( null != ContainerOC )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Location,
                        PropName = CswNbtObjClassContainer.PropertyName.HomeLocation,
                        ReadOnly = true,
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Text,
                        PropName = CswNbtObjClassContainer.PropertyName.Project,
                        ReadOnly = true,
                    } );
                
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Quantity,
                        PropName = CswNbtObjClassContainer.PropertyName.TareQuantity,
                        ReadOnly = true,
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Text,
                        PropName = CswNbtObjClassContainer.PropertyName.SpecificActivity,
                        ReadOnly = true
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Text,
                        PropName = CswNbtObjClassContainer.PropertyName.Concentration,
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Comments,
                        PropName = CswNbtObjClassContainer.PropertyName.Notes,
                    } );


            }//if null != ContainerOC
        } // update()

    }

}//namespace ChemSW.Nbt.Schema