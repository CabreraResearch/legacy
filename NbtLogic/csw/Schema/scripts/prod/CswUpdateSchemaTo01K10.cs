using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01K-10
	/// </summary>
	public class CswUpdateSchemaTo01K10 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 10 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 24381, 24309
			// set default value of Inspection Schedule's Parent View to:
			//   Inspection Schedule
			//     All InspectionTargetGroupClass (by Inspection Schedule's Inspection Group)
			//       All InspectionTargetClass (by Inspection Target's Inspection Target Group)

			CswNbtMetaDataObjectClass InspectionTargetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
			CswNbtMetaDataObjectClassProp ITGroupOCP = InspectionTargetOC.getObjectClassProp( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );

			CswNbtMetaDataNodeType InspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
			if( InspectionScheduleNT != null )
			{
				CswNbtMetaDataNodeTypeProp ISParentViewNTP = InspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );
				CswNbtMetaDataNodeTypeProp ISInspectionGroupNTP = InspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );

				CswNbtView DefaultParentView = _CswNbtSchemaModTrnsctn.restoreView( ISParentViewNTP.DefaultValue.AsViewReference.ViewId );
				DefaultParentView.Root.ChildRelationships.Clear();

				CswNbtViewRelationship ISViewRel = DefaultParentView.AddViewRelationship( InspectionScheduleNT, true );
				CswNbtViewRelationship ISGroupRel = DefaultParentView.AddViewRelationship( ISViewRel, CswNbtViewRelationship.PropOwnerType.First, ISInspectionGroupNTP, true );
				CswNbtViewRelationship ISTargetRel = DefaultParentView.AddViewRelationship( ISGroupRel, CswNbtViewRelationship.PropOwnerType.Second, ITGroupOCP, true );
				DefaultParentView.save();
			}
		}//Update()

	}//class CswUpdateSchemaTo01K10

}//namespace ChemSW.Nbt.Schema


