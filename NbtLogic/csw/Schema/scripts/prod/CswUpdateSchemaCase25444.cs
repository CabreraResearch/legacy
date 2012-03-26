using System;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25444
    /// </summary>
    public class CswUpdateSchemaCase25444 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Fix redundant values in multilist properties
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
            {
                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeType.getNodeTypeProps() )
                {
                    if( NodeTypeProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.MultiList )
                    {
                        foreach( CswNbtNode Node in NodeType.getNodes( false, true ) )
                        {
                            Node.Properties[NodeTypeProp].AsMultiList.ValidateValues();
                            Node.postChanges( false );
                        }
                    }
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase25444

}//namespace ChemSW.Nbt.Schema