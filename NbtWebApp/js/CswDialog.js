; (function ($) {
		
	var PluginName = 'CswDialog';

	var methods = {

		// Specialized

		'AddWelcomeItemDialog': function(options) {
							var o = {
								'onAdd': function() { }
								};

							if (options) {
								$.extend(o, options);
							}

							var $div = $('<div></div>');
							
							$div.CswWelcome('getAddItemForm', { 'onAdd': function() { $div.dialog('close'); o.onAdd(); } } );

							$div.dialog({ 'modal': true,
								'width': 400,
								'height': 400,
								'close': function(event, ui) { $div.remove(); } 
							});

						}, // AddWelcomeItemDialog

		'AddNodeDialog': function (options) {
							var o = {
								'nodetypeid': '', 
                                'relatednodeid': '',
								'onAddNode': function(nodeid, cswnbtnodekey) { }
								};

							if (options) {
								$.extend(o, options);
							}

							var $div = $('<div></div>');
							$div.CswNodeTabs({
								'nodetypeid': o.nodetypeid,
                                'relatednodeid': o.relatednodeid,
								'EditMode': 'AddInPopup',
								'onSave': function (nodeid, cswnbtnodekey) {
									$div.dialog('close');
									o.onAddNode(nodeid, cswnbtnodekey);
								}
							});
							$div.dialog({ 'modal': true,
								'width': 800,
								'height': 600,
								'close': function(event, ui) { $div.remove(); } 
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
								'height': 600,
								'close': function(event, ui) { $div.remove(); } 
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

							var $copynode = $('<input type="button"/>');
                            $copynode.CswButton('init', {ID: 'copynode_submit', 
                                            enabledText: 'Copy', 
                                            disabledText: 'Copying', 
                                            onclick: function() {
									                $div.dialog('close');
									                copyNode({
												                'nodeid': o.nodeid, 
												                'nodekey': o.nodekey, 
												                'onSuccess': o.onCopyNode 
											                   });
								                }
                                            });
                            $div.append($copynode);                                

							var $cancelcopy = $('<input type="button"/>');
                            $cancelcopy.CswButton('init', {ID: 'copynode_cancel', 
                                            enabledText: 'Cancel', 
                                            disabledText: 'Canceling', 
                                            onclick: function() {
                                                        $div.dialog('close');
                                                 }
                                            });    
                            $div.append($cancelcopy);

							
							$div.dialog({ 'modal': true,
								'width': 400,
								'height': 300,
								'close': function(event, ui) { $div.remove(); } 
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

							var $deletenode = $('<input type="button" id="" name="deletenode_submit" value="" />');
                            $deletenode.CswButton('init', {ID: 'deletenode_submit', 
                                            enabledText: 'Delete', 
                                            disabledText: 'Deleting', 
                                            onclick: function() {
                                                        $div.dialog('close');
									                    deleteNode({
												            'nodeid': o.nodeid, 
												            'nodekey': o.nodekey, 
												            'onSuccess': o.onDeleteNode 
										                });

                                                }
                                            });
                            $div.append($deletenode);

							var $deletecancel = $('<input type="button" />');
							$deletecancel.CswButton('init', {ID: 'deletenode_cancel', 
                                            enabledText: 'Cancel', 
                                            disabledText: 'Canceling', 
                                            onclick: function() {
                                                        $div.dialog('close');
                                                }
                                            });	
                            $div.append($deletecancel);

							$div.dialog({ 'modal': true,
								'width': 400,
								'height': 200,
								'close': function(event, ui) { $div.remove(); } 
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
								'height': 400,
								'close': function(event, ui) { $div.remove(); } 
							});
						},

		'SearchDialog': function (options) {
						var o = {
                            viewid: '',
                            nodetypeid: '',
                            onSearch: function() { }
                        }
                        if(options) $.extend(o,options);
                        
                        var $div = $('<div></div>');
						$div.CswSearch('getSearchForm', {
                                viewid: o.viewid,
                                nodetypeid: o.nodetypeid,
                                onSearch: o.onSearch
                            });
						
                        $div.dialog({ 'modal': true,
							'width': 800,
							'height': 600
						    });
						},


		'FileUploadDialog': function (options) {
							var o = {
								url: '',
								params: {},
								onSuccess: function() { }
							};
							if(options) {
								$.extend(o, options);
							}

							var $div = $('<div></div>');
								
							var uploader = new qq.FileUploader({
								element: $div.get(0),
								action: o.url,
								params: o.params,
								debug: false,
								onComplete: function() 
									{ 
										$div.dialog('close'); 
										o.onSuccess(); 
									}
							});

							var $fileuploadcancel = $('<input type="button" id="" name="fileupload_cancel" value="" />');
							$fileuploadcancel.CswButton('init', {ID: 'fileupload_cancel', 
                                            enabledText: 'Cancel', 
                                            disabledText: 'Canceling', 
                                            onclick: function() {
                                                        $div.dialog('close');
                                                }
                                            });	
                            $div.append($fileuploadcancel);                                
                            
						    $div.dialog({ 'modal': true,
								    'width': 400,
								    'height': 300,
								    'close': function(event, ui) { $div.remove(); } 
							    });
						},

		// Generic

//		'OpenPopup': function(url) { 
//							var popup = window.open(url, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
//							popup.focus();
//							return popup;
//						},
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
