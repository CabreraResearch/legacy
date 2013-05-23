using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29314
    /// </summary>
    public class CswUpdateSchema_02C_Case29314 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29314; }
        }

        public override void update()
        {
            // Add Design NodeType Properties to the table layout and set their row and column values
            CswNbtMetaDataNodeType DesignNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Design NodeType" );
            if( null != DesignNodeType )
            {
                foreach( CswNbtMetaDataNodeTypeProp NTProps in DesignNodeType.getNodeTypeProps() )
                {
                    if( NTProps.PropName.Equals( CswNbtObjClassDesignNodeType.PropertyName.Category ) )
                    {
                        NTProps.updateLayout( CswEnumNbtLayoutType.Table, true, Int32.MinValue, 1, 1 );
                    }
                    else if( NTProps.PropName.Equals( CswNbtObjClassDesignNodeType.PropertyName.NameTemplate ) )
                    {
                        NTProps.updateLayout( CswEnumNbtLayoutType.Table, true, Int32.MinValue, 2, 1 );
                    }
                    else if( NTProps.PropName.Equals( CswNbtObjClassDesignNodeType.PropertyName.ObjectClass ) )
                    {
                        NTProps.updateLayout( CswEnumNbtLayoutType.Table, true, Int32.MinValue, 3, 1 );
                    }
                    else
                    {
                        NTProps.removeFromLayout( CswEnumNbtLayoutType.Table );
                    }
                }
            }



        } // update()

    }//class CswUpdateSchema_02B_Case29314

}//namespace ChemSW.Nbt.Schema