using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Linq;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28258
    /// </summary>
    public class CswUpdateSchema_01W_Case28258 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28258; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass locationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );

            foreach( CswNbtMetaDataNodeType locationNT in locationOC.getNodeTypes() )
            {
                int tabOrderIdx = 3;
                foreach( CswNbtMetaDataNodeTypeTab tab in locationNT.getNodeTypeTabs().OrderBy( tab => tab.TabOrder ) ) //Preserve the current order of existing tabs
                {
                    if( tab.TabName.Equals( locationNT.NodeTypeName ) )
                    {
                        tab.TabOrder = 1; //Make the NT Name tab first
                    }
                    else if( tab.TabName.Equals( "Inventory Levels" ) )
                    {
                        tab.TabOrder = 2; //Make the Inv Levels tab second
                    }
                    else if( tab.TabName.Equals( "Containers" ) )
                    {
                        tab.TabOrder = 15; //Make the Containers tab last
                    }
                    else if( false == tab.TabName.Equals( "Identity" ) )
                    {
                        tab.TabOrder = tabOrderIdx;
                        tabOrderIdx++;
                    }
                }
            }

        } //Update()

    }//class CswUpdateSchema_01W_Case28258

}//namespace ChemSW.Nbt.Schema