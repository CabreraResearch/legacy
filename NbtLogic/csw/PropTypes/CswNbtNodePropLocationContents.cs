using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropLocationContents : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropLocationContents( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsLocationContents;
        }

        public CswNbtNodePropLocationContents( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            //if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.LocationContents )
            //{
            //    throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
            //                                "CswNbtNodePropLocationContents() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            //}
        }//CswNbtNodePropLocationContents()

        override public bool Empty
        {
            get { return ( 0 == Gestalt.Length ); }
        }

        override public string Gestalt
        {
            get { return _CswNbtNodePropData.Gestalt; }
        }//Gestalt

        public CswNbtView View
        {
            get
            {
                CswNbtView Ret = null;
                if( _CswNbtMetaDataNodeTypeProp.ViewId.isSet() )
                    Ret = _CswNbtResources.ViewSelect.restoreView( _CswNbtMetaDataNodeTypeProp.ViewId );
                return Ret;
            }
        }

        public override void ToJSON( JObject ParentObject )
        {
            // Nothing to save
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }

        public override void SyncGestalt()
        {

        }
    }

}//namespace 
