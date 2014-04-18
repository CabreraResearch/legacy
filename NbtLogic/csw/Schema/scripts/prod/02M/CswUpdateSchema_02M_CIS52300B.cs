using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52300B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52300; }
        }

        public override string Title
        {
            get { return "Filter out obsolete methods on default method view" + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtView MethodView = _CswNbtSchemaModTrnsctn.restoreView( "Methods" );

            CswNbtMetaDataObjectClass MethodOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass );

            CswNbtMetaDataObjectClassProp ObsoleteClassProp = MethodOC.getObjectClassProp( CswNbtObjClassMethod.PropertyName.Obsolete );

            CswNbtViewRelationship ParentViewRelationship = MethodView.getAllNbtViewRelationships()[0];

            MethodView.AddViewPropertyAndFilter( ParentViewRelationship,
                                                 ObsoleteClassProp,
                                                 Value: CswEnumTristate.True.ToString(),
                                                 FilterMode: CswEnumNbtFilterMode.NotEquals);
            MethodView.save();
        }
    }
}