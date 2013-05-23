using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    //TODO: Remove this class
    public class CswNbtNodePropLocationContents : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropLocationContents( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsLocationContents;
        }

        public CswNbtNodePropLocationContents( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            // No subfields
        }

        override public bool Empty
        {
            get { return ( 0 == Gestalt.Length ); }
        }

        override public string Gestalt
        {
            get { return _CswNbtNodePropData.Gestalt; }
        }//Gestalt

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

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
            //TODO: Remove this class
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //TODO: Remove this class
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //TODO: Remove this class
        }

        public override void SyncGestalt()
        {
            //TODO: Remove this class
        }
    }

}//namespace 
