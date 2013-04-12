using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Grid.ExtJs;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.Tree;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.Actions
{




    /// <summary>
    /// Holds logic for deleting demo data
    /// </summary>
    public class  CswNbtActDeleteDemoData
    {

        private CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public  CswNbtActDeleteDemoData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

    } // class   CswNbtActDeleteDemoData
}// namespace ChemSW.Nbt.Actions