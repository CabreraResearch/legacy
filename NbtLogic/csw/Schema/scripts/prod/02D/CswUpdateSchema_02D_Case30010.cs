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

                    foreach( CswNbtMetaDataNodeTypeProp CurrentRegListNTP in CurrentRegulatoryListNT.getNodeTypeProps() )
                    {
                        // Name
                        if( CurrentRegListNTP.PropName == CswNbtObjClassRegulatoryList.PropertyName.Name )
                        {
                            CurrentRegListNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                            CurrentRegListNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
                        }

                        // List Mode
                        if( CurrentRegListNTP.PropName == CswNbtObjClassRegulatoryList.PropertyName.ListMode )
                        {
                            CurrentRegListNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                            CurrentRegListNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
                        }

                        // Add CAS Numbers
                        if( CurrentRegListNTP.PropName == CswNbtObjClassRegulatoryList.PropertyName.AddCASNumbers )
                        {
                            CurrentRegListNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                            CurrentRegListNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                        }

                        // Exclusive
                        if( CurrentRegListNTP.PropName == CswNbtObjClassRegulatoryList.PropertyName.Exclusive )
                        {
                            CurrentRegListNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 4, DisplayColumn: 1 );
                            CurrentRegListNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
                        }


                    }//foreach( CswNbtMetaDataNodeTypeProp CurrentRegListNTP in CurrentRegulatoryListNT.getNodeTypeProps() )

                }//foreach( CswNbtMetaDataNodeType CurrentRegulatoryListNT in RegulatoryListOC.getNodeTypes() )

            }//if( null != RegulatoryListOC )

        } // update()

    }//class CswUpdateSchema_02C_Case30010

}//namespace ChemSW.Nbt.Schema