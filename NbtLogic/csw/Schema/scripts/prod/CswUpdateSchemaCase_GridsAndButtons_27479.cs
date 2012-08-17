using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27479
    /// </summary>
    public class CswUpdateSchemaCase_GridsAndButtons_27479 : CswUpdateSchemaTo
    {
        private void addButtonsRecursively(CswNbtView View, Collection<CswNbtViewRelationship> Relationships )
        {
            foreach( CswNbtViewRelationship ChildRelationship in Relationships )
            {
                View.AddViewPropertyByFieldType(ChildRelationship, ChildRelationship.SecondMetaDataObject(), CswNbtMetaDataFieldType.NbtFieldType.Button);
                if(ChildRelationship.ChildRelationships.Count > 0)
                {
                    addButtonsRecursively( View, ChildRelationship.ChildRelationships );
                }
            }
        }

        public override void update()
        {
            foreach( CswNbtView GridView in _CswNbtSchemaModTrnsctn.restoreAllViewsOfMode( NbtViewRenderingMode.Grid ) )
            {
                //addButtonsRecursively(GridView, GridView.Root.ChildRelationships);
                //GridView.save();
            }
            // This is a placeholder script that does nothing.
        }//Update()

    }//class CswUpdateSchemaCase_GridsAndButtons_27479

}//namespace ChemSW.Nbt.Schema