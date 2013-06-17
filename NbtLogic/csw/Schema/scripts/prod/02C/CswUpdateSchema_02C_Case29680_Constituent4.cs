using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29680
    /// </summary>
    public class CswUpdateSchema_02C_Case29680_Constituent4 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29680; }
        }

        public override void update()
        {
            // add active to constituent grid
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClass ComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ChemicalComponentsNTP = ChemicalNT.getNodeTypeProp( "Components" );
                if( null != ChemicalComponentsNTP )
                {
                    CswNbtView ComponentsView = _CswNbtSchemaModTrnsctn.restoreView( ChemicalComponentsNTP.ViewId );
                    if( ComponentsView.Root.ChildRelationships.Count > 0 )
                    {
                        CswNbtViewRelationship ChemRel = ComponentsView.Root.ChildRelationships[0];
                        if( ChemRel.ChildRelationships.Count > 0 )
                        {
                            CswNbtViewRelationship CompRel = ChemRel.ChildRelationships[0];
                            Int32 order = 1;
                            foreach( CswNbtViewProperty vp in CompRel.Properties )
                            {
                                vp.Order = order;
                                order++;
                            }
                            ComponentsView.AddViewProperty( CompRel, ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Active ), order );
                        }
                    }
                    ComponentsView.save();

                } // if( null != ChemicalComponentsNTP )
            } // foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
        } // update()

    }//class CswUpdateSchema_02C_Case29680_Constituent4

}//namespace ChemSW.Nbt.Schema