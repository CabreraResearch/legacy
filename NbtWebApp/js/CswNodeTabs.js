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

		getTabs(o.nodeid);


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
							$tabdiv.tabs({
								select: function(event, ui) {
											getProps($($tabdiv.children('div')[ui.index]).attr('id'));
										}
							});
							getProps($($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]).attr('id'));
						} // success{}
			});
		} // getTabs()
			
		function getProps(tabid)
		{
			CswAjaxXml({
				url: o.PropsUrl,
				data: 'EditMode='+ o.EditMode +'&NodePk=' + o.nodeid + '&TabId=' + tabid + '&NodeTypeId=' + o.nodetypeid,
				success: function ($xml) {
							$div = $("#" + tabid);
							$form = $div.children('form');
							$form.children().remove();
							
                            $div.CswLayoutTable('init', {
                                                          'ID': o.ID + '_props', 
                                                          cellset: { 
                                                                     rows: 1, 
                                                                     columns: 2 
                                                                   }
                                                        });
							
							var i = 0;

							_handleProps($div, $xml);

                            $div.CswLayoutTable('finish');

							$div.append('<input type="button" id="SaveTab" name="SaveTab" value="Save"/>')
								  .click(function() { Save($div, $xml) });

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

		function _handleProps($div, $xml)
		{
			$xml.children().each(function() { 
				var $prop = $(this);
				var fieldtype = $prop.attr('fieldtype');
                var $cellset = $div.CswLayoutTable('cellset', $prop.attr('displayrow'), $prop.attr('displaycol'));
				
                if( $prop.attr('display') != 'false' &&
					fieldtype != 'Image' && 
					fieldtype != 'Grid' )
				{
					var $labelcell = $cellset[1][1];
					$labelcell.addClass('propertylabel');
					$labelcell.append($prop.attr('name'));
				}

				var $propcell = $cellset[1][2];
				$propcell.addClass('propertyvaluecell');

				_makeProp($propcell, $prop);

			});
		} // _handleProps()

		function _makeProp($propcell, $prop)
		{
			$propcell.children().remove();
			if($prop.attr('display') != 'false')
			{
				var $propdiv = $('<div/>').appendTo($propcell); 
				var fieldtype = $prop.attr('fieldtype');

				var onchange = function() {};
				if($prop.attr('hassubprops') == "true")
					onchange = function() { 
									// do a fake 'save' to update the xml with the current value
									$.CswFieldTypeFactory('save', fieldtype, o.nodeid, $propdiv, $prop);              
									// update the propxml from the server
									CswAjaxXml({
												url: o.SinglePropUrl,
												data: 'EditMode='+ o.EditMode +'&NodePk=' + o.nodeid + '&NodeKey=' + o.cswnbtnodekey + '&PropId=' + $prop.attr('id') + '&NodeTypeId=' + o.nodetypeid + '&NewPropXml='+ xmlToString($prop),
												success: function ($xml) {
															 _makeProp($propcell, $xml.children().first());
														 }
												});
							   };

				$.CswFieldTypeFactory('make', o.nodeid, fieldtype, $propdiv, $prop, onchange, o.cswnbtnodekey); 

				// recurse on sub-props
				var $subprops = $prop.children('subprops');
				if($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0)
				{
					var $subtable = makeTable($prop.attr('id') + '_subproptable')
									.appendTo($propcell);

					_handleProps($subtable, $subprops);
				}
			}
		} // _makeProp()


		function Save($div, $propsxml)
		{
			_updatePropXmlFromForm($div, $propsxml);

			CswAjaxJSON({
				url: '/NbtWebApp/wsNBT.asmx/SaveProps',
				data: "{ EditMode: '"+ o.EditMode + "', NodePk: '" + o.nodeid + '&NodeKey=' + o.cswnbtnodekey + "', NodeTypeId: '"+ o.nodetypeid +"', NewPropsXml: '" + xmlToString($propsxml) + "' }",
				success: function(data) { 
					o.onSave(data.nodeid, data.cswnbtnodekey); 
				}
			});

		} // Save()

		function _updatePropXmlFromForm($div, $propsxml)
		{
			$propsxml.children().each(function() { 
				var $prop = $(this);
                var $cellset = $div.CswLayoutTable('cellset', $prop.attr('displayrow'), $prop.attr('displaycol'));
				var $propcell = $cellset[1][2];
				var fieldtype = $prop.attr('fieldtype');
				var $propdiv = $propcell.children('div').first();

				$.CswFieldTypeFactory('save', fieldtype, o.nodeid, $propdiv, $prop);              

				// recurse on subprops
				if($prop.attr('hassubprops') == "true")
				{
					var $subprops = $prop.children('subprops');
					if($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0)
					{
						var $subtable = $propcell.children('#' + $prop.attr('id') + '_subproptable').first();
						_updatePropXmlFromForm($subtable, $subprops);
					}
				}
			}); // each()
		} // _updatePropXmlFromForm()

		// For proper chaining support
		return this;

	}; // function(options) {
}) (jQuery);

