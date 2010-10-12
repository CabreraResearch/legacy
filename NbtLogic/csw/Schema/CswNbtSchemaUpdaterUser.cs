//using System;
//using System.Collections.Generic;
//using System.Text;
//using ChemSW.Nbt;
//using ChemSW.Nbt.PropTypes;
//using ChemSW.Nbt.ObjClasses;
//using ChemSW.Security;
//using ChemSW.Core;
//using ChemSW.Nbt.MetaData;
//using ChemSW.TblDn;


//namespace ChemSW.Nbt.Schema
//{
//    /// <summary>
//    /// Schema Updater's user object.  Schema Updater can do anything it wants.
//    /// </summary>
//    public class CswNbtSchemaUpdaterUser : ICswNbtUser
//    {
//        public bool CheckCreatePermission( int NodeTypeId )
//        {
//            return true;
//        }

//        public bool CheckPermission( NodeTypePermission Permission, int NodeTypeId, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp )
//        {
//            return true;
//        }

//        // BZ 9934 - No need for 'default view' anymore
//        //public Int32 DefaultViewId { get { return Int32.MinValue; } }

//        public bool IsAdministrator() { return true; }

//        public CswNbtObjClass RoleNode { get { return null; } }
//        public CswNbtObjClass UserNode { get { return null; } }

//        public CswNbtNodePropText UsernameProperty { get { return null; } }
//        public CswNbtNodePropPassword PasswordProperty { get { return null; } }
//        public CswNbtNodePropText FirstNameProperty { get { return null; } }
//        public CswNbtNodePropText LastNameProperty { get { return null; } }

//        public CswPrimaryKey RoleId { get { return null; } }
//        public Int32 RoleTimeout { get { return Int32.MinValue; } }
//        public CswPrimaryKey UserId { get { return null; } }
//        public string Username { get { return string.Empty; } }
//        public string Rolename { get { return string.Empty; } }
//        public string FirstName { get { return string.Empty; } }
//        public string LastName { get { return string.Empty; } }

//        public void postChanges( bool ForceUpdate )
//        {
//            // do nothing
//        }
//    }
//}
