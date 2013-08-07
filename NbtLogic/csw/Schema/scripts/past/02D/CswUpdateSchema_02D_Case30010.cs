using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30010
    /// </summary>
    public class CswUpdateSchema_02D_Case30010 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30010; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RegulatoryListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegulatoryListOC )
            {
                foreach( CswNbtMetaDataNodeType CurrentRegulatoryListNT in RegulatoryListOC.getNodeTypes() )
                {
                    Int32 TabId = CurrentRegulatoryListNT.getFirstNodeTypeTab().TabId;

                    // Name
                    CswNbtMetaDataNodeTypeProp NameNTP = CurrentRegulatoryListNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.Name );
                    NameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                    NameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );

                    // List Mode
                    CswNbtMetaDataNodeTypeProp ListModeNTP = CurrentRegulatoryListNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.ListMode );
                    ListModeNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                    ListModeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );

                    // Add CAS Numbers
                    CswNbtMetaDataNodeTypeProp AddCASNumbersNTP = CurrentRegulatoryListNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.AddCASNumbers );
                    AddCASNumbersNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                    AddCASNumbersNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                    AddCASNumbersNTP.setFilter( FilterProp: ListModeNTP,
                                                SubField: ListModeNTP.getFieldTypeRule().SubFields.Default,
                                                FilterMode: CswEnumNbtFilterMode.Equals,
                                                FilterValue: CswNbtObjClassRegulatoryList.CswEnumRegulatoryListListModes.ManuallyManaged );

                    // Exclusive
                    CswNbtMetaDataNodeTypeProp ExclusiveNTP = CurrentRegulatoryListNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.Exclusive );
                    ExclusiveNTP.removeFromAllLayouts();

                }//foreach( CswNbtMetaDataNodeType CurrentRegulatoryListNT in RegulatoryListOC.getNodeTypes() )

            }//if( null != RegulatoryListOC )

        } // update()

    }//class CswUpdateSchema_02C_Case30010

}//namespace ChemSW.Nbt.Schema