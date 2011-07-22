
$.CswFieldTypeFactory = function (method)
{
	var m = {
		nodeid: '',
		fieldtype: '',
		'$propdiv': '',
		'$savebtn': '',
		'$propxml': '',
		onchange: function () { },
		onReload: function () { },    // if a control needs to reload the tab
		cswnbtnodekey: '',
		'ID': '',
		'Required': '',
		'ReadOnly': '',
		'EditMode': EditMode.Edit.name,
		'onEditView': function (viewid) { }
	};

	var methods = {
		'make': function (options)
		{
			if (options)
			{
				$.extend(m, options);
			}
			m.ID = m.$propxml.CswAttrXml('id');
			m.Required = (m.$propxml.CswAttrXml('required') === "true") || m.Required;
			m.ReadOnly = (m.$propxml.CswAttrXml('readonly') === "true") || m.ReadOnly || m.EditMode === EditMode.PrintReport.name;

			switch (m.fieldtype)
			{
				case "AuditHistoryGrid":
					m.$propdiv.CswFieldTypeAuditHistoryGrid('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Barcode":
					m.$propdiv.CswFieldTypeBarcode('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Composite":
					m.$propdiv.CswFieldTypeComposite('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Date":
					m.$propdiv.CswFieldTypeDate('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "File":
					m.$propdiv.CswFieldTypeFile('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Grid":
					m.$propdiv.CswFieldTypeGrid('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Image":
					m.$propdiv.CswFieldTypeImage('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Link":
					m.$propdiv.CswFieldTypeLink('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "List":
					m.$propdiv.CswFieldTypeList('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Location":
					m.$propdiv.CswFieldTypeLocation('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "LocationContents":
					m.$propdiv.CswFieldTypeLocationContents('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Logical":
					m.$propdiv.CswFieldTypeLogical('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "LogicalSet":
					m.$propdiv.CswFieldTypeLogicalSet('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Memo":
					m.$propdiv.CswFieldTypeMemo('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "MTBF":
					m.$propdiv.CswFieldTypeMTBF('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "NodeTypeSelect":
					m.$propdiv.CswFieldTypeNodeTypeSelect('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Number":
					m.$propdiv.CswFieldTypeNumber('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Password":
					m.$propdiv.CswFieldTypePassword('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "PropertyReference":
					m.$propdiv.CswFieldTypePropertyReference('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Quantity":
					m.$propdiv.CswFieldTypeQuantity('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Question":
					m.$propdiv.CswFieldTypeQuestion('init', m); //'init', nodeid, $propxml, onchange
					break;
				case "Relationship":
					m.$propdiv.CswFieldTypeRelationship('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "Scientific":
					m.$propdiv.CswFieldTypeScientific('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "Sequence":
					m.$propdiv.CswFieldTypeSequence('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "Static":
					m.$propdiv.CswFieldTypeStatic('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "Text":
					m.$propdiv.CswFieldTypeText('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "Time":
					m.$propdiv.CswFieldTypeTime('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "TimeInterval":
					m.$propdiv.CswFieldTypeTimeInterval('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "UserSelect":
					m.$propdiv.CswFieldTypeUserSelect('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "ViewPickList":
					m.$propdiv.CswFieldTypeViewPickList('init', m); //('init', nodeid, $propxml, onchange);
					break;
				case "ViewReference":
					m.$propdiv.CswFieldTypeViewReference('init', m); //('init', nodeid, $propxml, onchange);
					break;
				default:
					m.$propdiv.append(m.$propxml.CswAttrXml('init', m)); //('gestalt'));
					break;
			} // switch (fieldtype)
		}, // make

		'save': function (options)
		{
			if (options)
			{
				$.extend(m, options);
			}
			m.ID = m.$propxml.CswAttrXml('id');
			m.Required = (m.$propxml.CswAttrXml('required') === "true");
			m.ReadOnly = (m.$propxml.CswAttrXml('readonly') === "true");

			switch (m.fieldtype)
			{
				case "Barcode":
					m.$propdiv.CswFieldTypeBarcode('save', m); //('save', $propdiv, $propxml);
					break;
				case "Composite":
					m.$propdiv.CswFieldTypeComposite('save', m); //('save', $propdiv, $propxml);
					break;
				case "Date":
					m.$propdiv.CswFieldTypeDate('save', m); //('save', $propdiv, $propxml);
					break;
				case "File":
					m.$propdiv.CswFieldTypeFile('save', m); //('save', $propdiv, $propxml);
					break;
				case "Grid":
					m.$propdiv.CswFieldTypeGrid('save', m); //('save', $propdiv, $propxml, cswnbtnodekey);
					break;
				case "Image":
					m.$propdiv.CswFieldTypeImage('save', m); //('save', $propdiv, $propxml);
					break;
				case "Link":
					m.$propdiv.CswFieldTypeLink('save', m); //('save', $propdiv, $propxml);
					break;
				case "List":
					m.$propdiv.CswFieldTypeList('save', m); //('save', $propdiv, $propxml);
					break;
				case "Location":
					m.$propdiv.CswFieldTypeLocation('save', m); //('save', $propdiv, $propxml);
					break;
				case "LocationContents":
					m.$propdiv.CswFieldTypeLocationContents('save', m); //('save', $propdiv, $propxml);
					break;
				case "Logical":
					m.$propdiv.CswFieldTypeLogical('save', m); //('save', $propdiv, $propxml);
					break;
				case "LogicalSet":
					m.$propdiv.CswFieldTypeLogicalSet('save', m); //('save', $propdiv, $propxml);
					break;
				case "Memo":
					m.$propdiv.CswFieldTypeMemo('save', m); //('save', $propdiv, $propxml);
					break;
				case "MTBF":
					m.$propdiv.CswFieldTypeMTBF('save', m); //('save', $propdiv, $propxml);
					break;
				case "NodeTypeSelect":
					m.$propdiv.CswFieldTypeNodeTypeSelect('save', m); //('save', $propdiv, $propxml);
					break;
				case "Number":
					m.$propdiv.CswFieldTypeNumber('save', m); //('save', $propdiv, $propxml);
					break;
				case "Password":
					m.$propdiv.CswFieldTypePassword('save', m); //('save', $propdiv, $propxml);
					break;
				case "PropertyReference":
					m.$propdiv.CswFieldTypePropertyReference('save', m); //('save', $propdiv, $propxml);
					break;
				case "Quantity":
					m.$propdiv.CswFieldTypeQuantity('save', m); //('save', $propdiv, $propxml);
					break;
				case "Question":
					m.$propdiv.CswFieldTypeQuestion('save', m); //('save', $propdiv, $propxml);
					break;
				case "Relationship":
					m.$propdiv.CswFieldTypeRelationship('save', m); //('save', $propdiv, $propxml);
					break;
				case "Scientific":
					m.$propdiv.CswFieldTypeScientific('save', m); //('save', $propdiv, $propxml);
					break;
				case "Sequence":
					m.$propdiv.CswFieldTypeSequence('save', m); //('save', $propdiv, $propxml);
					break;
				case "Static":
					m.$propdiv.CswFieldTypeStatic('save', m); //('save', $propdiv, $propxml);
					break;
				case "Text":
					m.$propdiv.CswFieldTypeText('save', m); //('save', $propdiv, $propxml);
					break;
				case "Time":
					m.$propdiv.CswFieldTypeTime('save', m); //('save', $propdiv, $propxml);
					break;
				case "TimeInterval":
					m.$propdiv.CswFieldTypeTimeInterval('save', m); //('save', $propdiv, $propxml);
					break;
				case "UserSelect":
					m.$propdiv.CswFieldTypeUserSelect('save', m); //('save', $propdiv, $propxml);
					break;
				case "ViewPickList":
					m.$propdiv.CswFieldTypeViewPickList('save', m); //('save', $propdiv, $propxml);
					break;
				case "ViewReference":
					m.$propdiv.CswFieldTypeViewReference('save', m); //('save', $propdiv, $propxml);
					break;
				default:
					break;
			} // switch(fieldtype)
		} // save
	};

	// Method calling logic
	if (methods[method])
	{
		return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
	} else if (typeof method === 'object' || !method)
	{
		return methods.init.apply(this, arguments);
	} else
	{
		$.error('Method ' + method + ' does not exist on ' + PluginName);
	}
}  // $.CswFieldTypeFactory
