using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-12
    /// </summary>
    public class CswUpdateSchemaTo01L12 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 12 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24656

            CswNbtMetaDataNodeType InspectionSchedNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            if( null != InspectionSchedNt )
            {
                CswNbtMetaDataNodeTypeProp OwnerNtp = InspectionSchedNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                OwnerNtp.HelpText = "Which set of targets (Inspection Points) will be scheduled. Usually by locations or types of items. (ex: Safety Equipment - Fixed)";

                CswNbtMetaDataNodeTypeProp ParentTypeNtp = InspectionSchedNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
                ParentTypeNtp.HelpText = "What will be inspected? (ex: Eye Wash Station)";

                CswNbtMetaDataNodeTypeProp TargetTypeNtp = InspectionSchedNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
                TargetTypeNtp.HelpText = "What Inspection Design will be used. (ex: Eye Wash Station Check)";
            }
            #endregion Case 24656

        }//Update()

    }//class CswUpdateSchemaTo01L12

}//namespace ChemSW.Nbt.Schema


