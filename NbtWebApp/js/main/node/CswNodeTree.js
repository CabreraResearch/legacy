/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

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
					onSelectNode: null, // function(optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', cswnbtnodekey: '', viewid: '' }; return o; },
					onInitialSelectNode: undefined,
					onViewChange: null, // function(newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
					SelectFirstChild: true,
					ShowCheckboxes: false,
					IncludeInQuickLaunch: true
				};
				if(options) $.extend(o, options);

				if(o.onInitialSelectNode === undefined)
					o.onInitialSelectNode = o.onSelectNode;

				var idPrefix = o.ID + '_';
				var $treediv = $('<div id="'+ idPrefix + '" class="treediv" />')
								.appendTo($(this));

				var url = o.ViewTreeUrl;
				var dataParam = { 
					UsePaging: o.UsePaging,
					ViewId: o.viewid,
					IDPrefix: idPrefix,
					IsFirstLoad: true,
					ParentNodeKey: '',
					IncludeNodeRequired: o.IncludeNodeRequired,
					IncludeNodeKey: tryParseString(o.cswnbtnodekey),
					ShowEmpty: o.showempty,
					ForSearch: o.forsearch,
					NodePk: tryParseString(o.nodeid),
					IncludeInQuickLaunch: o.IncludeInQuickLaunch
				};

				if( isNullOrEmpty( o.viewid ) )
				{
					url = o.NodeTreeUrl;
				}

				CswAjaxJson({
					url: url,
					data: dataParam,
					stringify: false,
					success: function (data) {
						var idToSelect = '';
						//var treePlugins = ["themes", "xml_data", "ui", "types", "crrm"];
						var treePlugins = ["themes", "html_data", "ui", "types", "crrm"];
					    var treeThemes;
						if( !isNullOrEmpty( o.nodeid ) ) 
						{
							idToSelect = idPrefix + o.nodeid;
						}

						var newviewid = data.viewid;
						if (o.viewid !== newviewid )
						{
							if(isFunction(o.onViewChange)) {
							    o.onViewChange(newviewid, 'tree');
							}
						    o.viewid = newviewid;
						}

					    var treeData = data.tree;
					    var jsonTypes = data.types;
					    
						//var $selecteditem = data.find('item[id="'+ selectid + '"]');
					    var selectLevel = -1;
						if (o.SelectFirstChild) {	
							if (o.viewmode === 'list' ) {
								selectLevel = 1;
								treeThemes = {"dots": false};
							} else {
								selectLevel = 2;
								treeThemes = {"dots": true};
							}
						} 
						else 
						{
							if (isNullOrEmpty(idToSelect)) {
								idToSelect = idPrefix + 'root';
							}
						}
					    
					    var hasNodes = false;
					    var selectid = '';
						function treeJsonToHtml(json,level)
						{
					        hasNodes = true;
					        var id = json.attr.id;
					        if (idToSelect === id || (level === selectLevel && isNullOrEmpty(selectid))) {
					            selectid = id;
					        }
					        
					        var nodeid = tryParseString(id.substring(idPrefix.length));
							var nodename = tryParseString(json.data);
					        var nbtnodekey = tryParseString(json.attr.cswnbtnodekey);
					        var rel = tryParseString(json.attr.rel);
					        var species = tryParseString(json.attr.species);
					        
					        var treestr = '<li id="'+ id +'" ';
							treestr += '    rel="'+ rel +'" ';
							treestr += '    species="'+ species +'" ';
							treestr += '    class="jstree-'+ json.attr.state +'" ';
							if (!isNullOrEmpty(nbtnodekey)) {
								treestr += '    cswnbtnodekey="'+ nbtnodekey.replace(/"/g, '&quot;') +'"';
							}
							treestr += '>';
							if (o.ShowCheckboxes) {
								treestr += '  <input type="checkbox" class="'+ idPrefix +'check" id="check_'+ nodeid +'" rel="'+ rel +'" nodeid="'+ nodeid +'" nodename="'+ nodename +'"></input>';
							}
							treestr += '  <a href="#">'+ nodename +'</a>';
					        var children = json.children;
					        for (var child in children) {
								// recurse
								if (children.hasOwnProperty(child)) {
								    var childNode = children[child];
								    treestr += '<ul>';
								    treestr += treeJsonToHtml(childNode,2);
								    treestr += '</ul>';
								}
					        }
							treestr += '</li>';
							return treestr;
						} // _treeXmlToHtml()
					    
					    var treehtmlstring = '<ul>';
					    for (var parent in treeData) {
					        if (treeData.hasOwnProperty(parent)) {
					            var parentNode = treeData[parent];
					            treehtmlstring += treeJsonToHtml(parentNode, 1);
					        }
					    }
                        treehtmlstring += '</ul>';

					    if(hasNodes) {
							var $nodepreview = undefined;
							var nodepreview_timeout;

							$treediv.bind('init_done.jstree', function() { 

								// initially_open and initially_select cause multiple event triggers and race conditions.
								// So we'll do it ourselves instead.
								// Open
							    if (!isNullOrEmpty(selectid)) {
							        var $selecteditem = $treediv.find('#' + selectid);
							        var $itemparents = $selecteditem.parents().andSelf();
							        $itemparents.each(function() {
							            $treediv.jstree('open_node', '#' + $(this).CswAttrXml('id'));
							        });

							        // Select
							        $treediv.jstree('select_node', '#' + selectid);
							    }
							}).bind('load_node.jstree', function() {
								$('.'+ idPrefix +'check').unbind('click');
								$('.'+ idPrefix +'check').click(function() { return handleCheck($treediv, $(this)); });

							}).bind('select_node.jstree', function(e, newData) { 
								return firstSelectNode({
									e: e, 
									data: newData, 
									url: url,
									$treediv: $treediv, 
									IDPrefix: idPrefix, 
									onSelectNode: o.onSelectNode,
									onInitialSelectNode: o.onInitialSelectNode,
									viewid: o.viewid,
									UsePaging: o.UsePaging,
									forsearch: o.forsearch
								});
								
							}).bind('hover_node.jstree', function(e, data) {
								var $hoverLI = $(data.rslt.obj[0]);
								var nodeid = $hoverLI.CswAttrDom('id').substring(idPrefix.length);
								var cswnbtnodekey = $hoverLI.CswAttrDom('cswnbtnodekey');

								nodepreview_timeout = setTimeout(function() {
									$nodepreview = $.CswNodePreview('open', {
										ID: nodeid + "_preview",
										nodeid: nodeid, 
										cswnbtnodekey: cswnbtnodekey,
										X: data.args[1].pageX,
										Y: data.args[1].pageY
									});
								}, 1000);

							}).bind('dehover_node.jstree', function(e, data) {
								var selected = jsTreeGetSelected($treediv);
								clearTimeout(nodepreview_timeout);
								if($nodepreview !== undefined)
								{
									$nodepreview.CswNodePreview('close');
									$nodepreview = undefined;
								}

							}).jstree({
								"html_data":
									{
										"data": treehtmlstring,
										"ajax":
											{
												type: 'POST',
												url: url,
												dataType: "json",
											    contentType: 'application/json; charset=utf-8',
												data: function($nodeOpening) {
												    var nodekey = $nodeOpening.CswAttrXml('cswnbtnodekey');
													var retDataParam = {
														UsePaging: o.UsePaging,
														ViewId: o.viewid,
														IDPrefix: idPrefix,
														IsFirstLoad: false,
														ParentNodeKey: nodekey,
														IncludeNodeRequired: false,
														IncludeNodeKey: '',
														ShowEmpty: false,
														ForSearch: o.forsearch,
														NodePk: tryParseString(o.nodeid),
														IncludeInQuickLaunch: false
													};
													return JSON.stringify(retDataParam);
												},
												success: function(rawData) {
												    var newData = JSON.parse(rawData.d);
												    var nodeData = newData.tree;
													var childhtmlstr = '';
													for (var nodeItem in nodeData) {
														if (nodeData.hasOwnProperty(nodeItem)) {
														    var thisNode = nodeData[nodeItem];
														    childhtmlstr += treeJsonToHtml(thisNode);
														}
													}
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
							
							$('.'+ idPrefix +'check').click(function() { return handleCheck($treediv, $(this)); });
							
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
		        };  
				if (optSelect) {
					$.extend(o, optSelect);
				}
		        var $treediv = $(this).children('.treediv');
				var idPrefix = $treediv.CswAttrDom('id');
				$treediv.jstree('select_node', '#' + idPrefix + o.newnodeid);
			}
	};

	function firstSelectNode(myoptions)
	{
		var m = {
			e: '', 
			data: '', 
			url: '',
			$treediv: '', 
			IDPrefix: '', 
			onSelectNode: null, //function() {},
			onInitialSelectNode: null, //function() {},
			viewid: '',
			UsePaging: '',
			forsearch: ''
		};
		if(myoptions) $.extend(m, myoptions);
		
		// case 21715 - don't trigger onSelectNode event on first event
		var m2 = {};
		$.extend(m2, m);
		m2.onSelectNode = m.onInitialSelectNode;
		handleSelectNode(m2);

		// rebind event for next select
		m.$treediv.unbind('select_node.jstree');
		m.$treediv.bind('select_node.jstree', function() { return handleSelectNode(m); });
	}

	function handleSelectNode(myoptions)
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

		var selected = jsTreeGetSelected(m.$treediv);
		var optSelect =  {
			nodeid: selected.id, 
			nodename: selected.text, 
			iconurl: selected.iconurl, 
			cswnbtnodekey: selected.$item.CswAttrDom('cswnbtnodekey'),
			nodespecies: selected.$item.CswAttrDom('species'),
			viewid: m.viewid
		};
												
		if(optSelect.nodespecies === "More")
		{
			var parentNodeKey = '';
			var parent = m.data.inst._get_parent(m.data.rslt.obj);
			if(parent !== -1)
			{
				parentNodeKey = tryParseString(parent.CswAttrDom('cswnbtnodekey'),'');
			}
													
			var nextDataParam = { 
				UsePaging: m.UsePaging,
				ViewId: m.viewid,
				IDPrefix: m.IDPrefix,
				IsFirstLoad: false,
				ParentNodeKey: parentNodeKey,
				IncludeNodeRequired: false,
				IncludeNodeKey: optSelect.cswnbtnodekey,
				ShowEmpty: false,
				ForSearch: m.forsearch,
				NodePk: selected.id,
				IncludeInQuickLaunch: false
			};

			// get next page of nodes
			CswAjaxJson({
				url: m.url,
				data: nextDataParam,
				success: function (data) 
					{
						var afterNodeId = m.IDPrefix + optSelect.nodeid;
						var itemJson = data.tree;
																
						// we have to do these one at a time in successive OnSuccess callbacks, 
						// or else they won't end up in the right place on the tree
						doContinue();

						function doContinue()
						{
							if(itemJson.length > 0)
							{
								m.$treediv.jstree('create', '#' + afterNodeId, 'after',
//									{ 
//										'attr': {
//													'id': itemJson.CswAttrXml('id'), 
//													'rel': itemJson.CswAttrXml('rel'),
//													'cswnbtnodekey': itemJson.CswAttrXml('cswnbtnodekey'),
//													'species': itemJson.CswAttrXml('species')
//												},
//										'data': itemJson.children('content').children('name').text(), 
//										'state': itemJson.CswAttrXml('state') 
									    itemJson,
//									}, 
									function() 
									{
										// remove 'More' node
										if(afterNodeId === itemJson.attr.id)
										{
											m.$treediv.jstree('remove', '#' + m.IDPrefix + optSelect.nodeid + '[species="More"]' );
										}

										afterNodeId = itemJson.attr.id;
//										itemJson = itemJson.next();
//										_continue();
									}, 
									true, true);

							} // if($itemxml.length > 0)
						} // _continue()

					} // success
				}); // ajax
		}
		else 
		{
			clearChecks(m.IDPrefix);
			m.onSelectNode(optSelect);
		}
	}

	function handleCheck($treediv, $checkbox)
	{
		var $selected = jsTreeGetSelected($treediv);
		return ($selected.$item.CswAttrDom('rel') === $checkbox.CswAttrDom('rel'));
	}

	function clearChecks(IDPrefix)
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

