using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using System.Xml;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace ChemSW.Nbt.WebServices
{
	/// <summary>
	/// Webservice for the table of components on the Welcome page
	/// </summary>
	public class CswNbtWebServiceInspections
	{
		private CswNbtResources _CswNbtResources;

		public CswNbtWebServiceInspections( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;

		}




	} // class CswNbtWebServiceInspections
} // namespace ChemSW.Nbt.WebServices

