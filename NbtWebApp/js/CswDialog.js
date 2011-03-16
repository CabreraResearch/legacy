; (function ($) {
		
	var PluginName = 'CswDialog';

	var methods = {

		// Specialized

		'AddWelcomeItemDialog': function() {
							var $div = $('<div></div>');
							var $table = $div.CswTable('init', { ID: 'addwelcomeitem_tbl' });

							$table.CswTable('cell', 1, 1).append('Type:');
							var $typeselect = $('<select id="welcome_type" name="welcome_type"></select>')
												.appendTo($table.CswTable('cell', 1, 2));
							$typeselect.append('<option value="Add">Add</option>');
							$typeselect.append('<option value="Link">Link</option>');
							$typeselect.append('<option value="Search">Search</option>');
							$typeselect.append('<option value="Text">Text</option>');
						
							$table.CswTable('cell', 2, 1).append('View:');
							$table.CswTable('cell', 2, 2).CswViewSelect({
																			'ID': 'welcome_viewsel',
																			//'viewid': '',
																			//'onSelect': function(optSelect) { },
																		});

							$table.CswTable('cell', 3, 1).append('Add New:');
							$table.CswTable('cell', 3, 2).CswNodeTypeSelect({
																			'ID': 'welcome_ntsel'
																			});

							$table.CswTable('cell', 4, 1).append('Text:');
							$table.CswTable('cell', 4, 2).append('<input type="text" id="welcome_text" value="" />');

							$table.CswTable('cell', 5, 1).append('Use Button:');
							var $buttonsel = $('<select id="welcome_button" />')
												.appendTo($table.CswTable('cell', 5, 2));
							$buttonsel.append('<option value="blank.gif"></option>');

							var $buttonimg = $('<img id="welcome_btnimg" />')
												.appendTo( $table.CswTable('cell', 6, 2) );

							$buttonsel.change(function(event) { 
								$buttonimg.attr('src', 'Images/biggerbuttons/' + $buttonsel.val()); 
							});

							CswAjaxXml({ 
										'url': '/NbtWebApp/wsNBT.asmx/getWelcomeButtonIconList',
										'success': function($xml) { 
													$xml.children().each(function() {
														var $icon = $(this);
														var filename = $icon.attr('filename');
														if(filename != 'blank.gif') 
														{
															$buttonsel.append('<option value="'+ filename +'">'+ filename +'</option>');
														}
													}); // each
												} // success
										}); // CswAjaxXml

							$div.dialog({ 'modal': true,
								'width': 400,
								'height': 400
							});

						}, // AddWelcomeItemDialog

		'AddNodeDialog': function (options) {
							var o = {
								'nodetypeid': '', 
								'onAddNode': function(nodeid, nodekey) { }
								};

							if (options) {
								$.extend(o, options);
							}

							var $div = $('<div></div>');
							$div.CswNodeTabs({
								'nodetypeid': o.nodetypeid,
								'EditMode': 'AddInPopup',
								'onSave': function (nodeid,cswnbtnodekey) {
									$div.dialog('close');
									o.onAddNode(nodeid,cswnbtnodekey);
								}
							});
							$div.dialog({ 'modal': true,
								'width': 800,
								'height': 600
							});
						},        
		
		'EditNodeDialog': function (options) {
							var o = {
								'nodeid': '',
								'nodekey': '', 
								'onEditNode': function(nodeid, nodekey) { }
								};

							if (options) {
								$.extend(o, options);
							}

							var $div = $('<div></div>');
							$div.CswNodeTabs({
								'nodeid': o.nodeid,
								'nodekey': o.nodekey,
								'EditMode': 'EditInPopup',
								'onSave': function (nodeid,nodekey) {
									$div.dialog('close');
									o.onEditNode(nodeid,nodekey);
								}
							});
							$div.dialog({ 'modal': true,
								'width': 800,
								'height': 600
							});
						},

		'CopyNodeDialog': function (options) {
							var o = {
								'nodename': '',
								'nodeid': '',
								'nodekey': '', 
								'onCopyNode': function(nodeid, nodekey) { }
								};

							if (options) {
								$.extend(o, options);
							}

							var $div = $('<div>Copying: ' + o.nodename + '<br/><br/></div>');

							$('<input type="button" id="copynode_submit" name="copynode_submit" value="Copy" />')
								.appendTo($div)
								.click(function () {
									$div.dialog('close');
									copyNode({
												'nodeid': o.nodeid, 
												'nodekey': o.nodekey, 
												'onSuccess': o.onCopyNode 
											   });
								});

							$('<input type="button" id="copynode_cancel" name="copynode_cancel" value="Cancel" />')
								.appendTo($div)
								.click(function () {
									$div.dialog('close');
								});

							
							$div.dialog({ 'modal': true,
								'width': 400,
								'height': 300
							});
						},        
		
		'DeleteNodeDialog': function (options) {
							var o = {
								'nodename': '',
								'nodeid': '',
								'nodekey': '', 
								'onDeleteNode': function(nodeid, nodekey) { }
								};

							if (options) {
								$.extend(o, options);
							}

							var $div = $('<div>Are you sure you want to delete: ' + o.nodename + '?<br/><br/></div>');

							$('<input type="button" id="deletenode_submit" name="deletenode_submit" value="Delete" />')
								.appendTo($div)
								.click(function () {
									$div.dialog('close');
									deleteNode({
												'nodeid': o.nodeid, 
												'nodekey': o.nodekey, 
												'onSuccess': o.onDeleteNode 
											   });
								});

							$('<input type="button" id="deletenode_cancel" name="deletenode_cancel" value="Cancel" />')
								.appendTo($div)
								.click(function () {
									$div.dialog('close');
								});

							$div.dialog({ 'modal': true,
								'width': 400,
								'height': 200
							});
						},

		'AboutDialog': function () {
							var $div = $('<div></div>');
							CswAjaxXml({
								url: '/NbtWebApp/wsNBT.asmx/getAbout',
								data: '',
								success: function ($xml) {
									$div.append('NBT Assembly Version: ' + $xml.children('assembly').text() + '<br/><br/>');
									var $table = $div.CswTable('init', { ID: 'abouttable' });
									var row = 1;
									$xml.children('component').each(function () {
										var $namecell = $table.CswTable('cell', row, 1);
										var $versioncell = $table.CswTable('cell', row, 2);
										var $copyrightcell = $table.CswTable('cell', row, 3);
										$namecell.css('padding', '2px 5px 2px 5px');
										$versioncell.css('padding', '2px 5px 2px 5px');
										$copyrightcell.css('padding', '2px 5px 2px 5px');
										var $component = $(this);
										$namecell.append($component.children('name').text());
										$versioncell.append($component.children('version').text());
										$copyrightcell.append($component.children('copyright').text());
										row++;
									});
								}
							});
							$div.dialog({ 'modal': true,
								'width': 600,
								'height': 400
							});
						},

		'SearchDialog': function (viewid, onSearchSubmit, props) {
						var $div = $('<div></div>');
						CswAjaxXml({
							url: '/NbtWebApp/wsNBT.asmx/getSearch',
							data: 'ViewNum: ' + viewid ,
							success: function ($xml) {
									
							}
						});
						$div.dialog({ 'modal': true,
							'width': 800,
							'height': 600
						});
					},

		// Generic

		'OpenPopup': function(url) { 
							var popup = window.open(url, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
							popup.focus();
							return popup;
						},
		'OpenDialog': function (id, url) {
							var $dialogdiv = $('<div id="' + id + '"></div>');
							$dialogdiv.load(url,
											{},
											function (responseText, textStatus, XMLHttpRequest) {
												$dialogdiv.appendTo('body')
														  .dialog();
											});
						},
		'CloseDialog': function (id) {
							$('#' + id)
								.dialog('close')
								.remove();
						}
	};
	
	// Method calling logic
	$.CswDialog = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);
