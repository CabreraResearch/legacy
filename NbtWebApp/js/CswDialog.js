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

							_openDiv($div, 400, 400);

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

							_openDiv($div, 800, 600);
						},

		'EditNodeDialog': function (options)
						{
							var o = {
								'nodeid': '',
								'cswnbtnodekey': '',
								'filterToPropId': '',
								'title': '',
								'onEditNode': function (nodeid, nodekey) { }
							};
							if (options) $.extend(o, options);
							var $div = $('<div></div>');
							$div.CswNodeTabs({
								'nodeid': o.nodeid,
								'cswnbtnodekey': o.cswnbtnodekey,
								'filterToPropId': o.filterToPropId,
								'EditMode': 'EditInPopup',
								'title': o.title,
								'onSave': function (nodeid, nodekey)
								{
									$div.dialog('close');
									o.onEditNode(nodeid, nodekey);
								}
							});
							if(o.filterToPropId != '')
								_openDiv($div, 600, 400);
							else
								_openDiv($div, 800, 600);
						},

		'CopyNodeDialog': function (options)
						{
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

							var $copybtn = $div.CswButton({ID: 'copynode_submit', 
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

							var $cancelbtn = $div.CswButton({ID: 'copynode_cancel', 
                                                                        enabledText: 'Cancel', 
                                                                        disabledText: 'Canceling', 
                                                                        onclick: function() {
                                                                                    $div.dialog('close');
                                                                             }
                                                                        });    
							
							_openDiv($div, 400, 300);
						},        
		
		'DeleteNodeDialog': function (options) {
							var o = {
								'nodename': '',
								'nodeid': '',
								'nodekey': '', 
								'onDeleteNode': function(nodeid, nodekey) { },
								'Multi': false,
								'NodeCheckTreeId': ''
								};

							if (options) {
								$.extend(o, options);
							}

							var $div = $('<div><span>Are you sure you want to delete:</span></div>');

							var nodeids = [];
							if(o.Multi)
							{
								var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
								$nodechecks.each(function() {
									var $nodecheck = $(this);
									nodeids[nodeids.length] = $nodecheck.attr('nodeid');
									$div.append('<br/><span style="padding-left: 10px;">' + $nodecheck.attr('nodename') + '</span>');
								});
							} else {
								$div.append('<span>' + o.nodename + '?</span>');
								nodeids[0] = o.nodeid;
							}
							$div.append('<br/><br/>');
	
							var $deletebtn = $div.CswButton({ID: 'deletenode_submit', 
                                                                        enabledText: 'Delete', 
                                                                        disabledText: 'Deleting', 
                                                                        onclick: function() {
														                            $div.dialog('close');
														                            deleteNodes({
																	                            'nodeids': nodeids, 
																	                            'onSuccess': o.onDeleteNode 
																	                            });

                                                                            }
                                                                        });

							var $cancelbtn = $div.CswButton({ID: 'deletenode_cancel', 
                                                                        enabledText: 'Cancel', 
                                                                        disabledText: 'Canceling', 
                                                                        onclick: function() {
                                                                                    $div.dialog('close');
                                                                            }
                                                                        });
							_openDiv($div, 400, 200);
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
							_openDiv($div, 600, 400);
						},

		'SearchDialog': function (options) {
						var o = {
                            viewid: '',
                            nodetypeid: ''
                        }
                        if(options) $.extend(o,options);
                        
                        var $div = $('<div></div>');
						$div.CswSearch('getSearchForm', {
                                viewid: o.viewid,
                                nodetypeid: o.nodetypeid
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

							var $fileuploadcancel = $div.CswButton({ID: 'fileupload_cancel', 
                                                                    enabledText: 'Cancel', 
                                                                    disabledText: 'Canceling', 
                                                                    onclick: function() {
                                                                                $div.dialog('close');
                                                                    }
						    });

							_openDiv($div, 400, 300);
						},

		'ShowLicenseDialog': function (options) {
							var o = {
								'GetLicenseUrl': '/NbtWebApp/wsNBT.asmx/getLicense',
								'AcceptLicenseUrl': '/NbtWebApp/wsNBT.asmx/acceptLicense',
								'onAccept': function() {},
								'onDecline': function() {}
							};
							if(options) $.extend(o, options);
							var $div = $('<div align="center"></div>');
							$div.append('Service Level Agreement<br/>');
							var $licensetextarea = $('<textarea id="license" disabled="true" rows="30" cols="80"></textarea>')
													.appendTo($div);
							$div.append('<br/>');

							CswAjaxJSON({
								url: o.GetLicenseUrl,
								success: function(data)
								{
									$licensetextarea.text(data.license);
								}
							});

							var $acceptbtn = $div.CswButton({ID: 'license_accept', 
                                                            enabledText: 'I Accept', 
                                                            disabledText: 'Accepting...', 
                                                            onclick: function() {
									                                CswAjaxJSON({
										                                url: o.AcceptLicenseUrl,
										                                success: function(data) 
											                                {
												                                $div.dialog('close');
												                                o.onAccept();
											                                }
									                                }); // ajax
								                                }
                                                            });

							var $declinebtn = $div.CswButton({ID: 'license_decline', 
                                                                enabledText: 'I Decline', 
                                                                disabledText: 'Declining...', 
                                                                onclick: function() {
                                                                            $div.dialog('close');
									                                        o.onDecline();
                                                                    }
                                                                });

							_openDiv($div, 800, 600);
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


	function _openDiv($div, width, height)
	{
		$div.dialog({ 'modal': true,
			'width': width,
			'height': height,
			'close': function (event, ui) { 
				$div.remove(); 
			}
		});
	}
	
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
