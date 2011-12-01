using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01K-03
	/// </summary>
	public class CswUpdateSchemaTo01K03 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 03 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 20732
			
			CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
			CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
			CswNbtMetaDataObjectClass InspectionTargetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );

			CswNbtMetaDataNodeType InspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Physical Inspection Schedule" );
			if( InspectionScheduleNT == null )
			{
				InspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GeneratorOC.ObjectClassId, CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName, "Inspections" );
			}

			CswNbtMetaDataNodeTypeProp ParentTypeNTP = InspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
			CswNbtMetaDataNodeTypeProp ParentViewNTP = InspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );
			CswNbtMetaDataNodeTypeProp TargetTypeNTP = InspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );

			// Rename "Physical Inspection Schedule" to "Inspection Schedule"
			InspectionScheduleNT.NodeTypeName = CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName;
			InspectionScheduleNT.Category = "Inspections";
			InspectionScheduleNT.getFirstNodeTypeTab().TabName = CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName;

			// Rename "FE Inspection Point Type" to "Inspection Target Type"
			ParentTypeNTP.PropName = "Inspection Target Type";
			ParentTypeNTP.ReadOnly = false;

			// Rename "FE Inspection Point View" to "Inspection Target View"
			ParentViewNTP.PropName = "Inspection Target View";
			ParentViewNTP.ReadOnly = false;

			// Set "Inspection Type" nodetypeselect constraint to Inspection Design object class
			TargetTypeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), InspectionDesignOC.ObjectClassId );

			// Set "Inspection Target Type" nodetypeselect constraint to Inspection Target object class
			ParentTypeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), InspectionTargetOC.ObjectClassId );

		}//Update()

	}//class CswUpdateSchemaTo01K03

}//namespace ChemSW.Nbt.Schema


