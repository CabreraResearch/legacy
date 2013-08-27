﻿using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case29402 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29402; }
        }

        public override void update()
        {
            HashSet<CswNbtViewId> SeenViewIds = new HashSet<CswNbtViewId>();

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(CswEnumNbtObjectClass.ChemicalClass);
            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp ConstituentOCP = MaterialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            CswNbtMetaDataObjectClassProp PercentageOCP = MaterialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Percentage);

            CswNbtMetaDataNodeType ConstituentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Constituent" );
            if( null != ConstituentNT )
            {
                CswNbtMetaDataNodeTypeProp ConstituentCASNoNTP = ConstituentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.CasNo );

                foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp ConstituentNTP = ChemicalNT.getNodeTypeProp( "Components" );
                    if( null != ConstituentNTP )
                    {
                        if( false == SeenViewIds.Contains( ConstituentNTP.ViewId ) )
                        {
                            CswNbtView ConstituentsPropView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( ConstituentNTP.ViewId );
                            CswNbtViewRelationship MaterialComponentRelationship = ConstituentsPropView.Root.ChildRelationships[0].ChildRelationships[0]; //Intentionally get the 2nd lvl parent
                            MaterialComponentRelationship.Properties.Clear();
                            MaterialComponentRelationship.ChildRelationships.Clear();

                            ConstituentsPropView.AddViewProperty( MaterialComponentRelationship, ConstituentOCP, 1 );

                            CswNbtViewRelationship ConstituentRelationship = ConstituentsPropView.AddViewRelationship( MaterialComponentRelationship, CswEnumNbtViewPropOwnerType.First, ConstituentOCP, false );
                            ConstituentsPropView.AddViewProperty( ConstituentRelationship, ConstituentCASNoNTP, 2 );

                            ConstituentsPropView.AddViewProperty( MaterialComponentRelationship, PercentageOCP, 3 );

                            ConstituentsPropView.save();
                            SeenViewIds.Add( ConstituentNTP.ViewId );
                        }
                    }
                }
            }


        } // update()

    }

}//namespace ChemSW.Nbt.Schema