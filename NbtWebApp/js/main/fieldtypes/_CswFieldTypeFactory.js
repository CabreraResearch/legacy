
$.CswFieldTypeFactory = function (method)
{
	var m = {
		nodeid: '',
		fieldtype: '',
		$propdiv: '',
		$savebtn: '',
		propData: '',
		onchange: function () { },
		onReload: function () { },    // if a control needs to reload the tab
		cswnbtnodekey: '',
		ID: '',
		Required: '',
		ReadOnly: '',
		EditMode: EditMode.Edit.name,
		onEditView: function () { }
	};

	var methods = {
		'make': function (options)
		{
			if (options)
			{
				$.extend(m, options);
			}
			m.ID = m.propData.id;
			m.Required = isTrue(m.propData.required);
			m.ReadOnly = isTrue(m.propData.readonly) || m.EditMode === EditMode.PrintReport.name;

			switch (m.fieldtype)
			{
				case "AuditHistoryGrid":
					m.$propdiv.CswFieldTypeAuditHistoryGrid('init', m); //'init', nodeid, propData, onchange
					break;
				case "Barcode":
					m.$propdiv.CswFieldTypeBarcode('init', m); //'init', nodeid, propData, onchange
					break;
				case "Composite":
					m.$propdiv.CswFieldTypeComposite('init', m); //'init', nodeid, propData, onchange
					break;
				case "DateTime":
					m.$propdiv.CswFieldTypeDateTime('init', m); //'init', nodeid, propData, onchange
					break;
				case "File":
					m.$propdiv.CswFieldTypeFile('init', m); //'init', nodeid, propData, onchange
					break;
				case "Grid":
					m.$propdiv.CswFieldTypeGrid('init', m); //'init', nodeid, propData, onchange
					break;
				case "Image":
					m.$propdiv.CswFieldTypeImage('init', m); //'init', nodeid, propData, onchange
					break;
				case "Link":
					m.$propdiv.CswFieldTypeLink('init', m); //'init', nodeid, propData, onchange
					break;
				case "List":
					m.$propdiv.CswFieldTypeList('init', m); //'init', nodeid, propData, onchange
					break;
				case "Location":
					m.$propdiv.CswFieldTypeLocation('init', m); //'init', nodeid, propData, onchange
					break;
				case "LocationContents":
					m.$propdiv.CswFieldTypeLocationContents('init', m); //'init', nodeid, propData, onchange
					break;
				case "Logical":
					m.$propdiv.CswFieldTypeLogical('init', m); //'init', nodeid, propData, onchange
					break;
				case "LogicalSet":
					m.$propdiv.CswFieldTypeLogicalSet('init', m); //'init', nodeid, propData, onchange
					break;
				case "Memo":
					m.$propdiv.CswFieldTypeMemo('init', m); //'init', nodeid, propData, onchange
					break;
				case "MTBF":
					m.$propdiv.CswFieldTypeMTBF('init', m); //'init', nodeid, propData, onchange
					break;
				case "MultiList":
					m.$propdiv.CswFieldTypeMultiList('init', m); //'init', nodeid, propData, onchange
					break;
				case "NodeTypeSelect":
					m.$propdiv.CswFieldTypeNodeTypeSelect('init', m); //'init', nodeid, propData, onchange
					break;
				case "Number":
					m.$propdiv.CswFieldTypeNumber('init', m); //'init', nodeid, propData, onchange
					break;
				case "Password":
					m.$propdiv.CswFieldTypePassword('init', m); //'init', nodeid, propData, onchange
					break;
				case "PropertyReference":
					m.$propdiv.CswFieldTypePropertyReference('init', m); //'init', nodeid, propData, onchange
					break;
				case "Quantity":
					m.$propdiv.CswFieldTypeQuantity('init', m); //'init', nodeid, propData, onchange
					break;
				case "Question":
					m.$propdiv.CswFieldTypeQuestion('init', m); //'init', nodeid, propData, onchange
					break;
				case "Relationship":
					m.$propdiv.CswFieldTypeRelationship('init', m); //('init', nodeid, propData, onchange);
					break;
				case "Scientific":
					m.$propdiv.CswFieldTypeScientific('init', m); //('init', nodeid, propData, onchange);
					break;
				case "Sequence":
					m.$propdiv.CswFieldTypeSequence('init', m); //('init', nodeid, propData, onchange);
					break;
				case "Static":
					m.$propdiv.CswFieldTypeStatic('init', m); //('init', nodeid, propData, onchange);
					break;
				case "Text":
					m.$propdiv.CswFieldTypeText('init', m); //('init', nodeid, propData, onchange);
					break;
//				case "Time":
//					m.$propdiv.CswFieldTypeTime('init', m); //('init', nodeid, propData, onchange);
//					break;
				case "TimeInterval":
					m.$propdiv.CswFieldTypeTimeInterval('init', m); //('init', nodeid, propData, onchange);
					break;
				case "UserSelect":
					m.$propdiv.CswFieldTypeUserSelect('init', m); //('init', nodeid, propData, onchange);
					break;
				case "ViewPickList":
					m.$propdiv.CswFieldTypeViewPickList('init', m); //('init', nodeid, propData, onchange);
					break;
				case "ViewReference":
					m.$propdiv.CswFieldTypeViewReference('init', m); //('init', nodeid, propData, onchange);
					break;
				default:
					m.$propdiv.append(m.propData.gestalt);
					CswError({
						'type': 'Error',
						'message': 'Unrecognized Field Type',
						'detail': 'CswFieldTypeFactory.make: Unrecognized Field Type: ' + m.fieldtype,
						'display': true
					});
					break;
			} // switch (fieldtype)
		}, // make

		'save': function (options)
		{
			if (options)
			{
				$.extend(m, options);
			}
			m.ID = m.propData.id;
			m.Required = isTrue(m.propData.required);
			m.ReadOnly = isTrue(m.propData.readonly);

			switch (m.fieldtype)
			{
				case "Barcode":
					m.$propdiv.CswFieldTypeBarcode('save', m); //('save', $propdiv, propData);
					break;
				case "Composite":
					m.$propdiv.CswFieldTypeComposite('save', m); //('save', $propdiv, propData);
					break;
				case "DateTime":
					m.$propdiv.CswFieldTypeDateTime('save', m); //('save', $propdiv, propData);
					break;
				case "File":
					m.$propdiv.CswFieldTypeFile('save', m); //('save', $propdiv, propData);
					break;
				case "Grid":
					m.$propdiv.CswFieldTypeGrid('save', m); //('save', $propdiv, propData, cswnbtnodekey);
					break;
				case "Image":
					m.$propdiv.CswFieldTypeImage('save', m); //('save', $propdiv, propData);
					break;
				case "Link":
					m.$propdiv.CswFieldTypeLink('save', m); //('save', $propdiv, propData);
					break;
				case "List":
					m.$propdiv.CswFieldTypeList('save', m); //('save', $propdiv, propData);
					break;
				case "Location":
					m.$propdiv.CswFieldTypeLocation('save', m); //('save', $propdiv, propData);
					break;
				case "LocationContents":
					m.$propdiv.CswFieldTypeLocationContents('save', m); //('save', $propdiv, propData);
					break;
				case "Logical":
					m.$propdiv.CswFieldTypeLogical('save', m); //('save', $propdiv, propData);
					break;
				case "LogicalSet":
					m.$propdiv.CswFieldTypeLogicalSet('save', m); //('save', $propdiv, propData);
					break;
				case "Memo":
					m.$propdiv.CswFieldTypeMemo('save', m); //('save', $propdiv, propData);
					break;
				case "MTBF":
					m.$propdiv.CswFieldTypeMTBF('save', m); //('save', $propdiv, propData);
					break;
				case "MultiList":
					m.$propdiv.CswFieldTypeMultiList('save', m); //('save', $propdiv, propData);
					break;
				case "NodeTypeSelect":
					m.$propdiv.CswFieldTypeNodeTypeSelect('save', m); //('save', $propdiv, propData);
					break;
				case "Number":
					m.$propdiv.CswFieldTypeNumber('save', m); //('save', $propdiv, propData);
					break;
				case "Password":
					m.$propdiv.CswFieldTypePassword('save', m); //('save', $propdiv, propData);
					break;
				case "PropertyReference":
					m.$propdiv.CswFieldTypePropertyReference('save', m); //('save', $propdiv, propData);
					break;
				case "Quantity":
					m.$propdiv.CswFieldTypeQuantity('save', m); //('save', $propdiv, propData);
					break;
				case "Question":
					m.$propdiv.CswFieldTypeQuestion('save', m); //('save', $propdiv, propData);
					break;
				case "Relationship":
					m.$propdiv.CswFieldTypeRelationship('save', m); //('save', $propdiv, propData);
					break;
				case "Scientific":
					m.$propdiv.CswFieldTypeScientific('save', m); //('save', $propdiv, propData);
					break;
				case "Sequence":
					m.$propdiv.CswFieldTypeSequence('save', m); //('save', $propdiv, propData);
					break;
				case "Static":
					m.$propdiv.CswFieldTypeStatic('save', m); //('save', $propdiv, propData);
					break;
				case "Text":
					m.$propdiv.CswFieldTypeText('save', m); //('save', $propdiv, propData);
					break;
//				case "Time":
//					m.$propdiv.CswFieldTypeTime('save', m); //('save', $propdiv, propData);
//					break;
				case "TimeInterval":
					m.$propdiv.CswFieldTypeTimeInterval('save', m); //('save', $propdiv, propData);
					break;
				case "UserSelect":
					m.$propdiv.CswFieldTypeUserSelect('save', m); //('save', $propdiv, propData);
					break;
				case "ViewPickList":
					m.$propdiv.CswFieldTypeViewPickList('save', m); //('save', $propdiv, propData);
					break;
				case "ViewReference":
					m.$propdiv.CswFieldTypeViewReference('save', m); //('save', $propdiv, propData);
					break;
				default:
					CswError({
						'type': 'Error',
						'message': 'Unrecognized Field Type',
						'detail': 'CswFieldTypeFactory.save: Unrecognized Field Type: ' + m.fieldtype,
						'display': true
					});
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
}   // $.CswFieldTypeFactory
