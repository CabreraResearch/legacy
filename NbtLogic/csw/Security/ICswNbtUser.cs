using System.Data;
using System.Collections;
using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;
using ChemSW.Core;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Security
{
    public interface ICswNbtUser : ICswUser
    {
        CswPrimaryKey UserId { get; }
        CswPrimaryKey RoleId { get; }
        void postChanges( bool ForceUpdate ); //bz# 5446
        bool CheckPermission(NodeTypePermission Permission, Int32 NodeTypeId, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp);
        bool CheckCreatePermission( Int32 NodeTypeId );
        bool CheckActionPermission( CswNbtActionName ActionName );
        CswNbtNodePropText UsernameProperty { get; }
        string Username { get; }
        CswNbtNodePropPassword PasswordProperty { get; }
        string Rolename { get; }
        CswNbtObjClassRole RoleNode { get; }
        CswNbtObjClassUser UserNode { get; }
        bool IsAdministrator();
        //bool IsDesigner();
        //Int32 DefaultViewId { get; }   // BZ 9934
        CswNbtNodePropText FirstNameProperty { get; }
        CswNbtNodePropText LastNameProperty { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        CswNbtNodePropText EmailProperty { get; }

    }//ICswNbtUser
}//namespace ChemSW.Nbt

