using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28958
    /// </summary>
    public class CswUpdateSchema_01Y_Case28958 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28958; }
        }

        public override void update()
        {
            // Update existing 'Locations' view for Order change
            {
                CswNbtView LocationsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( "Locations", NbtViewVisibility.Global );
                if( null != LocationsView )
                {
                    CswNbtObjClassLocation.makeLocationsTreeView( ref LocationsView, _CswNbtSchemaModTrnsctn );
                    LocationsView.save();
                }
            }

            // Also fix the Equipment by Location view
            {
                CswNbtView EquipByLocView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( "Equipment By Location", NbtViewVisibility.Global );
                if( null != EquipByLocView )
                {
                    CswNbtObjClassLocation.makeLocationsTreeView( ref EquipByLocView, _CswNbtSchemaModTrnsctn );

                    CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentClass );
                    CswNbtMetaDataNodeType EquipmentNT = EquipmentOC.FirstNodeType;
                    CswNbtMetaDataNodeTypeProp EquipmentLocationNTP = null;
                    if( null != EquipmentNT )
                    {
                        EquipmentLocationNTP = EquipmentNT.getNodeTypeProp( "Location" );
                    }
                    CswNbtMetaDataObjectClass AssemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentAssemblyClass );
                    CswNbtMetaDataNodeType AssemblyNT = AssemblyOC.FirstNodeType;
                    CswNbtMetaDataNodeTypeProp AssemblyLocationNTP = null;
                    if( null != AssemblyNT )
                    {
                        AssemblyLocationNTP = AssemblyNT.getNodeTypeProp( "Location" );
                    }

                    foreach( CswNbtViewRelationship LocRel in EquipByLocView.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
                    {
                        if( null != EquipmentLocationNTP )
                        {
                            EquipByLocView.AddViewRelationship( LocRel, NbtViewPropOwnerType.Second, EquipmentLocationNTP, true );
                        }
                        if( null != AssemblyLocationNTP )
                        {
                            EquipByLocView.AddViewRelationship( LocRel, NbtViewPropOwnerType.Second, AssemblyLocationNTP, true );
                        }
                    }
                    EquipByLocView.save();
                }
            }
        } //Update()

    }//class CswUpdateSchema_01V_Case28958

}//namespace ChemSW.Nbt.Schema