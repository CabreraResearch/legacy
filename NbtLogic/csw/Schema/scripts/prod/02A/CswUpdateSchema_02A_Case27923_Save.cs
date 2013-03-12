using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02A_Case27923_Save: CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 27923; }
        }

        public override void update()
        {
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp SaveNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClass.PropertyName.Save );
                if( null != SaveNtp )
                {
                    SaveNtp.removeFromAllLayouts();
                    SaveNtp.updateLayout(LayoutType: CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, DoMove: false, DisplayColumn: 1, DisplayRow: Int32.MaxValue );

                    foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.getNodeTypeTabs() )
                    {
                        if( Tab != NodeType.getIdentityTab() )
                        {
                            SaveNtp.updateLayout( LayoutType : CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove : false, DisplayColumn : 1, DisplayRow: Int32.MaxValue, TabId: Tab.TabId );        
                        }
                    }
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_CaseXXXXX

}//namespace ChemSW.Nbt.Schema