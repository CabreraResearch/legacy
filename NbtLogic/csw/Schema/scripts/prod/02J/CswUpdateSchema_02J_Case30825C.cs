using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case30825C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30825; }
        }

        public override string Title
        {
            get { return ""; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RegulatoryListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegulatoryListOC )
            {
                foreach( CswNbtMetaDataNodeType CurrentRegulatoryListNT in RegulatoryListOC.getNodeTypes() )
                {
                    Int32 TabId = CurrentRegulatoryListNT.getFirstNodeTypeTab().TabId;

                    CswNbtMetaDataNodeTypeProp ListModeNTP = CurrentRegulatoryListNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.ListMode );
                    CswNbtMetaDataNodeTypeProp RegionsNTP = CurrentRegulatoryListNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.Regions );
                    RegionsNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                    RegionsNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                    RegionsNTP.setFilter( FilterProp: ListModeNTP,
                                        SubField: ListModeNTP.getFieldTypeRule().SubFields.Default,
                                        FilterMode: CswEnumNbtFilterMode.Equals,
                                        FilterValue: CswNbtObjClassRegulatoryList.CswEnumRegulatoryListListModes.ArielManaged );

                }//foreach( CswNbtMetaDataNodeType CurrentRegulatoryListNT in RegulatoryListOC.getNodeTypes() )

            }//if( null != RegulatoryListOC )
        } // update()

    }

}//namespace ChemSW.Nbt.Schema