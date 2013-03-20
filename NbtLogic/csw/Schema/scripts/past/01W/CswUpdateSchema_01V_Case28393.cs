using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28393
    /// </summary>
    public class CswUpdateSchema_01V_Case28393 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28393; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                CswNbtView MHCView = _CswNbtSchemaModTrnsctn.makeNewView( "Missing Hazard Classes", NbtViewVisibility.Global );
                MHCView.Category = "Materials";
                MHCView.Visibility = NbtViewVisibility.Global;
                MHCView.ViewMode = NbtViewRenderingMode.Tree;
            
                CswNbtViewRelationship RootRel = MHCView.AddViewRelationship( ChemicalNT, true );

                CswNbtMetaDataNodeTypeProp SpecialFlagsNTP = ChemicalNT.getNodeTypeProp( "Special Flags" );
                CswNbtViewProperty SpecialFlagsVP = MHCView.AddViewProperty( RootRel, SpecialFlagsNTP );
                MHCView.AddViewPropertyFilter( SpecialFlagsVP,
                                                CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                CswNbtPropFilterSql.FilterResultMode.Hide,
                                                CswNbtSubField.SubFieldName.Value,
                                                CswNbtPropFilterSql.PropertyFilterMode.NotContains,
                                                "Not Reportable");

                CswNbtMetaDataNodeTypeProp HazardClassesNTP = ChemicalNT.getNodeTypeProp( "Hazard Classes" );
                CswNbtViewProperty HazardClassesVP = MHCView.AddViewProperty( RootRel, HazardClassesNTP );
                MHCView.AddViewPropertyFilter( HazardClassesVP,
                                                CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                CswNbtPropFilterSql.FilterResultMode.Hide,
                                                CswNbtSubField.SubFieldName.Value,
                                                CswNbtPropFilterSql.PropertyFilterMode.Null);
                MHCView.save();
            }
        }//Update()

    }//class CswUpdateSchemaCase_01V_28393

}//namespace ChemSW.Nbt.Schema