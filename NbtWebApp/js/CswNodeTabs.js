; (function ($) {
	$.fn.CswNodeTabs = function (options) {

		var o = {
			ID: '',
			TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
			SinglePropUrl: '/NbtWebApp/wsNBT.asmx/getSingleProp',
			PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
            MovePropUrl: '/NbtWebApp/wsNBT.asmx/moveProp',
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
			$outertabdiv.contents().remove();
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

							var $form = $div.children('form');
							$form.contents().remove();
							
                            var $buttondiv = $('<div />')
                                                .appendTo($form)
                                                .css({ float: 'right' });

                            var $configbutton = $buttondiv.CswImageButton({
                                                        ButtonType: CswImageButton_ButtonType.Configure,
                                                        AlternateText: 'Configure',
                                                        ID: o.ID + 'configbtn',
                                                        onClick: function (alttext) { 
                                                            Config($layouttable, $(this)); 
                                                            return CswImageButton_ButtonType.None; 
                                                        }
                                                    });
                            
                            var $layouttable = $form.CswLayoutTable('init', {
                                                          'ID': o.ID + '_props', 
                                                          cellset: { 
                                                                     rows: 1, 
                                                                     columns: 2 
                                                                   },
                                                          onSwap: function(e, onSwapData) { 
                                                                        onSwap(onSwapData);
                                                                   }
                                                        });
							
							var i = 0;
							
							_handleProps($layouttable, $xml);

                            $('<input type="button" id="SaveTab" name="SaveTab" value="Save"/>')
                                  .appendTo($form)
								  .click(function() { Save($layouttable, $xml) });


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

        function Config($layouttable, $configbutton)
        {
            $layouttable.CswLayoutTable('toggleConfig');
        }

        function onSwap(onSwapData)
        {
            _moveProp(_getPropertyCell(onSwapData.cellset).children('div').first(), onSwapData.swaprow, onSwapData.swapcolumn);
            _moveProp(_getPropertyCell(onSwapData.swapcellset).children('div').first(), onSwapData.row, onSwapData.column);
        } // onSwap()

        function _moveProp($propdiv, newrow, newcolumn)
        {
            if($propdiv.length > 0)
            {
                var propid = $propdiv.attr('propid');
                CswAjaxJSON({
				    url: o.MovePropUrl,
				    data: '{ "PropId": "'+ propid +'", "NewRow": "' + newrow + '", "NewColumn": "' + newcolumn + '" }',
				    success: function (result) {
                                
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

		function _handleProps($layouttable, $xml)
		{
			$xml.children().each(function() { 
				var $propxml = $(this);
				var fieldtype = $propxml.attr('fieldtype');
                var $cellset = $layouttable.CswLayoutTable('cellset', $propxml.attr('displayrow'), $propxml.attr('displaycol'));
				
                if( $propxml.attr('display') != 'false' &&
					fieldtype != 'Image' && 
					fieldtype != 'Grid' )
				{
					var $labelcell = _getLabelCell($cellset);
					$labelcell.addClass('propertylabel');
					$labelcell.append($propxml.attr('name'));
				}

				var $propcell = _getPropertyCell($cellset);
				$propcell.addClass('propertyvaluecell');

				_makeProp($propcell, $propxml);

			});
		} // _handleProps()

		function _makeProp($propcell, $propxml)
		{
			$propcell.contents().remove();
			if($propxml.attr('display') != 'false')
			{
				var fieldOpt = {
					'fieldtype': $propxml.attr('fieldtype'),
					'nodeid':  o.nodeid,
					'propid':  $propxml.attr('id'),
					'$propdiv': $('<div/>').appendTo($propcell),
					'$propxml': $propxml,
					'onchange': onchange, 
					'cswnbtnodekey': o.cswnbtnodekey
				};

                fieldOpt.$propdiv.attr('nodeid', fieldOpt.nodeid);
                fieldOpt.$propdiv.attr('propid', fieldOpt.propid);

				var onchange = function() {};
				if($propxml.attr('hassubprops') == "true")
				{	
					onchange = function() { 
									// do a fake 'save' to update the xml with the current value
									$.CswFieldTypeFactory('save', fieldOpt );              
									// update the propxml from the server
									CswAjaxXml({
												url: o.SinglePropUrl,
												data: 'EditMode='+ o.EditMode +'&NodePk=' + o.nodeid + '&NodeKey=' + o.cswnbtnodekey + '&PropId=' + $propxml.attr('id') + '&NodeTypeId=' + o.nodetypeid + '&NewPropXml='+ xmlToString($propxml),
												success: function ($xml) {
															 _makeProp($propcell, $xml.children().first());
														 }
												});
							   };
				}

				$.CswFieldTypeFactory('make', fieldOpt);

				// recurse on sub-props
				var $subprops = $propxml.children('subprops');
				if($subprops.length > 0 && $subprops.children('[display != "false"]').length > 0)
				{
                    var $subtable = $.CswTable({ ID: $propxml.attr('id') + '_subproptable' })
									.appendTo($propcell);
					_handleProps($subtable, $subprops);
				}
			}
		} // _makeProp()


		function Save($layouttable, $propsxml)
		{
			_updatePropXmlFromForm($layouttable, $propsxml);

			CswAjaxJSON({
				url: '/NbtWebApp/wsNBT.asmx/SaveProps',
				data: "{ EditMode: '"+ o.EditMode + "', NodePk: '" + o.nodeid + '&NodeKey=' + o.cswnbtnodekey + "', NodeTypeId: '"+ o.nodetypeid +"', NewPropsXml: '" + xmlToString($propsxml) + "' }",
				success: function(data) { 
					var dataOpt = {
						nodeid: data.nodeid,
						cswnbtnodekey: data.cswnbtnodekey 
					};
					o.onSave(dataOpt); 
				}
			});

		} // Save()

		function _updatePropXmlFromForm($layouttable, $propsxml)
		{
			$propsxml.children().each(function() { 
				var propOpt = {
     					'$propxml': $(this),
					'$propdiv': '',
					'$propCell': '',
					'fieldtype': '',
					'nodeid': o.nodeid
				};
				propOpt.fieldtype = propOpt.$propxml.attr('fieldtype');
				var $cellset = $layouttable.CswLayoutTable('cellset', propOpt.$propxml.attr('displayrow'), propOpt.$propxml.attr('displaycol'));
				propOpt.$propcell = _getPropertyCell($cellset);
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

