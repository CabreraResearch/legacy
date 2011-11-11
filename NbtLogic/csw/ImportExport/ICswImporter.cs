using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.ImportExport
{

    public enum ImportMode
    {
        Unknown,
        /// <summary>
        /// Make changes to existing data.  Unmatched data is ignored.
        /// </summary>
        Update,
        /// <summary>
        /// Make new data.  Matched data is ignored.
        /// </summary>
        Duplicate,
        /// <summary>
        /// Make changes to existing data, and make new when unmatched.
        /// </summary>
        Overwrite

    } // ImportMode

    public enum ImportAlgorithm { Legacy, Experimental }

    public interface ICswImporter
    {

        void ImportXml( ImportMode IMode, ref string ViewXml, ref string ResultXml, ref string ErrorLog );
        void stop(); 

    } // ICswImporter
} // namespace ChemSW.Nbt
