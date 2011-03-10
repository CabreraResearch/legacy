using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
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

        public XElement getSearchProps( CswNbtView View )
        {
            var ReturnNode = new XElement( "search" );
            return ReturnNode;
        }

        public XElement getSearchViews(ICswNbtUser Userid, bool ForMobile, string OrderBy)
        {
            var ReturnNode = new XElement( "search" );
            if( null != Userid )
            {
                ReturnNode = _CswNbtResources.ViewSelect.getSearchableViews( Userid, ForMobile, OrderBy);
            }
            return ReturnNode;
        }
	} // class CswNbtWebServiceSearch

} // namespace ChemSW.Nbt.WebServices
