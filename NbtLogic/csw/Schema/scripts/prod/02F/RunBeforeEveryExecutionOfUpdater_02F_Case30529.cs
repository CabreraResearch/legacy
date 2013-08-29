using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30529 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 30529"; } }
        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30529; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtView UoMView = UoMOC.CreateDefaultView();
            CswNbtMetaDataObjectClass RegListCasNoOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass );
            CswNbtMetaDataObjectClassProp TPQOCP = RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.TPQ );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TPQOCP, CswEnumNbtObjectClassPropAttributes.isfk, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TPQOCP, CswEnumNbtObjectClassPropAttributes.fktype, CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TPQOCP, CswEnumNbtObjectClassPropAttributes.fkvalue, UoMOC.ObjectClassId );
            foreach( CswNbtMetaDataNodeType RegListCasNoNT in RegListCasNoOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp TPQNTP = RegListCasNoNT.getNodeTypePropByObjectClassProp( TPQOCP );
                TPQNTP.ViewId = UoMView.ViewId;
                TPQNTP.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), UoMOC.ObjectClassId );
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30281
}//namespace ChemSW.Nbt.Schema


