﻿/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.2-vsdoc.js" />
/// <reference path="/js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="/js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
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
                    showempty: false, // if true, shows an empty tree (primarily for search)
                    forsearch: false, // if true, used to override default behavior of list views
					nodeid: '',       // if viewid are not supplied, loads a view of this node
					cswnbtnodekey: '',
					IncludeNodeRequired: false,
					UsePaging: true,
					onSelectNode: function(optSelect) {
											var o =  {
												nodeid: '', 
												nodename: '', 
												iconurl: '', 
												cswnbtnodekey: '',
												viewid: ''
											};
											return o;
									},
					onInitialSelectNode: undefined,
					onViewChange: function(newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
					SelectFirstChild: true,
					ShowCheckboxes: false,
					IncludeInQuickLaunch: true
				};
				if(options) $.extend(o, options);

				if(o.onInitialSelectNode === undefined)
					o.onInitialSelectNode = o.onSelectNode;

				var IDPrefix = o.ID + '_';
				var $treediv = $('<div id="'+ IDPrefix + '" class="treediv" />')
								.appendTo($(this));

				var url = o.ViewTreeUrl;
				var dataParam = { 
                    'UsePaging': o.UsePaging,
                    'ViewId': o.viewid,
                    'IDPrefix': IDPrefix,
                    'IsFirstLoad': true,
                    'ParentNodeKey': '',
                    'IncludeNodeRequired': o.IncludeNodeRequired,
                    'IncludeNodeKey': tryParseString(o.cswnbtnodekey, ''),
                    'ShowEmpty': o.showempty,
                    'ForSearch': o.forsearch,
                    'NodePk': tryParseString(o.nodeid,''),
					'IncludeInQuickLaunch': o.IncludeInQuickLaunch
                };

				if( isNullOrEmpty( o.viewid ) )
				{
					url = o.NodeTreeUrl;
				}

				CswAjaxXml({
					url: url,
					data: dataParam,
                    stringify: false,
					success: function ($xml) {
						var selectid = '';
						//var treePlugins = ["themes", "xml_data", "ui", "types", "crrm"];
						var treePlugins = ["themes", "html_data", "ui", "types", "crrm"];

						var treeThemes;
						if( !isNullOrEmpty( o.nodeid ) ) 
						{
							selectid = IDPrefix + o.nodeid;
						}

						var newviewid = $xml.children('viewid').text();
						if(o.viewid !== newviewid )
						{
							o.onViewChange(newviewid, 'tree');
							o.viewid = newviewid;
						}

						var $selecteditem = $xml.find('item[id="'+ selectid + '"]');
						if(selectid === '' || $selecteditem.length === 0)
						{
							if(o.SelectFirstChild)
							{	
								if(o.viewmode === 'list' )
								{
									selectid = $xml.find('item').first().CswAttrXml('id');
									treeThemes = {"dots": false};
								}
								else
								{
									selectid = $xml.find('item').first().find('item').first().CswAttrXml('id');
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
						$itemparents.each(function() { initiallyOpen[i] = $(this).CswAttrXml('id'); i++; });

						var strTypes = $xml.find('types').text();
						var jsonTypes = JSON.parse(strTypes);
						var $treexml = $xml.find('tree').children('root')
						if($treexml.length > 0)
						{
							//var treexmlstring = xmlToString($treexml);

							//var $ul = $('<ul></ul>').appendTo($treediv);

							function _treeXmlToHtml($itemxml)
							{
								var nodeid = $itemxml.CswAttrXml('id').substring(IDPrefix.length);
								var nodename = $itemxml.children('content').children('name').text();
								var treestr = '<li id="'+ $itemxml.CswAttrXml('id') +'" ';
								treestr += '    rel="'+ $itemxml.CswAttrXml('rel') +'" ';
								treestr += '    species="'+ $itemxml.CswAttrXml('species') +'" ';
								treestr += '    class="jstree-'+ $itemxml.CswAttrXml('state') +'" ';
								if($itemxml.CswAttrXml('cswnbtnodekey') !== undefined)
								{
									treestr += '    cswnbtnodekey="'+ $itemxml.CswAttrXml('cswnbtnodekey').replace(/"/g, '&quot;') +'"';
								}
								treestr += '>';
								if(o.ShowCheckboxes)
								{
									treestr += '  <input type="checkbox" class="'+ IDPrefix +'check" id="check_'+ nodeid +'" rel="'+ $itemxml.CswAttrXml('rel') +'" nodeid="'+ nodeid +'" nodename="'+ nodename +'"></input>';
								}
								treestr += '  <a href="#">'+ nodename +'</a>';
								if($itemxml.children('item').length > 0)
								{
									// recurse
									treestr += '<ul>';
									$itemxml.children('item').each(function() { treestr += _treeXmlToHtml($(this)); });
									treestr += '</ul>';
								}
								treestr += '</li>';
								return treestr;
							} // _treeXmlToHtml()

							var treehtmlstring = '<ul>';
							$treexml.children().each(function() { treehtmlstring += _treeXmlToHtml($(this)); });
							treehtmlstring += '</ul>';

							$treediv.bind('init_done.jstree', function(event, data) { 

								// initially_open and initially_select cause multiple event triggers and race conditions.
								// So we'll do it ourselves instead.

								// Open
								$selecteditem = $xml.find('item[id="'+ selectid + '"]');
								var $itemparents = $selecteditem.parents('item').andSelf();
								var initiallyOpen = new Array();
								var i = 0;
								$itemparents.each(function() { 
									$treediv.jstree('open_node', '#' + $(this).CswAttrXml('id') );
								});

								// Select
								$treediv.jstree('select_node', '#' + selectid);

							}).bind('load_node.jstree', function(e, data) {
								$('.'+ IDPrefix +'check').unbind('click');
								$('.'+ IDPrefix +'check').click(function() { return _handleCheck($treediv, $(this)); });

							}).bind('select_node.jstree', function(e, data) { return _firstSelectNode({
									e: e, 
									data: data, 
									url: url,
									$treediv: $treediv, 
									IDPrefix: IDPrefix, 
									onSelectNode: o.onSelectNode,
									onInitialSelectNode: o.onInitialSelectNode,
									viewid: o.viewid,
									UsePaging: o.UsePaging,
									forsearch: o.forsearch
								});
							}).jstree({
								"html_data":
									{
										"data": treehtmlstring,
										"ajax":
											{
												"type": 'POST',
												"url": url,
												"dataType": "xml",
												"data": function($nodeOpening) 
													{
														var nodekey = $nodeOpening.CswAttrXml('cswnbtnodekey');
														var retDataParam = {
                                                            'UsePaging': o.UsePaging,
                                                            'ViewId': o.viewid,
                                                            'IDPrefix': IDPrefix,
                                                            'IsFirstLoad': false,
                                                            'ParentNodeKey': nodekey,
                                                            'IncludeNodeRequired': false,
                                                            'IncludeNodeKey': '',
                                                            'ShowEmpty': false,
                                                            'ForSearch': o.forsearch,
                                                            'NodePk': tryParseString(o.nodeid,''),
															'IncludeInQuickLaunch': false
                                                        };
                                                        return $.param(retDataParam);
													},
												"success": function(data, textStatus, XMLHttpRequest) 
													{
														// this is IE compliant
														var $outerxml = $(XMLHttpRequest.responseXML);
														var $xml = $outerxml.children().first();
														var childhtmlstr = '';
														$xml.children().each(function() { childhtmlstr += _treeXmlToHtml($(this)); });
														return childhtmlstr;
													}
											}
									},
								"ui": {
									"select_limit": 1//,
									//"initially_select": selectid
								},
								"themes": treeThemes,
								"core": {
									//"initially_open": initiallyOpen
								},
								"types": {
									"types": jsonTypes,
									"max_children": -2,
									"max_depth": -2
								},
								"plugins": treePlugins
							});

							// DO NOT define an onSuccess() function here that interacts with the tree.
							// The tree has initalization events that appear to happen asynchronously,
							// and thus having an onSuccess() function that changes the selected node will
							// cause a race condition.
							
							$('.'+ IDPrefix +'check').click(function() { return _handleCheck($treediv, $(this)); });
							
							// case 21424 - Manufacture unique IDs on the expand <ins> for automated testing
							$treediv.find('li').each(function() {
								var $li = $(this);
								$li.children('ins').CswAttrDom('id', $li.CswAttrDom('id') + '_expand');
							});

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
				var IDPrefix = $treediv.CswAttrDom('id');
				$treediv.jstree('select_node', '#' + IDPrefix + o.newnodeid);
			}
	};

	function _firstSelectNode(myoptions)
	{
		var m = {
			e: '', 
			data: '', 
			url: '',
			$treediv: '', 
			IDPrefix: '', 
			onSelectNode: function() {},
			onInitialSelectNode: function() {},
			viewid: '',
			UsePaging: '',
			forsearch: ''
		};
		if(myoptions) $.extend(m, myoptions);
		
		// case 21715 - don't trigger onSelectNode event on first event
		var m2 = {};
		$.extend(m2, m);
		m2.onSelectNode = m.onInitialSelectNode;
		_handleSelectNode(m2);

		// rebind event for next select
		m.$treediv.unbind('select_node.jstree');
		m.$treediv.bind('select_node.jstree', function(e, data) { return _handleSelectNode(m); });
	}

	function _handleSelectNode(myoptions)
	{
		var m = {
			e: '', 
			data: '', 
			url: '',
			$treediv: '', 
			IDPrefix: '', 
			onSelectNode: function() {},
			viewid: '',
			UsePaging: '',
			forsearch: ''
		};
		if(myoptions) $.extend(m, myoptions);

		var Selected = jsTreeGetSelected(m.$treediv);
		var optSelect =  {
			nodeid: Selected.id, 
			nodename: Selected.text, 
			iconurl: Selected.iconurl, 
			cswnbtnodekey: Selected.$item.CswAttrDom('cswnbtnodekey'),
			nodespecies: Selected.$item.CswAttrDom('species'),
			viewid: m.viewid
		};
												
		if(optSelect.nodespecies === "More")
		{
			var ParentNodeKey = '';
			var Parent = m.data.inst._get_parent(m.data.rslt.obj);
			if(Parent !== -1)
			{
				ParentNodeKey = tryParseString(Parent.CswAttrDom('cswnbtnodekey'),'');
			}
                                                    
			var nextDataParam = { 
				'UsePaging': m.UsePaging,
				'ViewId': m.viewid,
				'IDPrefix': m.IDPrefix,
				'IsFirstLoad': false,
				'ParentNodeKey': ParentNodeKey,
				'IncludeNodeRequired': false,
				'IncludeNodeKey': optSelect.cswnbtnodekey,
				'ShowEmpty': false,
				'ForSearch': m.forsearch,
				'NodePk': Selected.id,
				'IncludeInQuickLaunch': false
			};

			// get next page of nodes
			CswAjaxXml({
				url: m.url,
				data: nextDataParam,
				success: function ($xml) 
					{
						var AfterNodeId = m.IDPrefix + optSelect.nodeid;
						var $itemxml = $xml.children().first();
																
						// we have to do these one at a time in successive OnSuccess callbacks, 
						// or else they won't end up in the right place on the tree
						_continue();

						function _continue()
						{
							if($itemxml.length > 0)
							{
								m.$treediv.jstree('create', '#' + AfterNodeId, 'after',
									{ 
										'attr': {
													'id': $itemxml.CswAttrXml('id'), 
													'rel': $itemxml.CswAttrXml('rel'),
													'cswnbtnodekey': $itemxml.CswAttrXml('cswnbtnodekey'),
													'species': $itemxml.CswAttrXml('species')
												},
										'data': $itemxml.children('content').children('name').text(), 
										'state': $itemxml.CswAttrXml('state') 
									}, 
									function() 
									{
										// remove 'More' node
										if(AfterNodeId === $itemxml.CswAttrXml('id'))
										{
											m.$treediv.jstree('remove', '#' + m.IDPrefix + optSelect.nodeid + '[species="More"]' );
										}

										AfterNodeId = $itemxml.CswAttrXml('id');
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
			_clearChecks(m.IDPrefix);
			m.onSelectNode(optSelect);
		}
	}

	function _handleCheck($treediv, $checkbox)
	{
		var $selected = jsTreeGetSelected($treediv);
		return ($selected.$item.CswAttrDom('rel') === $checkbox.CswAttrDom('rel'));
	}

	function _clearChecks(IDPrefix)
	{
		$('.'+ IDPrefix +'check').CswAttrDom('checked', '');
	}

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

