using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01K-07
    /// </summary>
    public class CswUpdateSchemaTo01K07 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 07 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
			// case 24334
			CswNbtMetaDataObjectClass GroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

			CswNbtMetaDataNodeType InspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
			if( InspectionScheduleNT != null )
			{
				CswNbtMetaDataNodeTypeProp OwnerNTP = InspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
				OwnerNTP.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), GroupOC.ObjectClassId );
				// twice to set the view
				OwnerNTP.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), GroupOC.ObjectClassId );
			}

        }//Update()

    }//class CswUpdateSchemaTo01K07

}//namespace ChemSW.Nbt.Schema


