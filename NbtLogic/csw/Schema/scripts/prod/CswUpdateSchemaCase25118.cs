using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25118
    /// </summary>
    public class CswUpdateSchemaCase25118 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //Create Time Unit view and apply it to every Expiration Interval ObjectClassprop

            //Create the new view
            CswNbtMetaDataNodeType TimeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Time Unit" );

            //Set ExpirationInterval ObjectClassProp default FKValue to Time Unit NodeType (so that new material NodeTypes will start with this Unit NodeType restriction)
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp ExpirationInterval = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( MaterialOC.ObjectClassId, CswNbtObjClassMaterial.ExpirationIntervalPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ExpirationInterval, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isfk, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ExpirationInterval, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.NodeTypeId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ExpirationInterval, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, TimeNT.NodeTypeId );

            //Update existing ExpirationInterval NodeTypeProps to use new Time view
            foreach( CswNbtMetaDataNodeType NodeType in MaterialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ExpInt = NodeType.getNodeTypeProp( "Expiration Interval" );
                CswNbtView TimeView = _CswNbtSchemaModTrnsctn.restoreView( ExpInt.ViewId );
                TimeView.Root.ChildRelationships.Clear();
                TimeView.ViewName = "CswNbtNodeTypePropExpirationInterval_" + ExpInt.PropId.ToString();
                TimeView.AddViewRelationship( TimeNT, true );
                TimeView.save();
                ExpInt.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), TimeNT.NodeTypeId );
            }

        }//Update()

    }//class CswUpdateSchemaCase25118

}//namespace ChemSW.Nbt.Schema