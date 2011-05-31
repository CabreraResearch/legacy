/// <reference path="../jquery/jquery-1.6.1-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

var CswViewEditor_WizardSteps = {
    'step1': { step: 1, description: 'Choose a View' },
    'step2': { step: 2, description: 'Edit View Attributes' },
    'step3': { step: 3, description: 'Add Relationships' },
    'step4': { step: 4, description: 'Select Properties' },
    'step5': { step: 5, description: 'Set Filters' },
    'step6': { step: 6, description: 'Fine Tuning' }
};

;  (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswViewEditor = function (options) 
	{
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
			onCancel: function($wizard) {},
			onFinish: function(viewid, viewmode) {},
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
				'ID': o.ID + '_wizard',
				'Title': 'Edit View',
				'StepCount': WizardStepArray.length,
				'Steps': WizardSteps,
				'StartingStep': o.startingStep,
				'FinishText': 'Save and Finish',
				'onNext': _handleNext,
				'onPrevious': _handlePrevious,
				'onBeforePrevious': _onBeforePrevious,
				'onCancel': o.onCancel,
				'onFinish': _handleFinish
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

					CswAjaxJSON({
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
			'ID': o.ID + '_deleteview',
			'enabledText': 'Delete View',
			'disableOnClick': true,
			'onclick': function() {
			    var viewid = _getSelectedViewId($viewgrid);
				if( !isNullOrEmpty( viewid ) )
				{
					if(confirm("Are you sure you want to delete: " + _getSelectedViewName($viewgrid)))
					{
						var dataJson = {
                            ViewId: viewid
                        };

                        CswAjaxJSON({
							url: o.DeleteViewUrl,
							data: dataJson,
							success: function (gridJson) {
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
					},
					'makeVisibilitySelect': _makeVisibilitySelect
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
			v = _makeVisibilitySelect($table2, 3, 'View Visibility:');
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
       
		var $currentviewxml;

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

                    var dataXml = {
                        ViewId: _getSelectedViewId($viewgrid)
                    };

                    CswAjaxXml({
						url: o.ViewInfoUrl,
						data: dataXml,
                        stringify: false,
						success: function($xml) {
							$currentviewxml = $xml;
							$viewnametextbox.val($currentviewxml.CswAttrXml('viewname'));
							$categorytextbox.val($currentviewxml.CswAttrXml('category'));
							if($currentviewxml.CswAttrXml('visibility') !== 'Property')
							{
								if(v.getvisibilityselect() !== undefined)
								{
									v.getvisibilityselect().val($currentviewxml.CswAttrXml('visibility')).trigger('change');
									v.getvisroleselect().val('nodes_' + $currentviewxml.CswAttrXml('visibilityroleid'));
									v.getvisuserselect().val('nodes_' + $currentviewxml.CswAttrXml('visibilityuserid'));
								}
							}

							if( isTrue( $currentviewxml.CswAttrXml('formobile') ) ) {
								$formobilecheckbox.CswAttrDom('checked','checked');
							}
							var mode = $currentviewxml.CswAttrXml('mode')
							$displaymodespan.text(mode);
							$gridwidthtextboxcell.CswNumberTextBox('setValue', $currentviewxml.CswAttrXml('width'));
							if(mode === "Grid") {
								$gridwidthlabelcell.show();
								$gridwidthtextboxcell.show();
							} else {
								$gridwidthlabelcell.hide();
								$gridwidthtextboxcell.hide();
							}
						} // success
					}); // ajax
					break;
				case CswViewEditor_WizardSteps.step3.step:
					// save step 2 content to $currentviewxml
					if($currentviewxml !== undefined)
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
            $currentviewxml.CswAttrXml('viewname', $viewnametextbox.val());
			$currentviewxml.CswAttrXml('category', $categorytextbox.val());
			if($currentviewxml.CswAttrXml('visibility') !== 'Property')
			{
				$currentviewxml.CswAttrXml('visibility', v.getvisibilityselect().val());
				
				// temporary workaround
				var rolenodeid = v.getvisroleselect().val();
				if(!isNullOrEmpty(rolenodeid))
				{
					rolenodeid = rolenodeid.substr('nodes_'.length)
				}
				var usernodeid = v.getvisuserselect().val();
				if(!isNullOrEmpty(usernodeid))
				{
					usernodeid = usernodeid.substr('nodes_'.length)
				}
				$currentviewxml.CswAttrXml('visibilityroleid', rolenodeid);
				$currentviewxml.CswAttrXml('visibilityuserid', usernodeid);
			}
            var formobile = ($formobilecheckbox.is(':checked') ? 'true' : 'false');
			$currentviewxml.CswAttrXml('formobile', formobile );
			$currentviewxml.CswAttrXml('width', $gridwidthtextboxcell.CswNumberTextBox('value'));
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

            if( CurrentStep === CswViewEditor_WizardSteps.step2.step &&
                !isNullOrEmpty( $currentviewxml ) )
            {
                cacheStepTwo();
            }

            var dataXml = {
                ViewId: viewid,
                ViewXml: xmlToString($currentviewxml)
            };

			CswAjaxXml({
				url: o.SaveViewUrl,
				data: dataXml,
                stringify: true,
				success: function ($xml) {
					o.onFinish(viewid, _getSelectedViewMode($viewgrid));
				} // success
			}); // ajax
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
				SelectedViewId: selectedviewid
            };

			CswAjaxJSON({
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
							}
							else 
							{
								$copyviewbtn.CswButton('disable');
								$deleteviewbtn.CswButton('disable');
							}
							$selview_span.text(_getSelectedViewName($viewgrid));
							$wizard.CswWizard('button', 'next', 'enable');
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

		function _makeVisibilitySelect($table, rownum, label)
		{
			var $visibilityselect;
			var $visroleselect;
			var $visuserselect;
			IsAdministrator({
				'Yes': function() {
						
						$table.CswTable('cell', rownum, 1).append(label);
						var $parent = $table.CswTable('cell', rownum, 2);
						var id = $table.CswAttrDom('id');

						$visibilityselect = $('<select id="' + id + '_vissel" />')
													.appendTo($parent);
						$visibilityselect.append('<option value="User">User:</option>');
						$visibilityselect.append('<option value="Role">Role:</option>');
						$visibilityselect.append('<option value="Global">Global</option>');

						$visroleselect = $parent.CswNodeSelect('init', {
																			'ID': id + '_visrolesel', 
																			'objectclass': 'RoleClass'
																		}).hide();
						$visuserselect = $parent.CswNodeSelect('init', {
																			'ID': id + '_visusersel', 
																			'objectclass': 'UserClass'
																		})

						$visibilityselect.change(function() {
							var val = $visibilityselect.val();
							if(val === 'Role')
							{
								$visroleselect.show();
								$visuserselect.hide();
							}
							else if(val === 'User')
							{
								$visroleselect.hide();
								$visuserselect.show();
							}
							else
							{
								$visroleselect.hide();
								$visuserselect.hide();
							}
						}); // change
					} // yes
			}); // IsAdministrator

			return {
				'getvisibilityselect': function() { return $visibilityselect; },
				'getvisroleselect': function() { return $visroleselect; },
				'getvisuserselect': function() { return $visuserselect; }
			}
		} // _makeVisibilitySelect()

		
		function _makeViewTree(stepno, $div)
		{
			var treecontent = _viewXmlToHtml(stepno, $currentviewxml);
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
				var childxml = $select.find('option:selected').data('optionviewxml');
				if($select.CswAttrDom('arbid') === "root")
				{
					$(childxml).appendTo($currentviewxml);
				} else {
					$(childxml).appendTo($currentviewxml.find('[arbitraryid="' + $select.CswAttrDom('arbid') +'"]'));
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
						$currentviewxml.find('[arbitraryid="' + $span.CswAttrDom('arbid') +'"]').remove();
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
														'viewxml': xmlToString($currentviewxml),
														'proparbitraryid': $span.CswAttrDom('proparbid'),
														'filtarbitraryid': '',
														'viewbuilderpropid': '',
														'ID': o.ID,
														'propRow': 1,
														'firstColumn': 1,
														'includePropertyName': false,
														'selectedSubfieldVal': '',
														'selectedFilterVal': '',
														'autoFocusInput': false
                                                });
					
					$tbl.CswTable('cell', 1, 5).CswButton('init', {
						'ID':	'addfiltbtn',
						'prefix': o.ID,
						'enabledText': 'Add',
						'disabledText': 'Adding',
						'onclick': function () { 
							var Json = $tbl.CswViewPropFilter('getFilterJson', { 
											ID: o.ID,
											$parent: $span,
											proparbitraryid: $span.CswAttrDom('proparbid')
										});

							var filterxml = $tbl.CswViewPropFilter('makeFilter', { 
								'viewxml': xmlToString($currentviewxml), 
								'filtJson': Json, 
								'onSuccess': function($filterxml) {
									var $propxml = $currentviewxml.find('[arbitraryid="' + $span.CswAttrDom('proparbid') +'"]');
									$(xmlToString($filterxml)).appendTo($propxml);
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

					var $viewnodexml = $currentviewxml.find('[arbitraryid="'+ $a.CswAttrDom('arbid') +'"]')

					var $table = $cell.CswTable({ 'ID': o.ID + '_editrel', 'FirstCellRightAlign': true });
					$table.CswTable('cell', 1, 1).append('Allow Deleting');
					var $allowdeletingcell = $table.CswTable('cell', 1, 2);
                    var $allowdeletingcheck = $allowdeletingcell.CswInput('init',{ID: o.ID + '_adcb',
                                                                  type: CswInput_Types.checkbox
                                                                });

					if($viewnodexml.CswAttrXml('allowdelete').toLowerCase() == 'true') {
						$allowdeletingcheck.CswAttrDom('checked', 'true');
					}

					$table.CswTable('cell', 2, 1).append('Group By');
					var $groupbyselect = $('<select id="' + o.ID + '_gbs" />')
												.appendTo($table.CswTable('cell', 2, 2));
					var dataXml = {
                        Type: $viewnodexml.CswAttrXml('secondtype'),
                        Id: $viewnodexml.CswAttrXml('secondid')
                    }
                    
                    CswAjaxXml({
						url: o.PropNamesUrl,
						data: dataXml,
                        stringify: false,
						success: function($xml) {
							$groupbyselect.empty();
							$('<option value="">[None]</option>')
								.appendTo($groupbyselect);
							$xml.children().each(function() {
								var $prop = $(this);
								var $option = $('<option value="'+ $prop.CswAttrXml('propid') +'">'+ $prop.CswAttrXml('propname') +'</option>')
									.appendTo($groupbyselect)
									.data('propxml', $prop);
								if($viewnodexml.CswAttrXml('groupbypropid') === $prop.CswAttrXml('propid') &&
								    $viewnodexml.CswAttrXml('groupbyproptype') === $prop.CswAttrXml('proptype') &&
								    $viewnodexml.CswAttrXml('groupbypropname') === $prop.CswAttrXml('propname'))
								{
									$option.CswAttrDom('selected', 'true');
								}
							}); // each
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
						if($viewnodexml.CswAttrXml('showintree').toLowerCase() == 'true') {
							$showtreecheck.CswAttrDom('checked', 'true');
						}
					}

					$table.CswTable('cell', 4, 2).CswButton({ 
						'ID': o.ID + '_saverel',
						'enabledText': 'Apply',
						'disableOnClick': false,
						'onclick': function() {
							if($showtreecheck !== undefined)
								$viewnodexml.CswAttrXml('showintree', ($showtreecheck.is(':checked')))
							$viewnodexml.CswAttrXml('allowdelete', ($allowdeletingcheck.is(':checked')))
							if($groupbyselect.val() !== '') {
								var $propxml = $groupbyselect.find(':selected').data('propxml');
								$viewnodexml.CswAttrXml('groupbypropid', $propxml.CswAttrXml('propid'));
								$viewnodexml.CswAttrXml('groupbyproptype', $propxml.CswAttrXml('proptype'));
								$viewnodexml.CswAttrXml('groupbypropname', $propxml.CswAttrXml('propname'));
							} else {
								$viewnodexml.CswAttrXml('groupbypropid', '');
								$viewnodexml.CswAttrXml('groupbyproptype', '');
								$viewnodexml.CswAttrXml('groupbypropname', '');
							}
						} // onClick
					}); // CswButton
				});

				// Property
				$div.find('.vieweditor_viewproplink').click(function() {
					$a = $(this);
					$cell.empty();

					if(viewmode === "Grid")
					{
						var $viewnodexml = $currentviewxml.find('[arbitraryid="'+ $a.CswAttrDom('arbid') +'"]')

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

					var $viewnodexml = $currentviewxml.find('[arbitraryid="'+ $a.CswAttrDom('arbid') +'"]')

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

		function _viewXmlToHtml(stepno, $itemxml)
		{
			var types = {};
			var arbid = $itemxml.CswAttrXml('arbitraryid');
			var nodename = $itemxml.get(0).nodeName;
			var name;
			var rel;
			var skipme = false;
			var skipchildoptions = true;
			var linkclass;

			if(nodename.toLowerCase() === 'treeview')
			{
				if(stepno === CswViewEditor_WizardSteps.step3.step) skipchildoptions = false;

				arbid = "root";
				name = $itemxml.CswAttrXml('viewname');
				rel = "root";
				types.root = { icon: { image: $itemxml.CswAttrXml('iconfilename') } };
				linkclass = 'vieweditor_viewrootlink';
			}
			else if(nodename.toLowerCase() === 'relationship')
			{
				if(stepno === CswViewEditor_WizardSteps.step3.step) skipchildoptions = false;
				if(stepno === CswViewEditor_WizardSteps.step4.step) skipchildoptions = false;

				name = $itemxml.CswAttrXml('secondname');
				var propname = $itemxml.CswAttrXml('propname');
                if( propname !== '' && propname !== undefined)
                {
                    if( $itemxml.CswAttrXml('propowner') === "First" )
                        name += " (by " + $itemxml.CswAttrXml('firstname') + "'s " + propname + ")";
                    else
                        name += " (by " + propname + ")";
                }
				rel = $itemxml.CswAttrXml('secondtype') + '_' + $itemxml.CswAttrXml('secondid');
				types[rel] = { icon: { image: $itemxml.CswAttrXml('secondiconfilename') } };
				linkclass = 'vieweditor_viewrellink';
			}
			else if(nodename.toLowerCase() === 'property')
			{
				if(stepno <= CswViewEditor_WizardSteps.step3.step) skipme = true;
				if(stepno === CswViewEditor_WizardSteps.step5.step) skipchildoptions = false;

				name = $itemxml.CswAttrXml('name');
				rel = "property";
				types.property = { icon: { image: "Images/view/property.gif" } };
				linkclass = "vieweditor_viewproplink";
			}
			else if(nodename.toLowerCase() === 'filter')
			{
				if(stepno <= CswViewEditor_WizardSteps.step4.step) skipme = true;

				name = $itemxml.CswAttrXml('subfieldname') + ' ' + $itemxml.CswAttrXml('filtermode') + ' ' + $itemxml.CswAttrXml('value');
				rel = "filter";
				types.filter = { icon: { image: "Images/view/filter.gif" } };
				linkclass = 'vieweditor_viewfilterlink';
			}
			
			var treestr = '';
			if(!skipme)
			{
				treestr = '<li id="'+ arbid +'" ';
				treestr += '    rel="'+ rel +'" ';
				treestr += '    class="jstree-open" ';
				treestr += '>';
				treestr += ' <a href="#" class="' + linkclass + '" arbid="'+ arbid +'">'+ name +'</a>';
				if(arbid !== "root")
				{
					treestr += ' <span style="" class="vieweditor_deletespan" arbid="'+ arbid +'"></span>';
				}

				treestr += '<ul>';
				$itemxml.children().each(function() { 
					var childcontent = _viewXmlToHtml(stepno, $(this)); 
					treestr += childcontent.htmlstring;
					$.extend(types, childcontent.types);
				});

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
						
                        var dataXml = {
                            StepNo: stepno,
                            ArbitraryId: arbid,
                            ViewXml: xmlToString($currentviewxml)
                        };

                        CswAjaxXml({
							url: o.ChildOptionsUrl,
							data: dataXml,
                            stringify: true,
							success: function($xml) 
							{
								var $select = $('#' + stepno + '_' + arbid + '_child');
								$select.empty();
								$select.append('<option value="">Select...</option>');
								$xml.children().each(function() {
									var $optionxml = $(this);
									var $optionviewxml = $($optionxml.CswAttrXml('value'));
									var $option = $('<option value="'+ $optionviewxml.CswAttrXml('arbitraryid') +'">'+ $optionxml.CswAttrXml('name') +'</option>')
													.appendTo($select);
									$option.data('optionviewxml', $optionxml.CswAttrXml('value'));
								});

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

