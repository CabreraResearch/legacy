using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Csw.WebSvc;

namespace ChemSW.Nbt.PropTypes
{
    [DataContract, KnownType( typeof( CswNbtWcfProperty ) )]
    public class CswNbtWcfPropCollection
    {
        [DataMember]
        public CswAjaxDictionary<CswNbtWcfProperty> properties = new CswAjaxDictionary<CswNbtWcfProperty>();

        public void addProperty( CswNbtWcfProperty newProp )
        {
            if( false == string.IsNullOrEmpty( newProp.OriginalPropName ) )
            {
                properties.Add( newProp.OriginalPropName, newProp );
            }
            else
            {
                properties.Add( newProp.PropName, newProp );
            }
        }
    }

}//namespace ChemSW.Nbt.PropTypes
