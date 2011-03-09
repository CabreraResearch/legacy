using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceSearch
	{
		private readonly CswNbtResources _CswNbtResources;
		
		public CswNbtWebServiceSearch( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

        public XElement getSearch( CswNbtView View )
        {
            var ReturnNode = new XElement( "root" );
            return ReturnNode;
        }
	} // class CswNbtWebServiceSearch

} // namespace ChemSW.Nbt.WebServices
