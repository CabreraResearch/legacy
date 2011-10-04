using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Security;

namespace ChemSW.Nbt.Security
{
    public interface ICswNbtUser : ICswUser
    {
        //CswPrimaryKey UserId { get; }
        //CswPrimaryKey RoleId { get; }
        void postChanges( bool ForceUpdate ); //bz# 5446
		//bool CheckPermission( NodeTypePermission Permission, Int32 NodeTypeId, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp );
		//bool CheckCreatePermission( Int32 NodeTypeId );
		//bool CheckActionPermission( CswNbtActionName ActionName );
        CswNbtNodePropText UsernameProperty { get; }
        //string Username { get; }
        CswNbtNodePropPassword PasswordProperty { get; }
        //string Rolename { get; }
        CswNbtObjClassRole RoleNode { get; }
        CswNbtObjClassUser UserNode { get; }
        bool IsAdministrator();
        //bool canEditPassword( CswNbtNode UserNode );
        //bool IsDesigner();
        //Int32 DefaultViewId { get; }   // BZ 9934
        CswNbtNodePropText FirstNameProperty { get; }
        CswNbtNodePropText LastNameProperty { get; }
        //string FirstName { get; }
        //string LastName { get; }
        string Email { get; }
        CswNbtNodePropText EmailProperty { get; }
        Int32 PageSize { get; }

    }//ICswNbtUser
}//namespace ChemSW.Nbt

