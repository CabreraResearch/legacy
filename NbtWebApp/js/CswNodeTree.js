﻿; (function ($) {
	
	var PluginName = 'CswNodeTree';

	var methods = {
		'init': function(options)     // options are defined in _getTreeContent()
			{    
				var o = {
					ID: '',
					ViewTreeUrl: '/NbtWebApp/wsNBT.asmx/getTreeOfView',
					NodeTreeUrl: '/NbtWebApp/wsNBT.asmx/getTreeOfNode',
					viewid: '',       // loads an arbitrary view
					viewmode: '',
					nodeid: '',       // if viewid is not supplied, loads a view of this node
					cswnbtnodekey: '',
					onSelectNode: function(optSelect) {
											var o =  {
												nodeid: '', 
												nodename: '', 
												iconurl: '', 
												cswnbtnodekey: '',
												viewid: ''
											};
									},
					SelectFirstChild: true
				};
				if(options) $.extend(o, options);

				var IDPrefix = o.ID + '_';
				var $treediv = $('<div id="'+ IDPrefix + '" class="treediv" />')
								.appendTo($(this));

				var url = o.ViewTreeUrl;
				var data = 'ViewNum=' + o.viewid + '&IDPrefix=' + IDPrefix + '&ParentNodeKey=&IncludeNodeKey=';
				if(o.viewid == '' || o.viewid == undefined)
				{
					url = o.NodeTreeUrl;
					data = 'IDPrefix=' + IDPrefix + '&NodePk=' + o.nodeid;
				}

				CswAjaxXml({
					url: url,
					data: data,
					success: function ($xml) {
						var selectid = '';
						var treePlugins = ["themes", "xml_data", "ui", "types", "crrm"];
						var treeThemes;
						if(o.nodeid != undefined && o.nodeid != '') 
						{
							selectid = IDPrefix + o.nodeid;
						}

						var $selecteditem = $xml.find('item[id="'+ selectid + '"]');
						if(selectid == '' || $selecteditem.length == 0)
						{
							if(o.SelectFirstChild)
							{	
								if(o.viewmode == 'list' )
								{
									selectid = $xml.find('item').first().attr('id');
									treeThemes = {"dots": false};
								}
								else
								{
									selectid = $xml.find('item').first().find('item').first().attr('id');
									treeThemes = {"dots": true};
								}
							}
							else
							{
								selectid = IDPrefix + 'root';
							}
						}

						// make sure selected item is visible
						$selecteditem = $xml.find('item[id="'+ selectid + '"]');
						var $itemparents = $selecteditem.parents('item').andSelf();
						var initiallyOpen = new Array();
						var i = 0;
						$itemparents.each(function() { initiallyOpen[i] = $(this).attr('id'); i++; });

						var strTypes = $xml.find('types').text();
						var jsonTypes = $.parseJSON(strTypes);
						var $treexml = $xml.find('tree').children('root')
						if($treexml.length > 0)
						{
							var treexmlstring = xmlToString($treexml);
							$treediv.jstree({
								"xml_data": 
									{
										"data": treexmlstring,
										"xsl": "nest",
										"ajax": 
											{
												"type": 'POST',
												"url": url,
												"dataType": "xml",
												"data": function($nodeOpening) 
													{
														var nodekey = $nodeOpening.attr('cswnbtnodekey');
														return 'ViewNum=' + o.viewid + '&IDPrefix=' + IDPrefix + '&ParentNodeKey=' + nodekey + '&IncludeNodeKey=';
													}
											}
									},
								"ui": {
									"select_limit": 1,
									"initially_select": selectid,
									"select_prev_on_delete": true    // needed for "More..." nodes
								},
								"themes": treeThemes,
								"core": {
									"initially_open": initiallyOpen
								},
								"types": {
									"types": jsonTypes,
									"max_children": -2,
									"max_depth": -2
								},
								"plugins": treePlugins
							}).bind('select_node.jstree', 
											function (e, data) {
												var Selected = jsTreeGetSelected($treediv, IDPrefix);
												var optSelect =  {
													nodeid: Selected.id, 
													nodename: Selected.text, 
													iconurl: Selected.iconurl, 
													cswnbtnodekey: Selected.$item.attr('cswnbtnodekey'),
													viewid: o.viewid
												};
												if(optSelect.nodename == "More...")
												{
													var ParentNodeKey = '';
													var Parent = data.inst._get_parent(data.rslt.obj);
													if(Parent != -1)
														ParentNodeKey = Parent.attr('cswnbtnodekey');
													if(ParentNodeKey == undefined)
														ParentNodeKey = '';

													// get next page of nodes
													CswAjaxXml({
														url: url,
														data: 'ViewNum=' + o.viewid + '&IDPrefix=' + IDPrefix + '&ParentNodeKey='+ ParentNodeKey +'&IncludeNodeKey=' + optSelect.cswnbtnodekey,
														success: function ($xml) 
															{
																// remove 'More' node
																$treediv.jstree('remove', '#' + IDPrefix + optSelect.nodeid );

																// get prior sibling (now selected)
																var NewSelected = jsTreeGetSelected($treediv, IDPrefix);
																var AfterNodeId = IDPrefix + NewSelected.id;
																var $itemxml = $xml.children().first();
																	
																// we have to do these one at a time in successive OnSuccess callbacks, 
																// or else they won't end up in the right place on the tree
																_continue();

																function _continue()
																{
																	if($itemxml.length > 0)
																	{
																		$treediv.jstree('create', '#' + AfterNodeId, 'after',
																						{ 
																							'attr': {
																										'id': $itemxml.attr('id'), 
																										'rel': $itemxml.attr('rel'),
																										'cswnbtnodekey': $itemxml.attr('cswnbtnodekey')
																									},
																							'data': $itemxml.children('content').children('name').text(), 
																							'state': $itemxml.attr('state') 
																						}, 
																						function() 
																						{
																							AfterNodeId = $itemxml.attr('id');
																							$itemxml = $itemxml.next();
																							_continue();
																						}, 
																						true, true);

																	} // if($itemxml.length > 0)
																} // _continue()

															} // success
														}); // ajax
												}
												else 
												{
													o.onSelectNode(optSelect);
												}
											});
							// DO NOT define an onSuccess() function here that interacts with the tree.
							// The tree has initalization events that appear to happen asynchronously,
							// and thus having an onSuccess() function that changes the selected node will
							// cause a race condition.
						} else {
							$treediv.append('No Results');
						}

					} // success{}
				}); // ajax

				return $treediv;
			},

		'selectNode': function(optSelect) 
			{
				var o = {
					newnodeid: '', 
					newcswnbtnodekey: ''
				}
				if (optSelect) {
					$.extend(o, optSelect);
				}
				var $treediv = $(this).children('.treediv')
				var IDPrefix = $treediv.attr('id');
				$treediv.jstree('select_node', '#' + IDPrefix + o.newnodeid);
			}
	};

	// Method calling logic
	$.fn.CswNodeTree = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

})(jQuery);

