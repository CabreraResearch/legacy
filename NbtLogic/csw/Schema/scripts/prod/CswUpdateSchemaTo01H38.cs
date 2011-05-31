using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-38
    /// </summary>
    public class CswUpdateSchemaTo01H38 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 38 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H38( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
	        // case 21761
   			// Reported By's relationship view is screwy in master data

			CswNbtMetaDataObjectClass ProblemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass);
			foreach(CswNbtMetaDataNodeType ProblemNT in ProblemOC.NodeTypes)
			{
				CswNbtMetaDataNodeTypeProp ReportedByNTP = ProblemNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ReportedByPropertyName );

				CswNbtView NewView = null;
				if( ReportedByNTP.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
				{
					CswNbtMetaDataNodeType ReportedByTargetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ReportedByNTP.FKValue );
					NewView = ReportedByTargetNT.CreateDefaultView();
				}
				else if( ReportedByNTP.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
				{
					CswNbtMetaDataObjectClass ReportedByTargetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ReportedByNTP.FKValue );
					NewView = ReportedByTargetOC.CreateDefaultView();
				}

				if( NewView != null )
				{
					NewView.ViewId = ReportedByNTP.ViewId;
					NewView.Visibility = NbtViewVisibility.Property;
					NewView.ViewMode = NbtViewRenderingMode.List;
					NewView.ViewName = ReportedByNTP.PropName;
					NewView.save();
				}
			}
        } // update()

    }//class CswUpdateSchemaTo01H38

}//namespace ChemSW.Nbt.Schema

