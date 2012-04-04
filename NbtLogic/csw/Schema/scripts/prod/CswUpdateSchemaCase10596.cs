using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Security;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 10596
    /// </summary>
    public class CswUpdateSchemaCase10596 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Sort Equipment and Assembly Tasks grid by Due Date
            CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClass AssemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );

            foreach( CswNbtMetaDataNodeType EquipmentNT in EquipmentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp EquipmentTaskGridNTP = EquipmentNT.getNodeTypeProp( "TaskGrid" );
                if( EquipmentTaskGridNTP != null )
                {
                    CswNbtView EquipmentTaskGridView = _CswNbtSchemaModTrnsctn.restoreView( EquipmentTaskGridNTP.ViewId );
                    if( EquipmentTaskGridView != null )
                    {
                        CswNbtViewProperty DueDateViewProp = EquipmentTaskGridView.findPropertyByName( CswNbtObjClassTask.DueDatePropertyName );
                        if( DueDateViewProp != null )
                        {
                            EquipmentTaskGridView.setSortProperty( DueDateViewProp, NbtViewPropertySortMethod.Ascending );
                            EquipmentTaskGridView.save();
                        }
                    }
                }
            } // foreach( CswNbtMetaDataNodeType EquipmentNT in EquipmentOC.getNodeTypes() )

            foreach( CswNbtMetaDataNodeType AssemblyNT in AssemblyOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp AssemblyTaskGridNTP = AssemblyNT.getNodeTypeProp( "Tasks Grid" );
                if( AssemblyTaskGridNTP != null )
                {
                    CswNbtView AssemblyTaskGridView = _CswNbtSchemaModTrnsctn.restoreView( AssemblyTaskGridNTP.ViewId );
                    if( AssemblyTaskGridView != null )
                    {
                        CswNbtViewProperty DueDateViewProp = AssemblyTaskGridView.findPropertyByName( CswNbtObjClassTask.DueDatePropertyName );
                        if( DueDateViewProp != null )
                        {
                            AssemblyTaskGridView.setSortProperty( DueDateViewProp, NbtViewPropertySortMethod.Ascending );
                            AssemblyTaskGridView.save();
                        }
                    }
                }
            } // foreach( CswNbtMetaDataNodeType AssemblyNT in AssemblyOC.getNodeTypes() )


        }//Update()

    }//class CswUpdateSchemaCase10596

}//namespace ChemSW.Nbt.Schema