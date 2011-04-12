; (function ($) {

	$.fn.CswViewEditor = function (options) 
	{
		var o = {
			ViewGridUrl: '/NbtWebApp/wsNBT.asmx/getViewGrid',
			ViewInfoUrl: '/NbtWebApp/wsNBT.asmx/getViewInfo',
			SaveViewUrl: '/NbtWebApp/wsNBT.asmx/saveViewInfo',
			CopyViewUrl: '/NbtWebApp/wsNBT.asmx/copyView',
			DeleteViewUrl: '/NbtWebApp/wsNBT.asmx/deleteView',
			viewid: '',
			ID: 'vieweditor',
			ColumnViewName: 'VIEWNAME',
			ColumnViewId: 'NODEVIEWID',
			ColumnViewMode: 'VIEWMODE',
			onCancel: function() {},
			onFinish: function(viewid, viewmode) {}
		};
		if(options) $.extend(o, options);

		var $parent = $(this);
		var $div = $('<div></div>')
					.appendTo($parent);

		var $wizard = $div.CswWizard('init', { 
				'ID': o.ID + '_wizard',
				'Title': 'Edit View',
				'StepCount': 5,
				'Steps': 
				{ 
					1: 'Choose a View',
					2: 'Edit View Attributes',
					3: 'Add Relationships',
					4: 'Select Properties',
					5: 'Set Filters'
				},
				'FinishText': 'Save and Finish',
				'onNext': _handleNext,
				//'onPrevious': _handlePrevious,
				'onCancel': o.onCancel,
				'onFinish': _handleFinish 
			});

		// Step 1 - Choose a View
		var $div1 = $wizard.CswWizard('div', 1);
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
		$div1_btntbl_cell12.attr('align', 'right');
		var $allcheck_div = $('<div></div>').appendTo($div1_btntbl_cell12);

		IsAdministrator({
			'Yes': function() {
				$('<input type="checkbox" id="'+ o.ID + '_all">Show Other Roles/Users</input>')
					.appendTo($allcheck_div)
					.click(function() { 
						_getViewsGrid(onViewGridSuccess); 
					});
			}
		});

		var $copyviewbtn = $div1_btntbl_cell11.CswButton({
			'ID': o.ID + '_copyview',
			'enabledText': 'Copy View',
			'disableOnClick': true,
			'onclick': function() {
				var viewid = _getSelectedRowValue($viewgrid, o.ColumnViewId);
				if(viewid != '' && viewid != undefined)
				{
					CswAjaxJSON({
						url: o.CopyViewUrl,
						data: "{ ViewId: "+ viewid +" }",
						success: function (gridJson) {
							_getViewsGrid(onViewGridSuccess, gridJson.copyviewid); 
						}
					});
				} // if(viewid != '' && viewid != undefined)
			} // onclick
		}); // copy button
		$copyviewbtn.CswButton('disable');

		var $deleteviewbtn = $div1_btntbl_cell11.CswButton({
			'ID': o.ID + '_deleteview',
			'enabledText': 'Delete View',
			'disableOnClick': true,
			'onclick': function() {
			    var viewid = _getSelectedRowValue($viewgrid, o.ColumnViewId);
				if(viewid != '' && viewid != undefined)
				{
					if(confirm("Are you sure you want to delete: " + _getSelectedRowValue($viewgrid, o.ColumnViewName)))
					{
						CswAjaxJSON({
							url: o.DeleteViewUrl,
							data: "{ ViewId: "+ viewid +" }",
							success: function (gridJson) {
								_getViewsGrid(onViewGridSuccess); 
								$copyviewbtn.CswButton('disable');
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

		$wizard.CswWizard('button', 'next', 'disable');


		// Step 2 - Edit View Attributes

		var $div2 = $wizard.CswWizard('div', 2);
		var $table2 = $div2.CswTable({ 
				'ID': o.ID + '_tbl2', 
				'FirstCellRightAlign': true 
		});

		$table2.CswTable('cell', 1, 1).append('View Name:');
		var $viewnametextbox = $('<input type="textbox" id="' + o.ID + '_viewname" />')
								.appendTo($table2.CswTable('cell', 1, 2));

		$table2.CswTable('cell', 2, 1).append('Category:');
		var $categorytextbox = $('<input type="textbox" id="' + o.ID + '_category" />')
								.appendTo($table2.CswTable('cell', 2, 2));

		$table2.CswTable('cell', 3, 1).append('View Visibility:');
		var v = _makeVisibilitySelect($table2.attr('id'), $table2.CswTable('cell', 3, 2));
		$table2.CswTable('cell', 4, 1).append('For Mobile:');
		var $formobilecheckbox = $('<input type="checkbox" id="' + o.ID + '_formobile" />')
								.appendTo($table2.CswTable('cell', 4, 2));

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

		var $div3 = $wizard.CswWizard('div', 3);

		// Step 4 - Select Properties

		var $div4 = $wizard.CswWizard('div', 4);

		// Step 5 - Set Filters

		var $div5 = $wizard.CswWizard('div', 5);


		var $currentviewxml;

		function _handleNext(newstepno)
		{
			switch(newstepno)
			{
				case 1:
					break;
				case 2:
					CswAjaxXml({
						url: o.ViewInfoUrl,
						data: 'ViewId='+ _getSelectedRowValue($viewgrid, o.ColumnViewId),
						success: function($xml) {
							$currentviewxml = $xml;

							$viewnametextbox.val($currentviewxml.attr('viewname'));
							$categorytextbox.val($currentviewxml.attr('category'));
							v.getvisibilityselect().val($currentviewxml.attr('visibility')).trigger('change');
							v.getvisroleselect().val('nodes_' + $currentviewxml.attr('visibilityroleid'));
							v.getvisuserselect().val('nodes_' + $currentviewxml.attr('visibilityuserid'));
							if($currentviewxml.attr('formobile') == 'true') {
								$formobilecheckbox.attr('checked', 'true');
							}
							var mode = $currentviewxml.attr('mode')
							$displaymodespan.text(mode);
							$gridwidthtextboxcell.CswNumberTextBox('setValue', $currentviewxml.attr('width'));
							if(mode == "Grid") {
								$gridwidthlabelcell.show();
								$gridwidthtextboxcell.show();
							} else {
								$gridwidthlabelcell.hide();
								$gridwidthtextboxcell.hide();
							}
						} // success
					}); // ajax
					break;
				case 3:
					_saveAll();					
					break;
				case 4:
					_saveAll();
					break;
				case 5:
					_saveAll();
					break;
			} // switch(newstepno)
		} // _handleNext()

		function _saveAll()
		{
			if($currentviewxml != undefined)
			{
				$currentviewxml.attr('viewname', $viewnametextbox.val());
				$currentviewxml.attr('category', $categorytextbox.val());
				$currentviewxml.attr('visibility', v.getvisibilityselect().val());
				
				// temporary workaround
				var rolenodeid = v.getvisroleselect().val();
				if(rolenodeid != '' && rolenodeid != undefined)
					rolenodeid = rolenodeid.substr('nodes_'.length)
				var usernodeid = v.getvisuserselect().val();
				if(usernodeid != '' && usernodeid != undefined)
					usernodeid = usernodeid.substr('nodes_'.length)
				$currentviewxml.attr('visibilityroleid', rolenodeid);
				$currentviewxml.attr('visibilityuserid', usernodeid);
				
				$currentviewxml.attr('formobile', ($formobilecheckbox.attr('checked') == 'true'));
				$currentviewxml.attr('width', $gridwidthtextboxcell.CswNumberTextBox('value'));
			} // if($currentviewxml != undefined)
		} // _saveAll()

		function _handleFinish()
		{
			var viewid = _getSelectedRowValue($viewgrid, o.ColumnViewId);
			_saveAll();

			CswAjaxXml({
				url: o.SaveViewUrl,
				data: 'ViewId='+ viewid +'&ViewXml='+ xmlToString($currentviewxml),
				success: function ($xml) {
					o.onFinish(viewid, _getSelectedRowValue($viewgrid, o.ColumnViewMode));
				} // success
			}); // ajax
		} //_handleFinish

		function _getViewsGrid(onSuccess, selectedrowpk)
		{
			var all = false;
			if($('#'+ o.ID + '_all:checked').length > 0)
				all = true;

			$selview_span.text('');
			$wizard.CswWizard('button', 'next', 'disable');

			CswAjaxJSON({
				url: o.ViewGridUrl,
				data: "{ All: "+ all +" }",
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
							$selview_span.text(_getSelectedRowValue($viewgrid, o.ColumnViewName));
							$wizard.CswWizard('button', 'next', 'enable');
						},
						'pager': $gridPager
					};
					$.extend(gridJson, mygridopts);

					$viewgrid.jqGrid(gridJson)
								.hideCol(o.ColumnViewId);

					if(selectedrowpk != undefined)
					{
						$viewgrid.setSelection(_getRowForPk($viewgrid, selectedrowpk));
					}
					onSuccess($viewgrid);
				} // success
			}); // ajax
		} // _getViewsGrid()
		
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
				if(pks[i].value == selectedpk)
					rowid = pks[i].id;
			}
			return rowid;
		}

		function _makeVisibilitySelect(id, $parent)
		{
			var $visibilityselect;
			var $visroleselect;
			var $visuserselect;
			IsAdministrator({
				'Yes': function() {
						
						$visibilityselect = $('<select id="' + id + '_vissel" />')
													.appendTo($parent);
						$visibilityselect.append('<option value="User">User:</option>');
						$visibilityselect.append('<option value="Role">Role:</option>');
						$visibilityselect.append('<option value="Global">Global</option>');

						$visroleselect = $parent.CswNodeSelect('init', {
																			'ID': id + '_visrolesel', 
																			'objectclass': 'RoleClass',
																		}).hide();
						$visuserselect = $parent.CswNodeSelect('init', {
																			'ID': id + '_visusersel', 
																			'objectclass': 'UserClass'
																		})

						$visibilityselect.change(function() {
							var val = $visibilityselect.val();
							if(val == 'Role')
							{
								$visroleselect.show();
								$visuserselect.hide();
							}
							else if(val == 'User')
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

		return $div;

	} // $.fn.CswViewEditor
}) (jQuery);

