using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28873
    /// </summary>
    public class CswUpdateSchema_01W_Case28873 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28873; }
        }

        public override void update()
        {
            // New 'Locations' view

            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp LocationLocationOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );

            Int32 maxDepth = CswConvert.ToInt32( _CswNbtSchemaModTrnsctn.getConfigVariableValue( CswNbtResources.ConfigurationVariables.loc_max_depth.ToString() ) );
            if( maxDepth == Int32.MinValue )
            {
                maxDepth = 5;
            }
            string LocationsViewName = "Locations";
            CswNbtView LocationsView = _CswNbtSchemaModTrnsctn.restoreView( LocationsViewName, NbtViewVisibility.Global );
            if( null == LocationsView )
            {
                LocationsView = _CswNbtSchemaModTrnsctn.makeView();
                LocationsView.saveNew( LocationsViewName, NbtViewVisibility.Global );
                LocationsView.ViewMode = NbtViewRenderingMode.Tree;
            }

            LocationsView.Root.ChildRelationships.Clear();
            CswNbtViewRelationship LocRel1 = LocationsView.AddViewRelationship( LocationOC, true );
            LocationsView.AddViewPropertyAndFilter( LocRel1, LocationLocationOCP,
                                                    Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                    SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                                                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null );
            CswNbtViewRelationship LocReln = LocRel1;
            for( Int32 i = 2; i <= maxDepth; i++ )
            {
                LocReln = LocationsView.AddViewRelationship( LocReln, NbtViewPropOwnerType.Second, LocationLocationOCP, true );
            }
            LocationsView.save();
        } //Update()

    }//class CswUpdateSchema_01V_Case28873

}//namespace ChemSW.Nbt.Schema