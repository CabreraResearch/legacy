/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

;  (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswViewEditor = function (options) {
		var o = {
			ViewGridUrl: '/NbtWebApp/wsNBT.asmx/getViewGrid',
			ViewInfoUrl: '/NbtWebApp/wsNBT.asmx/getViewInfo',
			SaveViewUrl: '/NbtWebApp/wsNBT.asmx/saveViewInfo',
			CopyViewUrl: '/NbtWebApp/wsNBT.asmx/copyView',
			DeleteViewUrl: '/NbtWebApp/wsNBT.asmx/deleteView',
			ChildOptionsUrl: '/NbtWebApp/wsNBT.asmx/getViewChildOptions',
			PropNamesUrl: '/NbtWebApp/wsNBT.asmx/getPropNames',
			viewid: '',
			viewname: '',
			viewmode: '',
			ID: 'vieweditor',
			ColumnViewName: 'VIEWNAME', 
			ColumnViewId: 'NODEVIEWID',
			ColumnFullViewId: 'VIEWID',
			ColumnViewMode: 'VIEWMODE',
			onCancel: null, // function($wizard) {},
			onFinish: null, // function(viewid, viewmode) {},
			startingStep: 1
		};
		if(options) $.extend(o, options);

		var WizardStepArray = [ CswViewEditor_WizardSteps.step1, CswViewEditor_WizardSteps.step2, CswViewEditor_WizardSteps.step3, 
								CswViewEditor_WizardSteps.step4, CswViewEditor_WizardSteps.step5, CswViewEditor_WizardSteps.step6];
		var WizardSteps = {};                
		for( var i = 1; i <= WizardStepArray.length; i++ )
		{                
			WizardSteps[i] = WizardStepArray[i-1].description;
		}

		var CurrentStep = o.startingStep;

		var $parent = $(this);
		var $div = $('<div></div>')
					.appendTo($parent);

		var $wizard = $div.CswWizard('init', { 
				ID: o.ID + '_wizard',
				Title: 'Edit View',
				StepCount: WizardStepArray.length,
				Steps: WizardSteps,
				StartingStep: o.startingStep,
				FinishText: 'Save and Finish',
				onNext: _handleNext,
				onPrevious: _handlePrevious,
				onBeforePrevious: _onBeforePrevious,
				onCancel: o.onCancel,
				onFinish: _handleFinish
			});

		// don't activate Save and Finish until step 2
		if(o.startingStep === 1)
			$wizard.CswWizard('button', 'finish', 'disable');

		// Step 1 - Choose a View
		var $div1 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.step1.step);
		var instructions = "A <em>View</em> controls the arrangement of information you see in a tree or grid.  "+
							"Views are useful for defining a user's workflow or for creating elaborate search criteria. "+
							"This wizard will take you step by step through the process of creating a new View or "+
							"editing an existing View.<br/><br/>";
		$div1.append(instructions);
		$div1.append('Select a View to Edit:&nbsp;');
		var $selview_span = $('<span id="'+ o.ID +'_selviewname" style="font-weight: bold"></span>')
								.appendTo($div1);
		var $viewgrid_div = $('<div></div>').appendTo($div1);
		var $viewgrid;
		function onViewGridSuccess($vg) { 
			$viewgrid = $vg; 
		}

		_getViewsGrid(onViewGridSuccess, o.viewid);

		var $div1_btntbl = $div1.CswTable({ ID: o.ID + '_1_btntbl', width: '100%' });
		var $div1_btntbl_cell11 = $div1_btntbl.CswTable('cell', 1, 1)
		var $div1_btntbl_cell12 = $div1_btntbl.CswTable('cell', 1, 2)
		$div1_btntbl_cell12.CswAttrDom('align', 'right');
		var $allcheck_div = $('<div></div>').appendTo($div1_btntbl_cell12);

		IsAdministrator({
			'Yes': function() {
				var $showOther = $allcheck_div.CswInput('init',{ID: o.ID + '_all',
																type: CswInput_Types.checkbox,
																onChange: function() { 
																				_getViewsGrid(onViewGridSuccess); 
																			}
																});
				$allcheck_div.append('Show Other Roles/Users');
			}
		});

		var $copyviewbtn = $div1_btntbl_cell11.CswButton({
			'ID': o.ID + '_copyview',
			'enabledText': 'Copy View',
			'disableOnClick': true,
			'onclick': function() {
				var viewid = _getSelectedViewId($viewgrid);
				if(!isNullOrEmpty(viewid))
				{
					var dataJson = {
						ViewId: viewid
					};

					CswAjaxJson({
						url: o.CopyViewUrl,
						data: dataJson,
						success: function (gridJson) {
							_getViewsGrid(onViewGridSuccess, gridJson.copyviewid); 
						},
						error: function() {
							$copyviewbtn.CswButton('enable');
						}
					});
				} // if(viewid !== '' && viewid !== undefined)
			} // onclick
		}); // copy button
		$copyviewbtn.CswButton('disable');

		var $deleteviewbtn = $div1_btntbl_cell11.CswButton({
			ID: o.ID + '_deleteview',
			enabledText: 'Delete View',
			disableOnClick: true,
			onclick: function() {
				var viewid = _getSelectedViewId($viewgrid);
				if( !isNullOrEmpty( viewid ) )
				{
					if(confirm("Are you sure you want to delete: " + _getSelectedViewName($viewgrid)))
					{
						var dataJson = {
							ViewId: viewid
						};

						CswAjaxJson({
							url: o.DeleteViewUrl,
							data: dataJson,
							success: function () {
								_getViewsGrid(onViewGridSuccess); 
								$copyviewbtn.CswButton('disable');
							},
							error: function() {
								$deleteviewbtn.CswButton('enable');
							}
						});
					}
				}
			} // onclick
		}); // delete button
		$deleteviewbtn.CswButton('disable');

		var $newviewbtn = $div1_btntbl_cell11.CswButton({
			'ID': o.ID + '_newview',
			'enabledText': 'Create New View',
			'disableOnClick': false,
			'onclick': function() {
				$.CswDialog('AddViewDialog', { 
					'onAddView': function(newviewid) {
						$viewgrid = _getViewsGrid(onViewGridSuccess, newviewid); 
					},
					'onClose': function() {
						$newviewbtn.CswButton('enable');
					}
				}); // CswDialog
			} // onclick
		})

		//$wizard.CswWizard('button', 'next', 'disable');

		// Step 2 - Edit View Attributes
		var $div2 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.step2.step);
		var $table2 = $div2.CswTable({ 
				'ID': o.ID + '_tbl2', 
				'FirstCellRightAlign': true 
		});

		$table2.CswTable('cell', 1, 1).append('View Name:');
		var $viewnametextcell = $table2.CswTable('cell', 1, 2);
		var $viewnametextbox = $viewnametextcell.CswInput('init',{ID: o.ID + '_viewname',
																	type: CswInput_Types.text
																	});

		$table2.CswTable('cell', 2, 1).append('Category:');
		var $categorytextcell = $table2.CswTable('cell', 2, 2);
		var $categorytextbox = $categorytextcell.CswInput('init',{ID: o.ID + '_category',
																	type: CswInput_Types.text
																});

		var v;
		// we don't have xml to see whether this is a Property view or not yet,
		// so checking startingStep will have to suffice
		if(o.startingStep === 1)
		{
			v = makeViewVisibilitySelect($table2, 3, 'View Visibility:');
		}

		$table2.CswTable('cell', 4, 1).append('For Mobile:');
		var $formobilecheckcell = $table2.CswTable('cell', 4, 2);
		var $formobilecheckbox = $formobilecheckcell.CswInput('init',{ID: o.ID + '_formobile',
																		type: CswInput_Types.checkbox
																});

		$table2.CswTable('cell', 5, 1).append('Display Mode:');
		var $displaymodespan = $table2.CswTable('cell', 5, 2).append('<span id="'+ o.ID +'_displaymode"></span>');
		
		var $gridwidthlabelcell = $table2.CswTable('cell', 6, 1)
								.append('Grid Width (in characters):');
		var $gridwidthtextboxcell = $table2.CswTable('cell', 6, 2);
		$gridwidthtextboxcell.CswNumberTextBox('init', {
				'ID': o.ID + '_gridwidth',
				'Value': '',
				'MinValue': '1',
				'MaxValue': '',
				'Precision': '0',
				'onchange': function() { }
		});
		
		// Step 3 - Add Relationships
		var $div3 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.step3.step);
		$div3.append('Add relationships from the select boxes below:<br/><br/>');
		var $treediv3 = $('<div />').appendTo($div3);
		
		// Step 4 - Select Properties
		var $div4 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.step4.step);
		$div4.append('Add properties from the select boxes below:<br/><br/>');
		var $treediv4 = $('<div />').appendTo($div4);
		
		// Step 5 - Set Filters
		var $div5 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.step5.step);
		$div5.append('Add filters by selecting properties from the tree:<br/><br/>');
		var $treediv5 = $('<div />').appendTo($div5);

		// Step 6 - Fine Tuning
		var $div6 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.step6.step);
		$div6.append('Select what you want to edit from the tree:<br/><br/>');
		var $table6 = $div6.CswTable({ 'ID': o.ID + '_6_tbl' });
	   
		var currentViewJson;

		function _onBeforePrevious($wizard, stepno)
		{
			return (stepno !== CswViewEditor_WizardSteps.step2.step || confirm("You will lose any changes made to the current view if you continue.  Are you sure?") );
		}

		function _handleNext($wizard, newstepno)
		{
			CurrentStep = newstepno;
			switch(newstepno)
			{
				case CswViewEditor_WizardSteps.step1.step:
					break;
				case CswViewEditor_WizardSteps.step2.step:
					$wizard.CswWizard('button', 'finish', 'enable');
					$wizard.CswWizard('button', 'next', 'disable');

					var jsonData = {
						ViewId: _getSelectedViewId($viewgrid)
					};

					CswAjaxJson({
						url: o.ViewInfoUrl,
						data: jsonData,
						success: function(data) {
						    currentViewJson = data.TreeView;

						    $viewnametextbox.val(currentViewJson.viewname);
							$categorytextbox.val(currentViewJson.category);
						    var visibility = tryParseString(currentViewJson.visibility);
						    if (visibility !== 'Property') {
								if(v.getvisibilityselect() !== undefined) {
									v.getvisibilityselect().val(visibility).trigger('change');
									v.getvisroleselect().val('nodes_' + currentViewJson.visibilityroleid);
									v.getvisuserselect().val('nodes_' + currentViewJson.visibilityuserid);
								}
							}

							if (isTrue( currentViewJson.formobile)) {
								$formobilecheckbox.CswAttrDom('checked','checked');
							}
						    var mode = currentViewJson.mode;
							$displaymodespan.text(mode);
							$gridwidthtextboxcell.CswNumberTextBox('setValue', o.ID + '_gridwidth', currentViewJson.width);
							if(mode === "Grid") {
								$gridwidthlabelcell.show();
								$gridwidthtextboxcell.show();
							} else {
								$gridwidthlabelcell.hide();
								$gridwidthtextboxcell.hide();
							}

							$wizard.CswWizard('button', 'next', 'enable');
						} // success
					}); // ajax
					break;
				case CswViewEditor_WizardSteps.step3.step:
					// save step 2 content to $currentviewxml
					if(currentViewJson !== undefined)
					{
						cacheStepTwo();
					} // if($currentviewxml !== undefined)

					// make step 3 tree
					_makeViewTree(CswViewEditor_WizardSteps.step3.step, $treediv3);
					break;
				case CswViewEditor_WizardSteps.step4.step:
					_makeViewTree(CswViewEditor_WizardSteps.step4.step, $treediv4);
					break;
				case CswViewEditor_WizardSteps.step5.step:
					_makeViewTree(CswViewEditor_WizardSteps.step5.step, $treediv5);
					break;
				case CswViewEditor_WizardSteps.step6.step:
					_makeViewTree(CswViewEditor_WizardSteps.step6.step, $table6.CswTable('cell', 1, 1));
					break;
			} // switch(newstepno)
		} // _handleNext()

		function cacheStepTwo()
		{
			currentViewJson.viewname = $viewnametextbox.val();
			currentViewJson.category = $categorytextbox.val();
			if (currentViewJson.visibility !== 'Property') {
				if (v.getvisibilityselect() !== undefined) {
					var visibility = v.getvisibilityselect().val();
					currentViewJson.visibility = visibility;

					var rolenodeid = '';
					if (visibility === 'Role') {
						rolenodeid = v.getvisroleselect().val();
						if(!isNullOrEmpty(rolenodeid)) {
						    rolenodeid = rolenodeid.substr('nodes_'.length);
						}
					}
					currentViewJson.visibilityroleid = rolenodeid;
					
					var usernodeid = '';
					if (visibility === 'User') {
						usernodeid = v.getvisuserselect().val();
						if (!isNullOrEmpty(usernodeid)) {
						    usernodeid = usernodeid.substr('nodes_'.length);
						}
					}
					currentViewJson.visibilityuserid = usernodeid;
				}
			}
			var formobile = ($formobilecheckbox.is(':checked') ? 'true' : 'false');
			currentViewJson.formobile = formobile;
			currentViewJson.width = $gridwidthtextboxcell.CswNumberTextBox('value', o.ID + '_gridwidth');
		}

		function _handlePrevious($wizard, newstepno)
		{
			if(newstepno === 1)
				$wizard.CswWizard('button', 'finish', 'disable');
			
			CurrentStep = newstepno;
			switch(newstepno)
			{
				case CswViewEditor_WizardSteps.step1.step: 
					break;
				case CswViewEditor_WizardSteps.step2.step: 
					break;
				case CswViewEditor_WizardSteps.step3.step:
					_makeViewTree(CswViewEditor_WizardSteps.step3.step, $treediv3);
					break;
				case CswViewEditor_WizardSteps.step4.step:
					_makeViewTree(CswViewEditor_WizardSteps.step4.step, $treediv4);
					break;
				case CswViewEditor_WizardSteps.step5.step:
					_makeViewTree(CswViewEditor_WizardSteps.step5.step, $treediv5);
					break;
				case CswViewEditor_WizardSteps.step6.step:
					_makeViewTree(CswViewEditor_WizardSteps.step6.step, $table6.CswTable('cell', 1, 1));
					break;
			}
		}


		function _handleFinish($wizard)
		{
			var viewid = _getSelectedViewId($viewgrid);
			var processView = true; 

			if (!isNullOrEmpty(currentViewJson)) {
				if (CurrentStep === CswViewEditor_WizardSteps.step2.step) {
					cacheStepTwo();
				}
				if (currentViewJson.mode === 'Grid' &&
					(currentViewJson.children('relationship').length === 0 ||
					  currentViewJson.children('relationship').children('property').length === 0 ) )
				{
					processView = confirm('You are attempting to create a Grid without properties. This will not display any information. Do you want to continue?');
					if(!processView) $wizard.CswWizard('button', 'finish', 'enable');
				}
			}

			if(processView)
			{
				var jsonData = {
					ViewId: viewid,
					ViewJson: JSON.stringify(currentViewJson)
				};

				CswAjaxJson({
					url: o.SaveViewUrl,
					data: jsonData,
					success: function () {
						o.onFinish(viewid, _getSelectedViewMode($viewgrid));
					} // success
				});
			} // ajax
		} //_handleFinish

		function _getViewsGrid(onSuccess, selectedviewid)
		{
			var all = false;
			if($('#'+ o.ID + '_all:checked').length > 0)
				all = true;

			$selview_span.text('');
			if(o.startingStep === 1)
				$wizard.CswWizard('button', 'next', 'disable');
			
			// passing selectedviewid in allows us to translate SessionViewIds to ViewIds
			var dataJson = {
				All: all,
				SelectedViewId: tryParseString(selectedviewid,'')
			};

			CswAjaxJson({
				url: o.ViewGridUrl,
				data: dataJson,
				success: function (gridJson) {

					$viewgrid_div.empty();
					var $gridPager = $('<div id="' + o.ID + '_gp" style="width:100%; height:20px;" />')
									 .appendTo($viewgrid_div);
					var $viewgrid = $('<table id="'+ o.ID + '_gt" />')
										.appendTo($viewgrid_div);

					var mygridopts = {
						'autowidth': true,
						'height': 180,
						'onSelectRow': function(id, selected) {
							if(selected) 
							{
								$copyviewbtn.CswButton('enable');
								$deleteviewbtn.CswButton('enable');
								$selview_span.text(_getSelectedViewName($viewgrid));
								$wizard.CswWizard('button', 'next', 'enable');
							}
							else 
							{
								$copyviewbtn.CswButton('disable');
								$deleteviewbtn.CswButton('disable');
								$selview_span.text("");
								$wizard.CswWizard('button', 'next', 'disable');
							}
						},
						'pager': $gridPager
					};
					$.extend(gridJson, mygridopts);

					$viewgrid.jqGrid(gridJson)
							.hideCol(o.ColumnFullViewId);

					if(!isNullOrEmpty( gridJson.selectedpk ))
					{
						$viewgrid.setSelection(_getRowForPk($viewgrid, gridJson.selectedpk));
						$viewgrid.CswNodeGrid('scrollToSelectedRow');
					}
					onSuccess($viewgrid);
				} // success
			}); // ajax
		} // _getViewsGrid()
		
		function _getSelectedViewId($viewgrid)
		{
			var ret = '';
			if(o.startingStep === 1) {
				ret = _getSelectedRowValue($viewgrid, o.ColumnFullViewId);
			} else {
				ret = o.viewid;
			}
			return ret;
		}
		
		function _getSelectedViewMode($viewgrid)
		{
			var ret = '';
			if(o.startingStep === 1) {
				ret = _getSelectedRowValue($viewgrid, o.ColumnViewMode);
			} else {
				ret = o.viewmode;
			}
			return ret;
		}

		function _getSelectedViewName($viewgrid)
		{
			var ret = '';
			if(o.startingStep === 1) {
				ret = _getSelectedRowValue($viewgrid, o.ColumnViewName);
			} else {
				ret = o.viewname;
			}
			return ret;
		}

		function _getSelectedRowValue($viewgrid, columnname)
		{
			var rowid = $viewgrid.jqGrid('getGridParam', 'selrow');
			var ret = $viewgrid.jqGrid('getCell', rowid, columnname);
			return ret;
		}

		function _getRowForPk($viewgrid, selectedpk)
		{
			var pks = $viewgrid.jqGrid('getCol', o.ColumnViewId, true);
			var rowid = 0;
			for(var i in pks)
			{
				if(pks[i].value.toString() === selectedpk.toString())
					rowid = pks[i].id;
			}
			return rowid;
		}

		
		function _makeViewTree(stepno, $div)
		{
			var treecontent = viewJsonToHtml(stepno, currentViewJson);
			$div.jstree({
						"html_data":
							{
								"data": treecontent.htmlstring
							},
						"ui": {
							"select_limit": 1 //,
							//"initially_select": selectid,
						},
						"types": {
							"types": treecontent.types,
							"max_children": -2,
							"max_depth": -2
						},
						"plugins": ["themes", "html_data", "ui", "types", "crrm"]
			}); // tree

			// tree events

			$div.find('.vieweditor_childselect').change(function() {
				var $select = $(this);
				var childJson = $select.find('option:selected').data('thisViewJson');
				if($select.CswAttrDom('arbid') === "root")
				{
				    $.extend(currentViewJson.childrelationships, childJson);
				    //currentViewJson.append($(childJson));
				} else {
				    var parentObj = findObject(currentViewJson, 'arbitraryid', $select.CswAttrDom('arbid'));
				    $.extend(parentObj.childrelationships, childJson);
                    //var $parent = currentViewJson.find('[arbitraryid="' + $select.CswAttrDom('arbid') +'"]');
					//parentObj.append($(childJson));
				}
				_makeViewTree(stepno, $div);
			}); // child select

			$div.find('.vieweditor_deletespan').each(function() {
				var $td = $(this); 
				$td.CswImageButton({
					ButtonType: CswImageButton_ButtonType.Delete,
					AlternateText: 'Delete',
					ID: $td.CswAttrDom('arbid') + '_delete',
					onClick: function ($ImageDiv) { 
						var $span = $ImageDiv.parent();
					    deleteObject(currentViewJson, 'arbitraryid', $span.CswAttrDom('arbid'));
					    //currentViewJson.find('[arbitraryid="' + $span.CswAttrDom('arbid') +'"]').remove();
						_makeViewTree(stepno, $div);
						return CswImageButton_ButtonType.None; 
					}
				});
			}); // delete

			if(stepno === CswViewEditor_WizardSteps.step5.step)
			{
				$div.find('.vieweditor_addfilter').each(function() {
					var $span = $(this);
					var $tbl = $span.CswTable({ 'ID': o.ID + '_propfilttbl' });
					$tbl.css('display', 'inline-table');
					$tbl.CswViewPropFilter('init', {
														viewJson: currentViewJson,
														proparbitraryid: $span.CswAttrDom('proparbid'),
														filtarbitraryid: '',
														viewbuilderpropid: '',
														ID: o.ID,
														propRow: 1,
														firstColumn: 1,
														includePropertyName: false,
														selectedSubfieldVal: '',
														selectedFilterVal: '',
														autoFocusInput: false
												});
					
					$tbl.CswTable('cell', 1, 5).CswButton('init', {
						'ID':	'addfiltbtn',
						'prefix': o.ID,
						'enabledText': 'Add',
						'disabledText': 'Adding',
						'onclick': function () {

						    var arbProp = findObject(currentViewJson, 'arbitraryid', $span.CswAttrDom('proparbid'));
						    if (arbProp.hasOwnProperty('fieldtype')) {
						        var fieldType = arbProp.fieldtype;
						    }
							var Json = $tbl.CswViewPropFilter('getFilterJson', { 
											ID: o.ID,
											$parent: $span,
											fieldtype: fieldType, //currentViewJson.find('[arbitraryid="' + $span.CswAttrDom('proparbid') +'"]').CswAttrXml('fieldtype'),
											proparbitraryid: $span.CswAttrDom('proparbid'),
											allowNullFilterValue: true
										});

							var filterxml = $tbl.CswViewPropFilter('makeFilter', { 
								viewJson: currentViewJson, 
								filtJson: Json, 
								onSuccess: function(filterJson) {
								    var propObj = findObject(currentViewJson, 'arbitraryid', $span.CswAttrDom('proparbid'));
								    propObj.filters[filterJson.arbitraryid] = filterJson;
								    //var $propxml = currentViewJson.find('[arbitraryid="' + $span.CswAttrDom('proparbid') +'"]');
									//$(xmlToString($filterxml)).appendTo($propxml);
									_makeViewTree(stepno, $div);
								} // onSuccess
							}); // CswViewPropFilter
						} // onClick
					}); // CswButton
				}); // property click
			} // if(stepno === 5)

			if(stepno === CswViewEditor_WizardSteps.step6.step)
			{
				var $cell = $table6.CswTable('cell', 1, 2);
				var viewmode = _getSelectedViewMode($viewgrid);

				// Root
				$div.find('.vieweditor_viewrootlink').click(function() {
					$cell.empty();
				});

				// Relationship
				$div.find('.vieweditor_viewrellink').click(function() {
					$a = $(this);
					$cell.empty();
					//$cell.append('For ' + $a.text());

				    var viewnodejson = findObject(currentViewJson, 'arbitraryid', $a.CswAttrDom('arbid'));  
					//var viewnodejson = currentViewJson.find('[arbitraryid="'+ $a.CswAttrDom('arbid') +'"]')

					var $table = $cell.CswTable({ 'ID': o.ID + '_editrel', 'FirstCellRightAlign': true });
					$table.CswTable('cell', 1, 1).append('Allow Deleting');
					var $allowdeletingcell = $table.CswTable('cell', 1, 2);
					var $allowdeletingcheck = $allowdeletingcell.CswInput('init',{ID: o.ID + '_adcb',
																  type: CswInput_Types.checkbox
																});

					if(viewnodejson.allowdelete.toLowerCase() == 'true') {
						$allowdeletingcheck.CswAttrDom('checked', 'true');
					}

					$table.CswTable('cell', 2, 1).append('Group By');
					var $groupbyselect = $('<select id="' + o.ID + '_gbs" />')
												.appendTo($table.CswTable('cell', 2, 2));
				    var jsonData = {
				        Type: viewnodejson.secondtype,
				        Id: viewnodejson.secondid
				    };
					
					CswAjaxJson({
						url: o.PropNamesUrl,
						data: jsonData,
						success: function(data) {
							$groupbyselect.empty();
							$('<option value="">[None]</option>')
								.appendTo($groupbyselect);
							for (var propName in data) {
							    if (data.hasOwnProperty(propName)) {
							        var thisProp = data[propName];
							        var $option = $('<option value="' + thisProp.propid + '">' + propName + '</option>')
    							        .appendTo($groupbyselect)
    							        .data('thisPropData', { propName: thisProp});
							        if (viewnodejson.groupbypropid === thisProp.propid &&
    							        viewnodejson.groupbyproptype === thisProp.proptype &&
        							        viewnodejson.groupbypropname === thisProp.propname)
							        {
							            $option.CswAttrDom('selected', 'true');
							        }
							    }
							} // each
						} // success
					}); // ajax

					var $showtreecheck;
					if(viewmode === "Tree")
					{
						$table.CswTable('cell', 3, 1).append('Show In Tree');
						var $showtreecheckcell = $table.CswTable('cell', 3, 2);
						$showtreecheck = $showtreecheckcell.CswInput('init',{ID: o.ID + '_stcb',
																  type: CswInput_Types.checkbox
																}); 
						if(viewnodejson.CswAttrXml('showintree').toLowerCase() == 'true') {
							$showtreecheck.CswAttrDom('checked', 'true');
						}
					}

					$table.CswTable('cell', 4, 2).CswButton({ 
						'ID': o.ID + '_saverel',
						'enabledText': 'Apply',
						'disableOnClick': false,
						'onclick': function() {
							if($showtreecheck !== undefined)
								viewnodejson.CswAttrXml('showintree', ($showtreecheck.is(':checked')))
							viewnodejson.CswAttrXml('allowdelete', ($allowdeletingcheck.is(':checked')))
							if($groupbyselect.val() !== '') {
								var $propxml = $groupbyselect.find(':selected').data('thisPropData');
								viewnodejson.CswAttrXml('groupbypropid', $propxml.CswAttrXml('propid'));
								viewnodejson.CswAttrXml('groupbyproptype', $propxml.CswAttrXml('proptype'));
								viewnodejson.CswAttrXml('groupbypropname', $propxml.CswAttrXml('propname'));
							} else {
								viewnodejson.CswAttrXml('groupbypropid', '');
								viewnodejson.CswAttrXml('groupbyproptype', '');
								viewnodejson.CswAttrXml('groupbypropname', '');
							}
						} // onClick
					}); // CswButton
				}); // $div.find('.vieweditor_viewrellink').click(function() {

				// Property
				$div.find('.vieweditor_viewproplink').click(function() {
					$a = $(this);
					$cell.empty();

					if(viewmode === "Grid")
					{
						var $viewnodexml = currentViewJson.find('[arbitraryid="'+ $a.CswAttrDom('arbid') +'"]')

						//$cell.append('For ' + $a.text());
						var $table = $cell.CswTable({ 'ID': o.ID + '_editprop', 'FirstCellRightAlign': true });

						$table.CswTable('cell', 1, 1).append('Sort By');
						var $sortbycheckcell = $table.CswTable('cell', 1, 2);
						var $sortbycheck = $sortbycheckcell.CswInput('init',{ID: o.ID + '_sortcb',
																  type: CswInput_Types.checkbox
																}); 
						if($viewnodexml.CswAttrXml('sortby').toLowerCase() == 'true') {
							$sortbycheck.CswAttrDom('checked', 'true');
						}

						$table.CswTable('cell', 2, 1).append('Grid Column Order');
						var $colordertextcell = $table.CswTable('cell', 2, 2);
						var $colordertextbox = $colordertextcell.CswInput('init',{ID: o.ID + '_gcotb',
																  type: CswInput_Types.text
																}); 
						$colordertextbox.val($viewnodexml.CswAttrXml('order'));

						$table.CswTable('cell', 3, 1).append('Grid Column Width (in characters)');
						var $colwidthtextcell = $table.CswTable('cell', 3, 2);
						var $colwidthtextbox = $colwidthtextcell.CswInput('init',{ID: o.ID + '_gcwtb',
																  type: CswInput_Types.text
																});
						$colwidthtextbox.val($viewnodexml.CswAttrXml('width'));

						$table.CswTable('cell', 4, 2).CswButton({ 
							'ID': o.ID + '_saveprop',
							'enabledText': 'Apply',
							'disableOnClick': false,
							'onclick': function() {
								$viewnodexml.CswAttrXml('sortby', ($sortbycheck.is(':checked')))
								$viewnodexml.CswAttrXml('order', $colordertextbox.val());
								$viewnodexml.CswAttrXml('width', $colwidthtextbox.val());
							} // onClick
						}); // CswButton
					}
				});

				// Filter
				$div.find('.vieweditor_viewfilterlink').click(function() {
					$a = $(this);
					$cell.empty();
					//$cell.append('For ' + $a.text());

					var $viewnodexml = currentViewJson.find('[arbitraryid="'+ $a.CswAttrDom('arbid') +'"]')

					var $table = $cell.CswTable({ 'ID': o.ID + '_editfilt', 'FirstCellRightAlign': true });
					$table.CswTable('cell', 1, 1).append('Case Sensitive');
					var $casecheck = $('<input type="checkbox" id="' + o.ID + '_casecb" />')
											.appendTo($table.CswTable('cell', 1, 2));
					if($viewnodexml.CswAttrXml('casesensitive').toLowerCase() === 'true') {
						$casecheck.CswAttrDom('checked', 'true');
					}

					$table.CswTable('cell', 4, 2).CswButton({ 
						'ID': o.ID + '_savefilt',
						'enabledText': 'Apply',
						'disableOnClick': false,
						'onclick': function() {
							$viewnodexml.CswAttrXml('casesensitive', ($casecheck.is(':checked')))
						} // onClick
					}); // CswButton
				});
			} // if(stepno === 6)
		} // _makeViewTree()

		function viewJsonToHtml(stepno, itemJson)
		{
			var types = {};
			var arbid = itemJson.arbitraryid;
			var nodename = itemJson.nodename;
			var name = '';
			var rel = '';
			var skipme = false;
			var skipchildoptions = true;
			var linkclass = '';
		    var children = '';

			switch (nodename) {
		        case 'treeview':
				    if(stepno === CswViewEditor_WizardSteps.step3.step) skipchildoptions = false;
		            children = 'childrelationships';
				    arbid = "root";
				    name = itemJson.viewname;
				    rel = "root";
				    types.root = { icon: { image: tryParseString(itemJson.iconfilename) } };
				    linkclass = 'vieweditor_viewrootlink';
			        break;
			    case 'relationship':
			        if(stepno === CswViewEditor_WizardSteps.step3.step) skipchildoptions = false;
				    if(stepno === CswViewEditor_WizardSteps.step4.step) skipchildoptions = false;
			        children = 'properties';
				    name = itemJson.secondname;
				    var propname = tryParseString(itemJson.propname);
				    if (!isNullOrEmpty(propname)) {
					    if( itemJson.propowner === "First" ) {
					        name += " (by " + itemJson.firstname + "'s " + propname + ")";
					    } else {
					        name += " (by " + propname + ")";
					    }
				    }
				    rel = tryParseString(itemJson.secondtype) + '_' + tryParseString(itemJson.secondid);
				    types[rel] = { icon: { image: tryParseString(itemJson.secondiconfilename) } };
				    linkclass = 'vieweditor_viewrellink';
			        break;
			    case 'property':
				    if(stepno <= CswViewEditor_WizardSteps.step3.step) skipme = true;
				    if(stepno === CswViewEditor_WizardSteps.step5.step) skipchildoptions = false;
			        children = 'filters';
				    name = itemJson.name;
				    rel = "property";
				    types.property = { icon: { image: "Images/view/property.gif" } };
				    linkclass = "vieweditor_viewproplink";
			        break;
			    case 'filter':
				    if(stepno <= CswViewEditor_WizardSteps.step4.step) skipme = true;

				    name = itemJson.subfieldname + ' ' + itemJson.filtermode + ' ' + itemJson.value;
				    rel = "filter";
				    types.filter = { icon: { image: "Images/view/filter.gif" } };
				    linkclass = 'vieweditor_viewfilterlink';
			        break;
			}
			
			var treestr = '';
			if(!skipme)
			{
				treestr = '<li id="'+ arbid +'" ';
				treestr += '    rel="'+ rel +'" ';
				treestr += '    class="jstree-open" ';
				treestr += '>';
				treestr += ' <a href="#" class="' + linkclass + '" arbid="'+ arbid +'">'+ name +'</a>';
				if (arbid !== "root") {
					treestr += ' <span style="" class="vieweditor_deletespan" arbid="'+ arbid +'"></span>';
				}

				treestr += '<ul>';
				if (!isNullOrEmpty(children)) {
				    var childJson = itemJson[children];
				    for (var child in childJson) {
				        if (childJson.hasOwnProperty(child)) {
                            var childcontent = viewJsonToHtml(stepno, childJson[child]);
				            treestr += childcontent.htmlstring;
				            $.extend(types, childcontent.types);				            
				        }
				    }
				}
			    if(!skipchildoptions) 
				{
					if(stepno === CswViewEditor_WizardSteps.step5.step)
					{ 
						// view filters
						treestr += '<li><span class="vieweditor_addfilter" proparbid="' + arbid + '"></span></li>';
					}
					else 
					{
						// relationships or properties
						treestr += '<li><select id="' + stepno + '_' + arbid + '_child" arbid="' + arbid + '" class="vieweditor_childselect"></select></li>';
						
						var dataJson = {
							StepNo: stepno,
							ArbitraryId: arbid,
							ViewJson: JSON.stringify(currentViewJson)
						};

						CswAjaxJson({
							url: o.ChildOptionsUrl,
							data: dataJson,
							success: function(data) 
							{
								var $select = $('#' + stepno + '_' + arbid + '_child');
								$select.empty();
								$select.append('<option value="">Select...</option>');
								for (var optionName in data) {
								    if (data.hasOwnProperty(optionName)) {
								        var thisOpt = data[optionName];
								        var $option = $('<option value="' + thisOpt.arbitraryid + '">' + optionName + '</option>')
    								        .appendTo($select);
								        $option.data('thisViewJson', {optionName: thisOpt});
								    }
								}
							} // success
						}); // ajax

					} // if-else(stepno === 5)
				} // if(!skipchildoptions) 

				treestr += '</ul>';
				treestr += '</li>';
			}

			return {
						'htmlstring': treestr,
						'types': types
					};
		} // _viewXmlToHtml()


		return $div;

	} // $.fn.CswViewEditor
}) (jQuery);

