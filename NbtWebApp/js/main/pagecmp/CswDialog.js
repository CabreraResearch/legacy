/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswTable.js" />
/// <reference path="../actions/CswAuditHistoryGrid.js" />
/// <reference path="../node/CswNodeTabs.js" />

; (function ($) { /// <param name="$" type="jQuery" />
		
	var pluginName = 'CswDialog';

	var methods = {

		// Specialized

		'ExpireDialog': function(options) {
			var o = {
				'onYes': function() { }
				};

			if (options) $.extend(o, options);

			var $div = $('<div></div>');
							
			$div.append('<p>Your session is about to time out.  Would you like to continue working?</p>');

			$div.CswButton({
				'ID': 'renew_btn',
				'enabledText': 'Yes',
				'onclick': function() { $div.dialog('close'); o.onYes(); }
			});

			_openDiv($div, 300, 250);

		}, // ExpireDialog
        'AddWelcomeItemDialog': function(options) {
			var o = {
				'onAdd': function() { }
				};

			if (options) $.extend(o, options);

			var $div = $('<div></div>');
							
			$div.CswWelcome('getAddItemForm', { 'onAdd': function() { $div.dialog('close'); o.onAdd(); } } );

			_openDiv($div, 400, 400);

		}, // AddWelcomeItemDialog
        'AddViewDialog': function (options) {
			var o = {
				'ID': 'addviewdialog',
				'onAddView': function (newviewid) { },
				'viewid': '',
				'viewmode': ''
			};
			if (options) $.extend(o, options);

			var $div = $('<div></div>');
			var $table = $div.CswTable('init', { 'ID': o.ID + '_tbl', 'FirstCellRightAlign': true });

			$table.CswTable('cell', 1, 1).append('Name:');
			var $nametextcell = $table.CswTable('cell', 1, 2); 
			var $nametextbox =  $nametextcell.CswInput('init',{ID: o.ID + '_nametb',
																type: CswInput_Types.text,
																cssclass: 'textinput'
														});
			var $displaymodeselect = $('<select id="' + o.ID + '_dmsel" />');
			if(isNullOrEmpty(o.viewid))
			{
				$table.CswTable('cell', 2, 1).append('Display Mode:');
				$displaymodeselect.append('<option value="List">List</option>');
				$displaymodeselect.append('<option value="Tree">Tree</option>');
				$displaymodeselect.append('<option value="Grid">Grid</option>');
				$displaymodeselect.appendTo($table.CswTable('cell', 2, 2));
			}

			var v = makeViewVisibilitySelect($table, 3, 'Available to:');
			var $savebtn = $div.CswButton({ID: o.ID + '_submit', 
													enabledText: 'Create View', 
													disabledText: 'Creating View', 
													onclick: function() {
																			
															var createData = {};
															createData.ViewName = $nametextbox.val();
															createData.ViewId = o.viewid;
															if(isNullOrEmpty(o.viewmode))
															{
																createData.ViewMode = $displaymodeselect.val();
															} else {
																createData.ViewMode = o.viewmode;
															}
															if(!isNullOrEmpty(v.getvisibilityselect()))
															{
																createData.Visibility = v.getvisibilityselect().val();
																createData.VisibilityRoleId = v.getvisroleselect().val();
																createData.VisibilityUserId = v.getvisuserselect().val();
															} else {
																createData.Visibility = "";
																createData.VisibilityRoleId = "";
																createData.VisibilityUserId = "";
															}

															CswAjaxJson({
																url: '/NbtWebApp/wsNBT.asmx/createView',
																data: createData,
																success: function(data) {
																	$div.dialog('close');
																	o.onAddView(data.newviewid);
																}, 
																error: function() {
																	$savebtn.CswButton('enable');
																}
															});
														}
													});

			var $cancelbtn = $div.CswButton({ID: o.ID + '_cancel', 
														enabledText: 'Cancel', 
														disabledText: 'Canceling', 
														onclick: function() {
																$div.dialog('close');
																}
														});    

			_openDiv($div, 400, 200);
		}, // AddViewDialog
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
				'EditMode': EditMode.AddInPopup.name,
				'onSave': function (nodeid, cswnbtnodekey) {
					$div.dialog('close');
					o.onAddNode(nodeid, cswnbtnodekey);
				},
				'onInitFinish': function() {
					_openDiv($div, 800, 600);
				},
				'ShowAsReport': false
			});

		}, // AddNodeDialog
        EditNodeDialog: function (options) {
			var o = {
				nodeid: '',
				nodepk: '',
				cswnbtnodekey: '',
				Multi: false,
				filterToPropId: '',
				title: '',
				onEditNode: null, // function (nodeid, nodekey) { },
				date: ''     // viewing audit records
			};
			if (options) $.extend(o, options);
            var nodeid, nodekey;
            if (o.Multi && isArray(o.nodeid)) {
                nodeid = o.nodeid[0];
                nodekey = o.cswnbtnodekey[0];
            } else {
                nodeid = tryParseString(o.nodeid, o.nodepk);
                nodekey = o.cswnbtnodekey;
            }
            
		    var $div = $('<div></div>');
							
			var myEditMode = EditMode.EditInPopup.name;
			var $table = $div.CswTable();
			if(false === isNullOrEmpty(o.date) && false === o.Multi) {
				myEditMode = EditMode.AuditHistoryInPopup.name;
				$table.CswTable('cell', 1, 1).CswAuditHistoryGrid({
					ID: nodeid + '_history',
					nodeid: nodeid,
					onEditNode: o.onEditNode,
					JustDateColumn: true,
					selectedDate: o.date,
					onSelectRow: function(date) { setupTabs(date); },
					allowEditRow: false
				});
			}
			var $tabcell = $table.CswTable('cell', 1, 2);

			setupTabs(o.date);

			function setupTabs(date)
			{
				$tabcell.empty();
				$tabcell.CswNodeTabs({
					nodeid: nodeid,
					cswnbtnodekey: nodekey,
					filterToPropId: o.filterToPropId,
				    Multi: o.Multi,    
					EditMode: myEditMode,
					title: o.title,
					tabid: $.CswCookie('get', CswCookieName.CurrentTabId),
					date: date,
				    onMultiSave: function () {
				        
				    }, 
					onSave: function (nodeid, nodekey, tabcount) {
						unsetChanged();
						if(tabcount === 1)
						{
							$div.dialog('close');
						}
						setupTabs(date);
						if (isFunction(o.onEditNode)) {
							o.onEditNode(nodeid, nodekey);
						}
					},
					onBeforeTabSelect: function (tabid) {
						return manuallyCheckChanges();
					},
					onTabSelect: function (tabid) {
						$.CswCookie('set', CswCookieName.CurrentTabId, tabid);
					},
					onPropertyChange: function (propid, propname) {
						setChanged();
					}
				});
			} // _setupTabs()

			if(o.filterToPropId !== '') {
			    _openDiv($div, 600, 400);
			} else {
			    _openDiv($div, 900, 600);
			}
		}, // EditNodeDialog
        CopyNodeDialog: function (options) {
			var o = {
				'nodename': '',
				'nodeid': '',
				'cswnbtnodekey': '', 
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
															copyNode({
																		'nodeid': o.nodeid, 
																		'nodekey': o.nodekey, 
																		'onSuccess': function(nodeid, nodekey) { 
																			$div.dialog('close');
																			o.onCopyNode(nodeid, nodekey);
																		},
																		'onError':  function() {
																			$copybtn.CswButton('enable');
																		}
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
		}, // CopyNodeDialog       
		DeleteNodeDialog: function (options, nodeids, nodekeys) {
			var o = {
				nodename: '',
				nodeid: '',
				cswnbtnodekey: '', 
				onDeleteNode: null, //function(nodeid, nodekey) { },
				Multi: false,
				NodeCheckTreeId: ''
			};

			if (options) {
				$.extend(o, options);
			}

			var $div = $('<div><span>Are you sure you want to delete:</span></div>');

		    if (false === isArray(nodeids) || false === isArray(nodekeys)) {
				nodeids = [];
				nodekeys = [];
			}
		    
			if(o.Multi)
			{
				if (nodeids.length === 0 || nodekeys.length === 0 ) {
				    var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
				    $nodechecks.each(function() {
				        var $nodecheck = $(this);
				        nodeids[nodeids.length] = $nodecheck.CswAttrDom('nodeid');
				        //nodekeys[nodekeys.length] = $nodecheck.CswAttrDom('cswnbtnodekey');
				        $div.append('<br/><span style="padding-left: 10px;">' + $nodecheck.CswAttrDom('nodename') + '</span>');
				    });
				} else {
				    for(var i=0; i<o.nodename.length; i++) {
				        $div.append('<br/><span style="padding-left: 10px;">' + o.nodename[i] + '</span>');
				    }
				}
			} else {
				$div.append('<span>' + o.nodename + '?</span>');
				nodeids[0] = o.nodeid;
				nodekeys[0] = o.cswnbtnodekey;
			}
			$div.append('<br/><br/>');
	
			var $deletebtn = $div.CswButton({ID: 'deletenode_submit', 
														enabledText: 'Delete', 
														disabledText: 'Deleting', 
														onclick: function() {
																	deleteNodes({
																				nodeids: nodeids, 
																				nodekeys: nodekeys,
																				onSuccess: function(nodeid, nodekey) {
																					$div.dialog('close');
																					if (isFunction(o.onDeleteNode)) {
																					    o.onDeleteNode(nodeid, nodekey);
																					}
																				},
																				onError: function() {
																					$deletebtn.CswButton('enable');
																				}
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
		}, // DeleteNodeDialog
        AboutDialog: function () {
			var $div = $('<div></div>');
			CswAjaxJson({
				url: '/NbtWebApp/wsNBT.asmx/getAbout',
				data: {},
				success: function (data) {
					$div.append('NBT Assembly Version: ' + data.assembly + '<br/><br/>');
					var $table = $div.CswTable('init', { ID: 'abouttable' });
					var row = 1;

					var components = data.components;
					for (var comp in components) {
						if (components.hasOwnProperty(comp)) {
							var thisComp = components[comp];
							var $namecell = $table.CswTable('cell', row, 1);
							var $versioncell = $table.CswTable('cell', row, 2);
							var $copyrightcell = $table.CswTable('cell', row, 3);
							$namecell.css('padding', '2px 5px 2px 5px');
							$versioncell.css('padding', '2px 5px 2px 5px');
							$copyrightcell.css('padding', '2px 5px 2px 5px');
							$namecell.append(thisComp.name);
							$versioncell.append(thisComp.version);
							$copyrightcell.append(thisComp.copyright);
							row++;
						}
					}
				}
			});
			_openDiv($div, 600, 400);
		}, // AboutDialog
		FileUploadDialog: function (options) {
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
		}, // FileUploadDialog
        ShowLicenseDialog: function (options) {
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

			CswAjaxJson({
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
													CswAjaxJson({
														url: o.AcceptLicenseUrl,
														success: function(data) 
															{
																$div.dialog('close');
																o.onAccept();
															},
														error: function() 
															{
																$acceptbtn.CswButton('enable');	
															}
													}); // ajax
												} // onclick
											}); // CswButton

			var $declinebtn = $div.CswButton({ID: 'license_decline', 
												enabledText: 'I Decline', 
												disabledText: 'Declining...', 
												onclick: function() {
															$div.dialog('close');
															o.onDecline();
													}
												});

			_openDiv($div, 800, 600);
		}, // ShowLicenseDialog
        PrintLabelDialog: function (options) {

			var o = {
				'ID': 'print_label',
				'GetPrintLabelsUrl': '/NbtWebApp/wsNBT.asmx/getLabels',
				'GetEPLTextUrl': '/NbtWebApp/wsNBT.asmx/getEPLText',
				'nodeid': '',
				'propid': ''
			};
			if(options) $.extend(o, options);
							
			var $div = $('<div align="center"></div>');
							
			$div.append('Select a Label to Print:<br/>');
			var $labelsel_div = $('<div />')
								.appendTo($div);
			var $labelsel;

            var jData = { PropId: o.propid };
			CswAjaxJson({
				url: o.GetPrintLabelsUrl,
				data: jData,
				success: function(data)
				{
					if(data.labels.length > 0)
					{
						$labelsel = $('<select id="' + o.ID + '_labelsel"></select>');
						for(var i = 0; i < data.labels.length; i++)
						{
							var label = data.labels[i];
							$labelsel.append('<option value="'+ label.nodeid +'">'+ label.name +'</option>');
						}
						$labelsel.appendTo($labelsel_div);
						$printbtn.CswButton('enable');
					} else {
						$printbtn.hide();
						$labelsel_div.append('<span>No labels have been assigned!</span>');
					}
				} // success
			}); // ajax

			var $printbtn = $div.CswButton({
												ID: 'print_label_print', 
												enabledText: 'Print', 
												//disabledText: 'Printing...', 
												disableOnClick: false,
												onclick: function() {
													var jData2 = { PropId: o.propid, PrintLabelNodeId: $labelsel.val() };
													CswAjaxJson({
														url: o.GetEPLTextUrl,
														data: jData2,
														success: function(data) {
															var labelx = $('#labelx').get(0);
															labelx.EPLScript = data.epl;
															labelx.Print();
														} // success
													}); // ajax
												} // onclick
											}); // CswButton
			$printbtn.CswButton('disable');

			var $closebtn = $div.CswButton({ID: 'print_label_close', 
												enabledText: 'Close', 
												disabledText: 'Closing...', 
												onclick: function() {
														$div.dialog('close');
													}
												});

			var $hiddendiv = $('<div style="display: none; border: 1px solid red;"></div>')
								.appendTo($div);
			var $labelx = $("<OBJECT ID='labelx' Name='labelx' classid='clsid:A8926827-7F19-48A1-A086-B1A5901DB7F0' codebase='CafLabelPrintUtil.cab#version=0,1,6,0' width=500 height=300 align=center hspace=0 vspace=0></OBJECT>")
								.appendTo($hiddendiv);

			_openDiv($div, 400, 300);
		}, // PrintLabelDialog


		// Generic

//		'OpenPopup': function(url) { 
//							var popup = window.open(url, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
//							popup.focus();
//							return popup;
//						},
		OpenDialog: function (id, url) {
			var $dialogdiv = $('<div id="' + id + '"></div>');
			$dialogdiv.load(url,
							function (responseText, textStatus, XMLHttpRequest) {
								_openDiv($dialogdiv, 600, 400);
							});
		},
		CloseDialog: function (id) {
			$('#' + id)
				.dialog('close')
				.remove();
		}
	};


	function _openDiv($div, width, height)
	{
		$('<div id="DialogErrorDiv" style="display: none;"></div>')
			.prependTo($div);

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
		  $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
		}    
  
	};
})(jQuery);
