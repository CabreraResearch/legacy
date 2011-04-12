; (function ($) {

	$.fn.CswViewEditor = function (options) 
	{
		var o = {
			ViewGridUrl: '/NbtWebApp/wsNBT.asmx/getViewGrid',
			CopyViewUrl: '/NbtWebApp/wsNBT.asmx/copyView',
			DeleteViewUrl: '/NbtWebApp/wsNBT.asmx/deleteView',
			viewid: '',
			ID: 'vieweditor',
			ColumnViewName: 'VIEWNAME',
			ColumnViewId: 'NODEVIEWID',
			onCancel: function() {},
			onFinish: function() {}
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
					},  // onAddView
					'onClose': function() {
						$newviewbtn.CswButton('enable');
					}  // onClose
				}); // CswDialog
			} // onclick
		})

		$wizard.CswWizard('button', 'next', 'disable');







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
					var $viewgrid = $('<table id="'+ o.ID + '" />')
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


		function _handleFinish()
		{
			o.onFinish();
		}


		return $div;

	} // $.fn.CswViewEditor
}) (jQuery);

