using System.Collections.ObjectModel;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30046 : CswUpdateSchemaTo
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
                CswNbtMetaDataObjectClassProp HomeLocationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Relationship,
                        PropName = CswNbtObjClassContainer.PropertyName.HomeLocation,
                        ReadOnly = true,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswEnumNbtObjectClass.LocationClass )
                    } );

                CswNbtMetaDataObjectClassProp ProjectOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Text,
                        PropName = CswNbtObjClassContainer.PropertyName.Project,
                        ReadOnly = true,
                    } );
                
                CswNbtMetaDataObjectClassProp TareQuantityOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Quantity,
                        PropName = CswNbtObjClassContainer.PropertyName.TareQuantity,
                        ReadOnly = true,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswEnumNbtObjectClass.UnitOfMeasureClass )
                    } );

                CswNbtMetaDataObjectClassProp SpecificActivityOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Text,
                        PropName = CswNbtObjClassContainer.PropertyName.SpecificActivity,
                        ReadOnly = true,
                    } );

                CswNbtMetaDataObjectClassProp ConcentrationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Text,
                        PropName = CswNbtObjClassContainer.PropertyName.Concentration,
                        ReadOnly = true,
                    } );

                CswNbtMetaDataObjectClassProp NotesOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Comments,
                        PropName = CswNbtObjClassContainer.PropertyName.Notes,
                        ReadOnly = true,
                    } );

                CswNbtMetaDataObjectClassProp OpenedDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.DateTime,
                        PropName = CswNbtObjClassContainer.PropertyName.OpenedDate,
                        ReadOnly = true,
                    } );
                _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps(); //in order to cascade hidden status to the nodetype props

                Collection<CswNbtMetaDataObjectClassProp> ObjectClassProps = new Collection<CswNbtMetaDataObjectClassProp>
                    {
                        HomeLocationOCP, ProjectOCP, TareQuantityOCP, SpecificActivityOCP, ConcentrationOCP, NotesOCP, OpenedDateOCP
                    }; 


                foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
                {
                    foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in ObjectClassProps )
                    {
                        CswNbtMetaDataNodeTypeProp NodetypeProp = ContainerNT.getNodeTypePropByObjectClassProp( ObjectClassProp );
                        NodetypeProp.removeFromAllLayouts();
                    }

                }

            }//if null != ContainerOC
        } // update()

    }

}//namespace ChemSW.Nbt.Schema