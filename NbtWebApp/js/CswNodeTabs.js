; (function ($) {
	$.fn.CswNodeTabs = function (options) {

		var o = {
			ID: '',
			TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
			SinglePropUrl: '/NbtWebApp/wsNBT.asmx/getSingleProp',
			PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
			nodeid: '',
			cswnbtnodekey: '',
			nodetypeid: '',
			EditMode: 'Edit', // Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue
			onSave: function() {}
		};

		if (options) {
			$.extend(o, options);
		}

		var $outertabdiv = $('<div id="' + o.ID + '_tabdiv" />')
						.appendTo($(this));

		getTabs(o);

		function clearTabs()
		{
			$outertabdiv.children().remove();
		}

		function getTabs()
		{
			CswAjaxXml({
				url: o.TabsUrl,
				data: 'EditMode='+ o.EditMode +'&NodePk=' + o.nodeid + '&NodeTypeId=' + o.nodetypeid,
				success: function ($xml) {
							clearTabs();
							var $tabdiv = $("<div><ul></ul></div>");
							$outertabdiv.append($tabdiv);
							//var firsttabid = null;
							$xml.children().each(function() { 
								$tab = $(this);
								$tabdiv.children('ul').append('<li><a href="#'+ $tab.attr('id') +'">'+ $tab.attr('name') +'</a></li>');
								$tabdiv.append('<div id="'+ $tab.attr('id') +'"><form id="'+ $tab.attr('id') +'_form" /></div>');
								//if(null == firsttabid) 
								//    firsttabid = $tab.attr('id');
							});
							var optSelect = {
								tabid: ''
							};
							$tabdiv.tabs({
								select: function(event, ui) {
											optSelect.tabid = $($tabdiv.children('div')[ui.index]).attr('id');
											getProps(optSelect);
										}
							});
							optSelect.tabid = $($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]).attr('id');
							getProps(optSelect);
						} // success{}
			});
		} // getTabs()
			
		function getProps(optSelect) //tabid
		{
			var p = {
				tabid: ''
			};
			if(optSelect)
			{
				$.extend(p, optSelect);
			}

			CswAjaxXml({
				url: o.PropsUrl,
				data: 'EditMode='+ o.EditMode +'&NodePk=' + o.nodeid + '&TabId=' + p.tabid + '&NodeTypeId=' + o.nodetypeid,
				success: function ($xml) {
							$div = $("#" + p.tabid);
							$form = $div.children('form');
							$form.children().remove();
							
							var $table = makeTable(o.ID + '_proptable')
										 .appendTo($form);
							
							var i = 0;
							var handleOpt = {
								'$table': $table, 
								'$xml': $xml
							};
							_handleProps(handleOpt); //$table, $xml

							$table.append('<tr><td><input type="button" id="SaveTab" name="SaveTab" value="Save"/></td></tr>')
								  .find('#SaveTab')
								  .click(function() { Save(handleOpt) }); //$table, $xml

							// Validation
							$form.validate({ 
											 highlight: function(element, errorClass) {
												 var $elm = $(element);
												 $elm.animate({ backgroundColor: '#ff6666'});
											 },
											 unhighlight: function(element, errorClass) {
												 var $elm = $(element);
												 $elm.css('background-color', '#66ff66');
												 setTimeout(function() { $elm.animate({ backgroundColor: 'transparent'}); }, 500);
											 }
										   });
						} // success{}
			}); 
		} // getProps()

		function _handleProps(handleOpt) //$table, $xml
		{
			handleOpt.$xml.children().each(function() { 
				var $prop = $(this);
				var fieldtype = $prop.attr('fieldtype');

				if( $prop.attr('display') != 'false' &&
					fieldtype != 'Image' && 
					fieldtype != 'Grid' )
				{
					var $labelcell = getTableCell(handleOpt.$table, $prop.attr('displayrow'), ($prop.attr('displaycol') * 2 ) - 1);
					$labelcell.addClass('propertylabel');
					$labelcell.append($prop.attr('name'));
				}

				var $propcell = getTableCell(handleOpt.$table, $prop.attr('displayrow'), ($prop.attr('displaycol') * 2));
				$propcell.addClass('propertyvaluecell');

				var makeOpt = {
					'$propcell': $propcell, 
					'$prop': $prop	
				};
				_makeProp(makeOpt); //$propcell, $prop

			});
		} // _handleProps()

		function _makeProp(makeOpt) //$propcell, $prop
		{
			makeOpt.$propcell.children().remove();
			if(makeOpt.$prop.attr('display') != 'false')
			{
				var fieldOpt = {
					'fieldtype': makeOpt.$prop.attr('fieldtype'),
					'nodeid':  o.nodeid,
					'$propdiv': $('<div/>').appendTo(makeOpt.$propcell),
					'$propxml': makeOpt.$prop,
					'onchange': onchange, 
					'cswnbtnodekey': o.cswnbtnodekey
				};

				var onchange = function() {};
				if(makeOpt.$prop.attr('hassubprops') == "true")
				{	
					onchange = function() { 
									// do a fake 'save' to update the xml with the current value
									$.CswFieldTypeFactory('save', fieldOpt );              
									// update the propxml from the server
									CswAjaxXml({
												url: o.SinglePropUrl,
												data: 'EditMode='+ o.EditMode +'&NodePk=' + o.nodeid + '&NodeKey=' + o.cswnbtnodekey + '&PropId=' + makeOpt.$prop.attr('id') + '&NodeTypeId=' + o.nodetypeid + '&NewPropXml='+ xmlToString(makeOpt.$prop),
												success: function ($xml) {
															 _makeProp({'$propcell': makeOpt.$propcell, '$prop': $xml.children().first()});
														 }
												});
							   };
				}

				$.CswFieldTypeFactory('make', fieldOpt);

				// recurse on sub-props
				var $subprops = makeOpt.$prop.children('subprops');
				if($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0)
				{
					var $subtable = makeTable(makeOpt.$prop.attr('id') + '_subproptable')
									.appendTo(makeOpt.$propcell);
					var handleOpt = {
						'$table': $subtable, 
						'$xml': $subprops
					};
					_handleProps(handleOpt);
				}
			}
		} // _makeProp()


		function Save(saveOpt) //$table, $propsxml
		{
			_updatePropXmlFromForm(saveOpt); //$table, $propsxml

			CswAjaxJSON({
				url: '/NbtWebApp/wsNBT.asmx/SaveProps',
				data: "{ EditMode: '"+ o.EditMode + "', NodePk: '" + o.nodeid + '&NodeKey=' + o.cswnbtnodekey + "', NodeTypeId: '"+ o.nodetypeid +"', NewPropsXml: '" + xmlToString(saveOpt.$propsxml) + "' }",
				success: function(data) { 
					var dataOpt = {
						nodeid: data.nodeid,
						cswnbtnodekey: data.cswnbtnodekey 
					};
					o.onSave(dataOpt); 
				}
			});

		} // Save()

		function _updatePropXmlFromForm(updateOpt) //$table, $propsxml
		{
			updateOpt.$propsxml.children().each(function() { 
				var propOpt = {
					'$propxml': $(this),
					'$propdiv': '',
					'$propCell': '',
					'fieldtype': '',
					'nodeid': o.nodeid
				};
				propOpt.fieldtype = propOpt.$propxml.attr('fieldtype');
				propOpt.$propCell = getTableCell(updateOpt.$table, propOpt.$propxml.attr('displayrow'), (propOpt.$propxml.attr('displaycol') * 2));
				propOpt.$propdiv = propOpt.$propcell.children('div').first();

				$.CswFieldTypeFactory('save', propOpt);              

				// recurse on subprops
				if(propOpt.$propxml.attr('hassubprops') == "true")
				{
					var $subprops = propOpt.$propxml.children('subprops');
					if($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0)
					{
						var $subtable = propOpt.$propcell.children('#' + propOpt.$propxml.attr('id') + '_subproptable').first();
						recOpt = {
							'$table': $subtable,
							'$propsxml': $subprops
						};
						_updatePropXmlFromForm(recOpt);
					}
				}
			}); // each()
		} // _updatePropXmlFromForm()

		// For proper chaining support
		return this;

	}; // function(options) {
}) (jQuery);

