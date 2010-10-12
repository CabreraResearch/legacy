using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public interface ICswNbtFieldTypeRule
    {

        string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilter );
        CswNbtSubFieldColl SubFields { get; }
        bool SearchAllowed { get; }
        string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode );
        void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropData PropertyValueToCheck );
        void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp );

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
