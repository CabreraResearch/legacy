using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Security;
using ChemSW.TblDn;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Security
{
    public class CswNbtSystemUser : ICswNbtUser
    {
        private CswNbtResources _CswNbtResources;
        private string _Username;

        public CswNbtSystemUser( CswNbtResources CswNbtResources, string Username )
        {
            _CswNbtResources = CswNbtResources;
            _Username = Username;
        }

        public bool CheckCreatePermission( int NodeTypeId )
        {
            return true;
        }
        public bool CheckActionPermission( CswNbtActionName ActionName )
        {
            return true;
        }

        public bool CheckPermission( NodeTypePermission Permission, int NodeTypeId, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp )
        {
            return true;
        }

        public bool IsAdministrator() { return true; }

        public CswNbtObjClassRole RoleNode { get { return null; } }
        public CswNbtObjClassUser UserNode { get { return null; } }

        public CswPrimaryKey RoleId { get { return null; } }
        public CswPrimaryKey UserId { get { return null; } }

        public CswNbtNodePropText UsernameProperty { get { return null; } }
        public CswNbtNodePropPassword PasswordProperty { get { return null; } }
        public CswNbtNodePropText FirstNameProperty { get { return null; } }
        public CswNbtNodePropText LastNameProperty { get { return null; } }
        public CswNbtNodePropText EmailProperty { get { return null; } }

        public Int32 RoleTimeout { get { return Int32.MinValue; } }

        public string Username { get { return _Username; } }
        public string Rolename { get { return string.Empty; } }
        public string FirstName { get { return string.Empty; } }
        public string LastName { get { return string.Empty; } }
        public string Email { get { return string.Empty; } }

		public Int32 PageSize { get { return 50; } }

        public void postChanges( bool ForceUpdate )
        {
            // do nothing
        }
    }
}
