/// <reference path="../jquery/jquery-1.5.js" />

; (function ($) {
	var PluginName = "CswSearch";

	var methods = {
	
		'getSearchForm': function(options) 
			{
				var o = { 
					'RenderSearchUrl': '/NbtWebApp/wsNBT.asmx/getClientSearchXml',
					'ExecViewSearchUrl': '/NbtWebApp/wsNBT.asmx/doViewSearch',
                    'ExecNodeSearchUrl': '/NbtWebApp/wsNBT.asmx/doNodeTypeSearch',
                    'SearchableViewsUrl': '/NbtWebApp/wsNBT.asmx/getSearchableViews',
                    viewid: '',
                    nodetypeid: '',
                    relatedidtype: '',
                    'onSearch': function() { }
				};
				if(options) {
					$.extend(o, options);
				}
                var $parent = $(this);
                CswAjaxXml({ 
			        'url': o.RenderSearchUrl,
			        'data': "ViewIdNum=" + o.viewid + "&SelectedNodeTypeIdNum=" + o.nodetypeid,
                    'success': function($xml) { 
                                    log($xml);
                                    var $table = $parent.CswTable('init', { ID: 'search_tbl' });
                                    var searchtype = $xml.attr('searchtype');
                                    switch(searchtype)
                                    {
                                        case 'nodetypesearch':
                                        {
                                            //Row 1, Column 1: nodetypeselect picklist
                                            var $typeselect = $table.CswTable('cell', 1, 1);
                                            var $option = $xml.children('select');
                                            var optionList = xmlToString($option);
                                            $typeselect.html(optionList);
                                            $('#node_type_select').hyjack_select({offset: ''});
                                            
                                            //Row 1, Column 2: properties picklist
                                            var $propselect = $table.CswTable('cell', 1, 2);
                                            var props = $xml.children('searchcriteria').children('properties');
                                            var propList = xmlToString($propselect);
                                            $propselect.html(propList);
                                            $propselect.hyjack_select({offset: ''});

                                            //Row 1, Column 3: default subfield
                                            var subfield = $xml.children('searchcriteria').children('properties').attr('defaultprop');
                                            var $staticsubfield = $table.CswTable('cell', 1, 3);
                                            $staticsubfield.html('<span>'+subfield+'</span>');
                                        }
                                        case 'viewsearch':
                                        {


                                        }

                                    }
							        
						        } // each
					         // success
			        }); // CswAjaxXml

				/*var $parent = $(this);
				var $table = $parent.CswTable('init', { ID: 'addwelcomeitem_tbl' });

				$table.CswTable('cell', 1, 1).append('Type:');
				var $typeselect = $('<select id="welcome_type" name="welcome_type"></select>')
									.appendTo($table.CswTable('cell', 1, 2));
				$typeselect.append('<option value="Add">Add</option>');
				$typeselect.append('<option value="Link">Link</option>');
				$typeselect.append('<option value="Search">Search</option>');
				$typeselect.append('<option value="Text">Text</option>');
						
				$table.CswTable('cell', 2, 1).append('View:');
				var $viewselect = $table.CswTable('cell', 2, 2).CswViewSelect({
																				'ID': 'welcome_viewsel',
																				//'viewid': '',
																				//'onSelect': function(optSelect) { },
																			});

				$table.CswTable('cell', 3, 1).append('Add New:');
				var $ntselect = $table.CswTable('cell', 3, 2).CswNodeTypeSelect({
																'ID': 'welcome_ntsel'
																});

				$table.CswTable('cell', 4, 1).append('Text:');
				var $welcometext = $('<input type="text" id="welcome_text" value="" />')
									.appendTo($table.CswTable('cell', 4, 2));

				$table.CswTable('cell', 5, 1).append('Use Button:');
				var $buttonsel = $('<select id="welcome_button" />')
									.appendTo($table.CswTable('cell', 5, 2));
				$buttonsel.append('<option value="blank.gif"></option>');

				var $buttonimg = $('<img id="welcome_btnimg" />')
									.appendTo( $table.CswTable('cell', 6, 2) );

				var $addbutton = $('<input type="button" id="welcome_add" name="welcome_add" value="Add" />')
									.appendTo( $table.CswTable('cell', 7, 2) )
									.click(function() { 
										_addItem({ 
													'AddWelcomeItemUrl': o.AddWelcomeItemUrl,
													'type': $typeselect.val(),
													'viewid': $viewselect.CswViewSelect('value'),
													'nodetypeid': $ntselect.CswNodeTypeSelect('value'),
													'text': $welcometext.val(),
													'iconfilename': $buttonsel.val(),
													'onSuccess': o.onAdd
												});
									});

				$buttonsel.change(function(event) { 
					$buttonimg.attr('src', 'Images/biggerbuttons/' + $buttonsel.val()); 
				});

                */
			} // getAddItemForm
		
	};

	
	// Method calling logic
	$.fn.CswSearch = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

})(jQuery);


