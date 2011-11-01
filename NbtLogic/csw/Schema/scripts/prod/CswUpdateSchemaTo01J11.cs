using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-11
    /// </summary>
    public class CswUpdateSchemaTo01J11 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 11 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            // case 24083
            CswNbtMetaDataNodeType EquipmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment" );
            CswNbtMetaDataNodeType ProblemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Problem" );

			CswNbtMetaDataNodeTypeProp ProblemOwnerNTP = ProblemNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.OwnerPropertyName );
			CswNbtMetaDataNodeTypeProp ProblemGridNTP = EquipmentNT.getNodeTypeProp( "ProblemGrid" );

			if( null != EquipmentNT &&
				null != ProblemNT &&
				null != ProblemOwnerNTP &&
				null != ProblemGridNTP )
			{
				CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( ProblemGridNTP.ViewId );
				View.Root.ChildRelationships.Clear();

				//Equipment
				//    Problem (by Equipment)
				//        Start Date
				//        Closed
				//        Problem

				CswNbtViewRelationship EquipmentViewRel = View.AddViewRelationship( EquipmentNT, false );
				CswNbtViewRelationship ProblemViewRel = View.AddViewRelationship( EquipmentViewRel, CswNbtViewRelationship.PropOwnerType.Second, ProblemOwnerNTP, true );

				CswNbtMetaDataNodeTypeProp ProblemStartDateNTP = ProblemNT.getNodeTypeProp( "Start Date" );
				if( ProblemStartDateNTP != null )
				{
					CswNbtViewProperty StartDateViewProp = View.AddViewProperty( ProblemViewRel, ProblemStartDateNTP );
					StartDateViewProp.Order = 1;
				}
				CswNbtMetaDataNodeTypeProp ProblemClosedNTP = ProblemNT.getNodeTypeProp( "Closed" );
				if( ProblemClosedNTP != null )
				{
					CswNbtViewProperty ClosedViewProp = View.AddViewProperty( ProblemViewRel, ProblemClosedNTP );
					ClosedViewProp.Order = 1;
				}
				CswNbtMetaDataNodeTypeProp ProblemProblemNTP = ProblemNT.getNodeTypeProp( "Problem" );
				if( ProblemProblemNTP != null )
				{
					CswNbtViewProperty ProblemViewProp = View.AddViewProperty( ProblemViewRel, ProblemProblemNTP );
					ProblemViewProp.Order = 1;
				}
				View.save();

			}

        }//Update()

    }//class CswUpdateSchemaTo01J11

}//namespace ChemSW.Nbt.Schema


