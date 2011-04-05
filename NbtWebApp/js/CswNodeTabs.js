; (function ($)
{
	$.fn.CswNodeTabs = function (options)
	{

		var o = {
			ID: '',
			TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
			SinglePropUrl: '/NbtWebApp/wsNBT.asmx/getSingleProp',
			PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
			MovePropUrl: '/NbtWebApp/wsNBT.asmx/moveProp',
			SavePropUrl: '/NbtWebApp/wsNBT.asmx/SaveProps',
			nodeid: '',
			tabid: '',
			cswnbtnodekey: '',
			nodetypeid: '',
			EditMode: 'Edit', // Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue
			onSave: function (nodeid, cswnbtnodekey) { },
			onBeforeTabSelect: function (tabid) { return true; },
			onTabSelect: function (tabid) { },
			onPropertyChange: function(propid, propname) { }
		};

		if (options)
		{
			$.extend(o, options);
		}

		var $outertabdiv = $('<div id="' + o.ID + '_tabdiv" />')
						.appendTo($(this));

		getTabs(o);

		function clearTabs()
		{
			$outertabdiv.contents().remove();
		}

		function getTabs()
		{
			CswAjaxXml({
				url: o.TabsUrl,
				data: 'EditMode=' + o.EditMode + '&SafeNodeKey=' + o.cswnbtnodekey + '&NodeTypeId=' + o.nodetypeid,
				success: function ($xml)
				{
					clearTabs();
					var $tabdiv = $("<div><ul></ul></div>");
					$outertabdiv.append($tabdiv);
					var selectedtabno = 0;
					var tabno = 0;
					$xml.children().each(function ()
					{
						$tab = $(this);
						$tabdiv.children('ul').append('<li><a href="#' + $tab.attr('id') + '">' + $tab.attr('name') + '</a></li>');
						$tabdiv.append('<div id="' + $tab.attr('id') + '"><form id="' + $tab.attr('id') + '_form" /></div>');
						if($tab.attr('id') == o.tabid)
						{
							selectedtabno = tabno;
						}
						tabno++;
					});
					$tabdiv.tabs({
						'selected': selectedtabno,
						'select': function (event, ui)
						{
							if(o.onBeforeTabSelect(tabid))
							{
								var $tabcontentdiv = $($tabdiv.children('div')[ui.index]);
								var tabid = $tabcontentdiv.attr('id');
								getProps($tabcontentdiv, tabid);

								o.onTabSelect(tabid);
							} else {
								return false;
							}
						}
					});
					var $tabcontentdiv = $($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]);
					var selectedtabid = $tabcontentdiv.attr('id');
					getProps($tabcontentdiv, selectedtabid);
					o.onTabSelect(selectedtabid);
				} // success{}
			});
		} // getTabs()

		function getProps($tabcontentdiv, tabid)
		{
			CswAjaxXml({
				url: o.PropsUrl,
				data: 'EditMode=' + o.EditMode + '&SafeNodeKey=' + o.cswnbtnodekey + '&TabId=' + tabid + '&NodeTypeId=' + o.nodetypeid,
				success: function ($xml)
				{
					var $form = $tabcontentdiv.children('form');
					$form.contents().remove();

					var $layouttable = $form.CswLayoutTable('init', {
						'ID': o.ID + '_props',
						'OddCellRightAlign': true,
						'cellset': {
							rows: 1,
							columns: 2
						},
						'onSwap': function (e, onSwapData)
						{
							onSwap(onSwapData);
						},
						'showConfigButton': true,
						'onConfigOn': function($buttontable) { 
							$xml.children().each(function ()
							{
								var $propxml = $(this);
								var $cellset = $layouttable.CswLayoutTable('cellset', $propxml.attr('displayrow'), $propxml.attr('displaycol'));
								var $propcell = _getPropertyCell($cellset);
								var $subtable = $propcell.children('#' + $propxml.attr('id') + '_subproptable');
								
								var fieldOpt = {
									'fieldtype': $propxml.attr('fieldtype'),
									'nodeid': o.nodeid,
									'propid': $propxml.attr('id'),
									'$propdiv': $propcell.children('div'),
									'$propxml': $propxml,
									'onchange': function() { },
									'onReload': function() { getProps($tabcontentdiv, tabid); },
									'cswnbtnodekey': o.cswnbtnodekey
								};
								
								if($subtable.length > 0)
								{
									//$subtable.CswLayoutTable('ConfigOn');
									_updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, $propxml.attr('id'), o.nodetypeid, $propxml, $propcell, $tabcontentdiv, tabid, true);
								}
							});
						},
						'onConfigOff': function($buttontable) { 
							$xml.children().each(function ()
							{
								var $propxml = $(this);
								var $cellset = $layouttable.CswLayoutTable('cellset', $propxml.attr('displayrow'), $propxml.attr('displaycol'));
								var $propcell = _getPropertyCell($cellset);
								var $subtable = $propcell.children('#' + $propxml.attr('id') + '_subproptable');

								var fieldOpt = {
									'fieldtype': $propxml.attr('fieldtype'),
									'nodeid': o.nodeid,
									'propid': $propxml.attr('id'),
									'$propdiv': $propcell.children('div'),
									'$propxml': $propxml,
									'onchange': function() { },
									'onReload': function() { getProps($tabcontentdiv, tabid); },
									'cswnbtnodekey': o.cswnbtnodekey
								};
								
								if($subtable.length > 0)
								{
									//$subtable.CswLayoutTable('ConfigOff');
									_updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, $propxml.attr('id'), o.nodetypeid, $propxml, $propcell, $tabcontentdiv, tabid, false);
								}
							});
						}

					});

					var i = 0;

					_handleProps($layouttable, $xml, $tabcontentdiv, tabid);

					$('<input type="button" id="SaveTab" name="SaveTab" value="Save Changes"/>')
                                  .button()
                                  .appendTo($form)
								  .click(function () { Save($form, $layouttable, $xml) });


					// Validation
					$form.validate({
						highlight: function (element, errorClass)
						{
							var $elm = $(element);
							$elm.attr('csw_invalid', '1');
							$elm.animate({ backgroundColor: '#ff6666' });
						},
						unhighlight: function (element, errorClass)
						{
							var $elm = $(element);
							if($elm.attr('csw_invalid') == '1')  // only unhighlight where we highlighted
							{
								$elm.css('background-color', '#66ff66');
								$elm.attr('csw_invalid', '0')
								setTimeout(function () { $elm.animate({ backgroundColor: 'transparent' }); }, 500);
							}
						}
					});
				} // success{}
			});
		} // getProps()

		function onSwap(onSwapData)
		{
			_moveProp(_getPropertyCell(onSwapData.cellset).children('div').first(), onSwapData.swaprow, onSwapData.swapcolumn);
			_moveProp(_getPropertyCell(onSwapData.swapcellset).children('div').first(), onSwapData.row, onSwapData.column);
		} // onSwap()

		function _moveProp($propdiv, newrow, newcolumn)
		{
			if ($propdiv.length > 0)
			{
				var propid = $propdiv.attr('propid');
				CswAjaxJSON({
					url: o.MovePropUrl,
					data: '{ "PropId": "' + propid + '", "NewRow": "' + newrow + '", "NewColumn": "' + newcolumn + '", "EditMode": "'+ o.EditMode +'" }',
					success: function (result)
					{

					}
				});
			}
		} // _moveProp()

		function _getLabelCell($cellset)
		{
			return $cellset[1][1];
		}
		function _getPropertyCell($cellset)
		{
			return $cellset[1][2];
		}

		function _handleProps($layouttable, $xml, $tabcontentdiv, tabid, ConfigMode)
		{
			$xml.children().each(function ()
			{
				var $propxml = $(this);
				var fieldtype = $propxml.attr('fieldtype');
				var $cellset = $layouttable.CswLayoutTable('cellset', $propxml.attr('displayrow'), $propxml.attr('displaycol'));

				if (($propxml.attr('display') != 'false' || ConfigMode ) &&
					fieldtype != 'Image' &&
					fieldtype != 'Grid')
				{
					var $labelcell = _getLabelCell($cellset);
					$labelcell.addClass('propertylabel');
					$labelcell.append($propxml.attr('name'));
				}

				var $propcell = _getPropertyCell($cellset);
				$propcell.addClass('propertyvaluecell');

				_makeProp($propcell, $propxml, $tabcontentdiv, tabid, ConfigMode);

			});
		} // _handleProps()

		function _makeProp($propcell, $propxml, $tabcontentdiv, tabid, ConfigMode)
		{
			$propcell.contents().remove();
			if ($propxml.attr('display') != 'false' || ConfigMode )
			{
				var fieldOpt = {
					'fieldtype': $propxml.attr('fieldtype'),
					'nodeid': o.nodeid,
					'propid': $propxml.attr('id'),
					'$propdiv': $('<div/>').appendTo($propcell),
					'$propxml': $propxml,
					'onchange': function() { },
					'onReload': function() { getProps($tabcontentdiv, tabid); },
					'cswnbtnodekey': o.cswnbtnodekey
				};

				fieldOpt.$propdiv.attr('nodeid', fieldOpt.nodeid);
				fieldOpt.$propdiv.attr('propid', fieldOpt.propid);
				fieldOpt.$propdiv.attr('cswnbtnodekey', fieldOpt.cswnbtnodekey);

				fieldOpt.onchange = function () { o.onPropertyChange(fieldOpt.propid, $propxml.attr('name')); };
				if ($propxml.attr('hassubprops') == "true")
				{
					fieldOpt.onchange = function ()
					{
						_updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, $propxml.attr('id'), o.nodetypeid, $propxml, $propcell, $tabcontentdiv, tabid, false);
						o.onPropertyChange(fieldOpt.propid, $propxml.attr('name'));
					};
				} // if ($propxml.attr('hassubprops') == "true")

				$.CswFieldTypeFactory('make', fieldOpt);

				// recurse on sub-props
				var $subprops = $propxml.children('subprops');
				if (($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0) || ConfigMode)
				{
					//var $subtable = $propcell.CswTable('init', { ID: $propxml.attr('id') + '_subproptable' });

					var $subtable = $propcell.CswLayoutTable('init', {
						'ID': fieldOpt.propid + '_subproptable',
						'OddCellRightAlign': true,
						'cellset': {
							rows: 1,
							columns: 2
						},
						'onSwap': function (e, onSwapData)
						{
							onSwap(onSwapData);
						},
						'showConfigButton': false
					});
					_handleProps($subtable, $subprops, $tabcontentdiv, tabid, ConfigMode);
					if(ConfigMode) {
						$subtable.CswLayoutTable('ConfigOn');
					} else {
						$subtable.CswLayoutTable('ConfigOff');
					}
				}
			} // if ($propxml.attr('display') != 'false' || ConfigMode )
		} // _makeProp()

		function _updateSubProps(fieldOpt, SinglePropUrl, EditMode, cswnbtnodekey, PropId, nodetypeid, $propxml, $propcell, $tabcontentdiv, tabid, ConfigMode)
		{
			// do a fake 'save' to update the xml with the current value
			$.CswFieldTypeFactory('save', fieldOpt);

			// update the propxml from the server
			CswAjaxXml({
				url: SinglePropUrl,
				data: 'EditMode=' + EditMode + '&SafeNodeKey=' + cswnbtnodekey + '&PropId=' + PropId + '&NodeTypeId=' + nodetypeid + '&NewPropXml=' + xmlToString($propxml),
				success: function ($xml)
				{
					_makeProp($propcell, $xml.children().first(), $tabcontentdiv, tabid, ConfigMode);
				}
			});
		} // _updateSubProps()

		function Save($form, $layouttable, $propsxml)
		{
			if($form.valid())
			{
				_updatePropXmlFromForm($layouttable, $propsxml);

				CswAjaxJSON({
					url: o.SavePropUrl,
					data: "{ EditMode: '" + o.EditMode + "', SafeNodeKey: '" + o.cswnbtnodekey + "', NodeTypeId: '" + o.nodetypeid + "', ViewId: '"+ $.CswCookie('get', CswCookieName.CurrentView.ViewId) +"', NewPropsXml: '" + xmlToString($propsxml) + "' }",
					success: function (data)
					{
						o.onSave(data.nodeid, data.cswnbtnodekey);
					}
				});
			} // if($form.valid())
		} // Save()

		function _updatePropXmlFromForm($layouttable, $propsxml)
		{
			$propsxml.children().each(function ()
			{
				var propOpt = {
					'$propxml': $(this),
					'$propdiv': '',
					'$propCell': '',
					'fieldtype': '',
					'nodeid': o.nodeid,
					'cswnbtnodekey': o.cswnbtnodekey
				};
				propOpt.fieldtype = propOpt.$propxml.attr('fieldtype');
				var $cellset = $layouttable.CswLayoutTable('cellset', propOpt.$propxml.attr('displayrow'), propOpt.$propxml.attr('displaycol'));
				propOpt.$propcell = _getPropertyCell($cellset);
				propOpt.$propdiv = propOpt.$propcell.children('div').first();

				$.CswFieldTypeFactory('save', propOpt);

				// recurse on subprops
				if (propOpt.$propxml.attr('hassubprops') == "true")
				{
					var $subprops = propOpt.$propxml.children('subprops');
					if ($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0)
					{
						var $subtable = propOpt.$propcell.children('#' + propOpt.$propxml.attr('id') + '_subproptable').first();
						_updatePropXmlFromForm($subtable, $subprops);
					}
				}
			}); // each()
		} // _updatePropXmlFromForm()

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

