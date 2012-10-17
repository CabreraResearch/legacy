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
    /// Schema Update for case 28002
    /// </summary>
    public class CswUpdateSchema_01S_Case28002 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Find and remove invalid property references
            Collection<CswNbtMetaDataNodeTypeProp> InvalidPropRefs = new Collection<CswNbtMetaDataNodeTypeProp>();
            foreach( CswNbtMetaDataNodeTypeProp PropRef in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.PropertyReference ) )
            {
                if( Int32.MinValue == PropRef.ObjectClassPropId ) // can't delete ones that derive from object class props
                {
                    if( NbtViewPropIdType.NodeTypePropId.ToString() == PropRef.FKType )
                    {
                        if( null == _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( PropRef.FKValue ) )
                        {
                            InvalidPropRefs.Add( PropRef );
                        }
                    }
                    else if( NbtViewPropIdType.ObjectClassPropId.ToString() == PropRef.FKType )
                    {
                        if( null == _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( PropRef.FKValue ) )
                        {
                            InvalidPropRefs.Add( PropRef );
                        }
                    }

                } // if( Int32.MinValue == PropRef.ObjectClassPropId )
            } // foreach( CswNbtMetaDataNodeTypeProp PropRef in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.PropertyReference ) )

            foreach( CswNbtMetaDataNodeTypeProp InvalidPropRef in InvalidPropRefs )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( InvalidPropRef );
            }
        }//Update()

    }//class CswUpdateSchemaCase28002

}//namespace ChemSW.Nbt.Schema