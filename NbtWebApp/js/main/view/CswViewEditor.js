/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../pagecmp/CswWizard.js" />
/// <reference path="CswViewPropFilter.js" />

;  (function ($) { /// <param name="$" type="jQuery" />

    $.fn.CswViewEditor = function(options) {
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
        if (options) $.extend(o, options);

        var childPropNames = {
            root: { name: 'root' },
            childrelationships: { name: 'childrelationships' },
            properties: { name: 'properties' },
            filters: { name: 'filters' },
            propfilters: { name: 'propfilters' },
            filtermodes: { name: 'filtermodes' }
        };

        var viewEditClasses = {
            vieweditor_viewrootlink: { name: 'vieweditor_viewrootlink' },
            vieweditor_viewproplink: { name: 'vieweditor_viewproplink' },
            vieweditor_viewfilterlink: { name: 'vieweditor_viewfilterlink' }, 
            vieweditor_addfilter: { name: 'vieweditor_addfilter' },
            vieweditor_deletespan: { name: 'vieweditor_deletespan' },
            vieweditor_childselect: { name: 'vieweditor_childselect' }
        };
        
        var WizardStepArray = [CswViewEditor_WizardSteps.viewselect, CswViewEditor_WizardSteps.attributes, CswViewEditor_WizardSteps.relationships,
            CswViewEditor_WizardSteps.properties, CswViewEditor_WizardSteps.filters, CswViewEditor_WizardSteps.tuning];
        var WizardSteps = { };
        for (var i = 1; i <= WizardStepArray.length; i++)
        {
            WizardSteps[i] = WizardStepArray[i - 1].description;
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
        if (o.startingStep === 1)
            $wizard.CswWizard('button', 'finish', 'disable');

        // Step 1 - Choose a View
        var $div1 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.viewselect.step);
        var instructions = "A <em>View</em> controls the arrangement of information you see in a tree or grid.  " +
            "Views are useful for defining a user's workflow or for creating elaborate search criteria. " +
                "This wizard will take you step by step through the process of creating a new View or " +
                    "editing an existing View.<br/><br/>";
        $div1.append(instructions);
        $div1.append('Select a View to Edit:&nbsp;');
        var $selview_span = $('<span id="' + o.ID + '_selviewname" style="font-weight: bold"></span>')
            .appendTo($div1);
        var $viewgrid_div = $('<div></div>').appendTo($div1);
        var $viewgrid;
        function onViewGridSuccess($vg) {
            $viewgrid = $vg;
        }

        _getViewsGrid(onViewGridSuccess, o.viewid);

        var $div1_btntbl = $div1.CswTable({ ID: o.ID + '_1_btntbl', width: '100%' });
        var $div1_btntbl_cell11 = $div1_btntbl.CswTable('cell', 1, 1);
        var $div1_btntbl_cell12 = $div1_btntbl.CswTable('cell', 1, 2);
        $div1_btntbl_cell12.CswAttrDom('align', 'right');
        var $allcheck_div = $('<div></div>').appendTo($div1_btntbl_cell12);

        IsAdministrator({
                'Yes': function() {
                    var $showOther = $allcheck_div.CswInput('init', {ID: o.ID + '_all',
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
                    if (!isNullOrEmpty(viewid))
                    {
                        var dataJson = {
                            ViewId: viewid
                        };

                        CswAjaxJson({
                                url: o.CopyViewUrl,
                                data: dataJson,
                                success: function(gridJson) {
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
                    if (!isNullOrEmpty(viewid))
                    {
                        if (confirm("Are you sure you want to delete: " + _getSelectedViewName($viewgrid)))
                        {
                            var dataJson = {
                                ViewId: viewid
                            };

                            CswAjaxJson({
                                    url: o.DeleteViewUrl,
                                    data: dataJson,
                                    success: function() {
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
        var $div2 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.attributes.step);
        var $table2 = $div2.CswTable({
                'ID': o.ID + '_tbl2',
                'FirstCellRightAlign': true
            });

        $table2.CswTable('cell', 1, 1).append('View Name:');
        var $viewnametextcell = $table2.CswTable('cell', 1, 2);
        var $viewnametextbox = $viewnametextcell.CswInput('init', {ID: o.ID + '_viewname',
            type: CswInput_Types.text
        });

        $table2.CswTable('cell', 2, 1).append('Category:');
        var $categorytextcell = $table2.CswTable('cell', 2, 2);
        var $categorytextbox = $categorytextcell.CswInput('init', {ID: o.ID + '_category',
            type: CswInput_Types.text
        });

        var v;
        // we don't have json to see whether this is a Property view or not yet,
        // so checking startingStep will have to suffice
        if (o.startingStep === 1)
        {
            v = makeViewVisibilitySelect($table2, 3, 'View Visibility:');
        }

        $table2.CswTable('cell', 4, 1).append('For Mobile:');
        var $formobilecheckcell = $table2.CswTable('cell', 4, 2);
        var $formobilecheckbox = $formobilecheckcell.CswInput('init', {ID: o.ID + '_formobile',
            type: CswInput_Types.checkbox
        });

        $table2.CswTable('cell', 5, 1).append('Display Mode:');
        var $displaymodespan = $table2.CswTable('cell', 5, 2).append('<span id="' + o.ID + '_displaymode"></span>');

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
        var $div3 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.relationships.step);
        $div3.append('Add relationships from the select boxes below:<br/><br/>');
        var $treediv3 = $('<div id="' + CswViewEditor_WizardSteps.relationships.divId + '"><div/>').appendTo($div3);

        // Step 4 - Select Properties
        var $div4 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.properties.step);
        $div4.append('Add properties from the select boxes below:<br/><br/>');
        var $treediv4 = $('<div id="' + CswViewEditor_WizardSteps.properties.divId + '"><div/>').appendTo($div4);

        // Step 5 - Set Filters
        var $div5 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.filters.step);
        $div5.append('Add filters by selecting properties from the tree:<br/><br/>');
        var $treediv5 = $('<div id="' + CswViewEditor_WizardSteps.filters.divId + '"><div/>').appendTo($div5);

        // Step 6 - Fine Tuning
        var $div6 = $wizard.CswWizard('div', CswViewEditor_WizardSteps.tuning.step);
        $div6.append('Select what you want to edit from the tree:<br/><br/>');
        var $table6 = $div6.CswTable({ 'ID': o.ID + '_6_tbl' });

        var currentViewJson;

        function _onBeforePrevious($wizard, stepno)
        {
            return (stepno !== CswViewEditor_WizardSteps.attributes.step || confirm("You will lose any changes made to the current view if you continue.  Are you sure?"));
        }

        function _handleNext($wizard, newstepno)
        {
            CurrentStep = newstepno;
            switch (newstepno)
            {
                case CswViewEditor_WizardSteps.viewselect.step:
                    break;
                case CswViewEditor_WizardSteps.attributes.step:
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
                                    if (v.getvisibilityselect() !== undefined) {
                                        v.getvisibilityselect().val(visibility).trigger('change');
                                        v.getvisroleselect().val('nodes_' + currentViewJson.visibilityroleid);
                                        v.getvisuserselect().val('nodes_' + currentViewJson.visibilityuserid);
                                    }
                                }

                                if (isTrue(currentViewJson.formobile)) {
                                    $formobilecheckbox.CswAttrDom('checked', 'checked');
                                }
                                var mode = currentViewJson.mode;
                                $displaymodespan.text(mode);
                                $gridwidthtextboxcell.CswNumberTextBox('setValue', o.ID + '_gridwidth', currentViewJson.width);
                                if (mode === "Grid") {
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
                case CswViewEditor_WizardSteps.relationships.step:
					// save step 2 content to currentviewjson
                    if (currentViewJson !== undefined)
                    {
                        cacheStepTwo();
                    } // if(currentViewJson !== undefined)

					// make step 3 tree
                    _makeViewTree(CswViewEditor_WizardSteps.relationships.step, $treediv3);
                    break;
                case CswViewEditor_WizardSteps.properties.step:
                    _makeViewTree(CswViewEditor_WizardSteps.properties.step, $treediv4);
                    break;
                case CswViewEditor_WizardSteps.filters.step:
                    _makeViewTree(CswViewEditor_WizardSteps.filters.step, $treediv5);
                    break;
                case CswViewEditor_WizardSteps.tuning.step:
                    _makeViewTree(CswViewEditor_WizardSteps.tuning.step, $table6.CswTable('cell', 1, 1));
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
                        if (!isNullOrEmpty(rolenodeid)) {
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
            if (newstepno === 1)
                $wizard.CswWizard('button', 'finish', 'disable');

            CurrentStep = newstepno;
            switch (newstepno)
            {
                case CswViewEditor_WizardSteps.viewselect.step:
                    break;
                case CswViewEditor_WizardSteps.attributes.step:
                    break;
                case CswViewEditor_WizardSteps.relationships.step:
                    _makeViewTree(CswViewEditor_WizardSteps.relationships.step, $treediv3);
                    break;
                case CswViewEditor_WizardSteps.properties.step:
                    _makeViewTree(CswViewEditor_WizardSteps.properties.step, $treediv4);
                    break;
                case CswViewEditor_WizardSteps.filters.step:
                    _makeViewTree(CswViewEditor_WizardSteps.filters.step, $treediv5);
                    break;
                case CswViewEditor_WizardSteps.tuning.step:
                    _makeViewTree(CswViewEditor_WizardSteps.tuning.step, $table6.CswTable('cell', 1, 1));
                    break;
            }
        }


        function _handleFinish($wizard)
        {
            var viewid = _getSelectedViewId($viewgrid);
            var processView = true;

            if (!isNullOrEmpty(currentViewJson)) {
                if (CurrentStep === CswViewEditor_WizardSteps.attributes.step) {
                    cacheStepTwo();
                }
                if (currentViewJson.mode === 'Grid' &&
                    (currentViewJson.children('relationship').length === 0 ||
                        currentViewJson.children('relationship').children('property').length === 0))
                {
                    processView = confirm('You are attempting to create a Grid without properties. This will not display any information. Do you want to continue?');
                    if (!processView) $wizard.CswWizard('button', 'finish', 'enable');
                }
            }

            if (processView)
            {
                var jsonData = {
                    ViewId: viewid,
                    ViewJson: JSON.stringify(currentViewJson)
                };

                CswAjaxJson({
                        url: o.SaveViewUrl,
                        data: jsonData,
                        success: function() {
                            o.onFinish(viewid, _getSelectedViewMode($viewgrid));
                        } // success
                    });
            } // ajax
        } //_handleFinish

        function _getViewsGrid(onSuccess, selectedviewid)
        {
            var all = false;
            if ($('#' + o.ID + '_all:checked').length > 0)
                all = true;

            $selview_span.text('');
            if (o.startingStep === 1)
                $wizard.CswWizard('button', 'next', 'disable');

            // passing selectedviewid in allows us to translate SessionViewIds to ViewIds
            var dataJson = {
                All: all,
                SelectedViewId: tryParseString(selectedviewid, '')
            };

            CswAjaxJson({
                    url: o.ViewGridUrl,
                    data: dataJson,
                    success: function(gridJson) {

                        $viewgrid_div.empty();
                        var $gridPager = $('<div id="' + o.ID + '_gp" style="width:100%; height:20px;" />')
                            .appendTo($viewgrid_div);
                        var $viewgrid = $('<table id="' + o.ID + '_gt" />')
                            .appendTo($viewgrid_div);

                        var mygridopts = {
                            'autowidth': true,
                            'height': 180,
                            'onSelectRow': function(id, selected) {
                                if (selected)
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

                        if (!isNullOrEmpty(gridJson.selectedpk))
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
            if (o.startingStep === 1) {
                ret = _getSelectedRowValue($viewgrid, o.ColumnFullViewId);
            } else {
                ret = o.viewid;
            }
            return ret;
        }

        function _getSelectedViewMode($viewgrid)
        {
            var ret = '';
            if (o.startingStep === 1) {
                ret = _getSelectedRowValue($viewgrid, o.ColumnViewMode);
            } else {
                ret = o.viewmode;
            }
            return ret;
        }

        function _getSelectedViewName($viewgrid)
        {
            var ret = '';
            if (o.startingStep === 1) {
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
            for (var i in pks)
            {
                if (pks[i].value.toString() === selectedpk.toString())
                    rowid = pks[i].id;
            }
            return rowid;
        }

        function makeTuningStep($content) {
            var $cell = $table6.CswTable('cell', 1, 2);
            var viewmode = _getSelectedViewMode($viewgrid);

            // Root
            $content.find('.vieweditor_viewrootlink').click(function() {
                $cell.empty();
            });

            // Relationship
            $content.find('.vieweditor_viewrellink').click(function() {
                var $a = $(this);
                $cell.empty();
                //$cell.append('For ' + $a.text());

                var objHelper = new ObjectHelper(currentViewJson);
                var arbitraryId = $a.CswAttrDom('arbid');
                var viewnodejson = objHelper.find('arbitraryid', arbitraryId);

                var $table = $cell.CswTable({ 'ID': o.ID + '_editrel', 'FirstCellRightAlign': true });
                $table.CswTable('cell', 1, 1).append('Allow Deleting');
                var $allowdeletingcell = $table.CswTable('cell', 1, 2);
                var $allowdeletingcheck = $allowdeletingcell.CswInput('init', {ID: o.ID + '_adcb',
                    type: CswInput_Types.checkbox
                });

                if (viewnodejson.allowdelete.toLowerCase() == 'true') {
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
                                    var dataProp = { };
                                    dataProp[propName] = thisProp;
                                    var $option = $('<option value="' + thisProp.propid + '">' + propName + '</option>')
                                        .appendTo($groupbyselect)
                                        .data('thisPropData', dataProp);
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
                if (viewmode === "Tree")
                {
                    $table.CswTable('cell', 3, 1).append('Show In Tree');
                    var $showtreecheckcell = $table.CswTable('cell', 3, 2);
                    $showtreecheck = $showtreecheckcell.CswInput('init', {ID: o.ID + '_stcb',
                        type: CswInput_Types.checkbox
                    });
                    if (viewnodejson.showintree.toLowerCase() == 'true') {
                        $showtreecheck.CswAttrDom('checked', 'true');
                    }
                }

                $table.CswTable('cell', 4, 2).CswButton({
                        'ID': o.ID + '_saverel',
                        'enabledText': 'Apply',
                        'disableOnClick': false,
                        'onclick': function() {
                            if ($showtreecheck !== undefined) {
                                viewnodejson.showintree = $showtreecheck.is(':checked');
                                viewnodejson.allowdelete = $allowdeletingcheck.is(':checked');
                            }
                            if ($groupbyselect.val() !== '') {
                                var propData = $groupbyselect.find(':selected').data('thisPropData');
                                viewnodejson.groupbypropid = propData.propid;
                                viewnodejson.groupbyproptype = propData.proptype;
                                viewnodejson.groupbypropname = propData.propname;
                            } else {
                                viewnodejson.groupbypropid = '';
                                viewnodejson.groupbyproptype = '';
                                viewnodejson.groupbypropname = '';
                            }
                        } // onClick
                    }); // CswButton
            }); // $content.find('.vieweditor_viewrellink').click(function() {

            // Property
            $content.find('.vieweditor_viewproplink').click(function() {
                var $a = $(this);
                $cell.empty();

                if (viewmode === "Grid") {
                    var objHelper = new ObjectHelper(currentViewJson);
                    var arbitraryId = $a.CswAttrDom('arbid');
                    var viewNodeData = objHelper.find('arbitraryid', arbitraryId);

                    //$cell.append('For ' + $a.text());
                    var $table = $cell.CswTable({ 'ID': o.ID + '_editprop', 'FirstCellRightAlign': true });

                    $table.CswTable('cell', 1, 1).append('Sort By');
                    var $sortbycheckcell = $table.CswTable('cell', 1, 2);
                    var $sortbycheck = $sortbycheckcell.CswInput('init', {ID: o.ID + '_sortcb',
                        type: CswInput_Types.checkbox
                    });
                    if (viewNodeData.sortby.toLowerCase() == 'true') {
                        $sortbycheck.CswAttrDom('checked', 'true');
                    }

                    $table.CswTable('cell', 2, 1).append('Grid Column Order');
                    var $colordertextcell = $table.CswTable('cell', 2, 2);
                    var $colordertextbox = $colordertextcell.CswInput('init', {ID: o.ID + '_gcotb',
                        type: CswInput_Types.text
                    });
                    $colordertextbox.val(viewNodeData.order);

                    $table.CswTable('cell', 3, 1).append('Grid Column Width (in characters)');
                    var $colwidthtextcell = $table.CswTable('cell', 3, 2);
                    var $colwidthtextbox = $colwidthtextcell.CswInput('init', {ID: o.ID + '_gcwtb',
                        type: CswInput_Types.text
                    });
                    $colwidthtextbox.val(viewNodeData.width);

                    $table.CswTable('cell', 4, 2).CswButton({
                            'ID': o.ID + '_saveprop',
                            'enabledText': 'Apply',
                            'disableOnClick': false,
                            'onclick': function() {
                                viewNodeData.sortby = $sortbycheck.is(':checked');
                                viewNodeData.order = $colordertextbox.val();
                                viewNodeData.width = $colwidthtextbox.val();
                            } // onClick
                        }); // CswButton
                }
            });

            // Filter
            $content.find('.vieweditor_viewfilterlink').click(function() {
                var $a = $(this);
                $cell.empty();
                //$cell.append('For ' + $a.text());
                var objHelper = new ObjectHelper(currentViewJson);
                var arbitraryId = $a.CswAttrDom('arbid');
                var viewNodeData = objHelper.find('arbitraryid', arbitraryId);

                var $table = $cell.CswTable({ 'ID': o.ID + '_editfilt', 'FirstCellRightAlign': true });
                $table.CswTable('cell', 1, 1).append('Case Sensitive');
                var $casecheck = $('<input type="checkbox" id="' + o.ID + '_casecb" />')
                    .appendTo($table.CswTable('cell', 1, 2));
                if (viewNodeData.casesensitive.toLowerCase() === 'true') {
                    $casecheck.CswAttrDom('checked', 'true');
                }

                $table.CswTable('cell', 4, 2).CswButton({
                        'ID': o.ID + '_savefilt',
                        'enabledText': 'Apply',
                        'disableOnClick': false,
                        'onclick': function() {
                            viewNodeData.casesensitive = $casecheck.is(':checked');
                        } // onClick
                    }); // CswButton
            });
        }
        
        function _makeViewTree(stepno, $content) {
            var $tree = $content;
            if (isNullOrEmpty($tree)) {
                $tree = getTreeDiv(stepno);
            }
            var treecontent = viewJsonHtml(stepno, currentViewJson);
            $tree.jstree({
                    "html_data":
                        {
                            "data": treecontent.html
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
            
            if (stepno >= CswViewEditor_WizardSteps.relationships.step && stepno <= CswViewEditor_WizardSteps.properties.step) {
                bindDeleteBtns(stepno);
            }
            
            if (stepno === CswViewEditor_WizardSteps.filters.step) {
                bindViewPropFilterBtns(stepno);
            }
            
            if (stepno === CswViewEditor_WizardSteps.tuning.step) {
                makeTuningStep($content);
            }
            return $tree;
        } // _makeViewTree()
        
        function bindDeleteBtns(stepno) {
            $('.' + viewEditClasses.vieweditor_deletespan.name).each(function() {
                var $span = $(this);
                var arbid = $span.CswAttrXml('arbid');
                var $btn = $span.find('#' + arbid + '_delete');
                $btn.bind('click', function() {
                    var objUtil = new ObjectHelper(currentViewJson);
                    objUtil.remove('arbitraryid', arbid);
                    _makeViewTree(stepno);
                    return CswImageButton_ButtonType.None;
                });
            });
        }
        
        function bindViewPropFilterBtns(stepno) {
            $('.' + viewEditClasses.vieweditor_addfilter.name).each(function() {
                var $span = $(this);
                var arbitraryId = $span.CswAttrXml('proparbid');

                var $btn = $span.find('#' + arbitraryId + '_addfiltbtn');
                $btn.bind('click', function() {
                    var objHelper = new ObjectHelper(currentViewJson);
                
                    var propJson = objHelper.find('arbitraryid', arbitraryId);
                    
                    var $tbl = $span.find('#' + o.ID + '_' + arbitraryId + '_propfilttbl');
                    var newFiltJson = $tbl.CswViewPropFilter('getFilterJson', {
                        ID: o.ID,
                        $parent: $span,
                        filtJson: propJson,
                        proparbitraryid: arbitraryId,
                        allowNullFilterValue: true
                    });

                    $tbl.CswViewPropFilter('makeFilter', {
                        viewJson: currentViewJson,
                        filtJson: newFiltJson,
                        onSuccess: function(newPropJson) {
                            if(false === propJson.hasOwnProperty(childPropNames.propfilters.name)) {
                                propJson[childPropNames.propfilters.name] = { };
                            }
                            $.extend(propJson[childPropNames.propfilters.name], newPropJson);
                            _makeViewTree(stepno);
                        } // onSuccess
                    }); // CswViewPropFilter
                });
            }); 
        }
        
        function viewJsonHtml(stepno, viewJson) {
            var types = { };
            var $ret = $('<ul></ul>');
            var $root = makeViewRootHtml(stepno, viewJson, types)
                            .appendTo($ret);

            if(viewJson.hasOwnProperty(childPropNames.childrelationships.name)) {
                var rootRelationships = viewJson[childPropNames.childrelationships.name];
                makeViewRelationshipsRecursive(stepno, rootRelationships, types, $root);
            }
            
            return { html: xmlToString($ret), types: types };
        }
        
        function makeViewRootHtml(stepno, itemJson, types) {
            var arbid = 'root';
            var name = itemJson.viewname;
            var rel = "root";
            types.root = { icon: { image: tryParseString(itemJson.iconfilename) } };
            var linkclass = viewEditClasses.vieweditor_viewrootlink.name;

            var $ret = makeViewListItem(arbid, linkclass, name, false, false, stepno, childPropNames.root, rel);
            return $ret;
        }
        
        function makeViewRelationshipHtml(stepno, itemJson, types) {
            var arbid = itemJson.arbitraryid;
            //var nodename = itemJson.nodename;
            var name = itemJson.secondname;
            var propname = tryParseString(itemJson.propname);
            if (!isNullOrEmpty(propname)) {
                if (itemJson.propowner === "First") {
                    name += " (by " + itemJson.firstname + "'s " + propname + ")";
                } else {
                    name += " (by " + propname + ")";
                }
            }
            var rel = tryParseString(itemJson.secondtype) + '_' + tryParseString(itemJson.secondid);
            var skipme = false;
            var skipchildoptions = (stepno <= CswViewEditor_WizardSteps.relationships.step);
            var linkclass = viewEditClasses.vieweditor_viewrootlink.name;
            var showDelete = (stepno === CswViewEditor_WizardSteps.relationships.step);
            types[rel] = { icon: { image: tryParseString(itemJson.secondiconfilename) } };
            
            var $ret = makeViewListItem(arbid, linkclass, name, showDelete, skipme, stepno, childPropNames.childrelationships, rel);
            
            if (stepno === CswViewEditor_WizardSteps.relationships.step) {
                var $select = makeChildSelect(stepno, arbid, childPropNames.childrelationships).appendTo($ret);
            }
            if (!skipchildoptions)
            {
                if(itemJson.hasOwnProperty(childPropNames.properties.name)) {
                    var propJson = itemJson[childPropNames.properties.name];
                    if (!isNullOrEmpty(propJson)) {
                        var $propUl = $('<ul></ul>').appendTo($ret);
                        for (var prop in propJson) {
                            if (propJson.hasOwnProperty(prop)) {
                                var thisProp = propJson[prop];
                                $propUl.append(makeViewPropertyHtml(thisProp, types, stepno));
                            }
                        }
                    }
                }
                $ret.append(makeChildSelect(stepno, arbid, childPropNames.properties));
            }
            return $ret;                
        }
        
        function makeViewRelationshipsRecursive(stepno, relationshipJson, types, $content) {
            if (!isNullOrEmpty(relationshipJson)) {
                var $ul = $('<ul></ul>').appendTo($content);
                for (var relationship in relationshipJson) {
                    if (relationshipJson.hasOwnProperty(relationship)) {
                        var thisRelationship = relationshipJson[relationship];
                        var $rel = makeViewRelationshipHtml(stepno, thisRelationship, types)
                                        .appendTo($ul);
                        if(thisRelationship.hasOwnProperty(childPropNames.childrelationships.name)) {
                            var childRelationships = thisRelationship[childPropNames.childrelationships.name];
                            makeViewRelationshipsRecursive(stepno, childRelationships, types, $rel);
                        }
                    }
                }
            }
        }
        
        function makeViewPropertyHtml(itemJson, types, stepno) {
            var $ret = $('<li></li>');
            if(!isNullOrEmpty(itemJson)) {
                var arbid = itemJson.arbitraryid;
                //var nodename = itemJson.nodename;
                var name = itemJson.name;
                var rel = 'property';
                var skipme = (stepno <= CswViewEditor_WizardSteps.relationships.step);
                var skipchildoptions = (stepno <= CswViewEditor_WizardSteps.properties.step);
                var linkclass = viewEditClasses.vieweditor_viewproplink.name;
                var showDelete = (stepno === CswViewEditor_WizardSteps.properties.step);
                if (!isNullOrEmpty(name)) {
                    $ret = makeViewListItem(arbid, linkclass, name, showDelete, skipme, stepno, childPropNames.properties, rel);
                }
                if (!isNullOrEmpty($ret) && !skipchildoptions) {
                    var $filtUl = $('<ul></ul>').appendTo($ret);
                    if (itemJson.hasOwnProperty(childPropNames.propfilters.name)) {
                        var filterJson = itemJson[childPropNames.propfilters.name];
                        if (!isNullOrEmpty(filterJson)) {
                            for (var filter in filterJson) {
                                if (filterJson.hasOwnProperty(filter)) {
                                    var thisFilt = filterJson[filter];
                                    $filtUl.append(makeViewPropertyFilterHtml(thisFilt, stepno, types, arbid));            
                                }
                            }
                        }
                    }
                    $filtUl.append(makeViewPropertyFilterHtml(null, stepno, types, arbid));
                }
                types.property = { icon: { image: "Images/view/property.gif" } };
            }
            return $ret;
        }
        
        function makeViewPropertyFilterHtml(itemJson, stepno, types, propArbId) {
            var $ret = $('<li></li>');
            if (stepno === CswViewEditor_WizardSteps.filters.step) {
                if (!isNullOrEmpty(itemJson)) {
                    $ret.append(makeViewPropFilterStaticSpan(propArbId, itemJson));
                    $ret.append(makeDeleteSpan(propArbId, stepno));
                } else {
                    $ret.append(makeViewPropFilterAddSpan(propArbId));                    
                }
            }
            types.filter = { icon: { image: "Images/view/filter.gif" } };
            return $ret;            
        }

        function makeViewPropFilterStaticSpan(propArbId, filterJson) {
            var $span = $('<span class="' + viewEditClasses.vieweditor_addfilter.name + '" proparbid="' + propArbId + '"></span>');
            var $tbl = $span.CswTable({ 'ID': o.ID + '_' + propArbId + '_propfilttbl' });
            $tbl.css('display', 'inline-table');

            var filtArbitraryId = tryParseString(filterJson.arbitraryid);
            $tbl.CswViewPropFilter('add', {
                ID: o.ID + '_' + propArbId + '_propfilttbl',
                propsData: filterJson,
                proparbitraryid: propArbId,
                filterarbitraryid: filtArbitraryId,
                propRow: 1,
                firstColumn: 1,
                includePropertyName: false,
                autoFocusInput: false
            });

            return $span;
        }
        
        function makeViewPropFilterAddSpan(propArbId) {
            var $span = $('<span class="' + viewEditClasses.vieweditor_addfilter.name + '" proparbid="' + propArbId + '"></span>');
            var $tbl = $span.CswTable({ 'ID': o.ID + '_' + propArbId + '_propfilttbl' });
            $tbl.css('display', 'inline-table');
            $tbl.CswViewPropFilter('init', {
                viewJson: currentViewJson,
                ID: o.ID + '_' + propArbId + '_propfilttbl',
                propsData: null,
                proparbitraryid: propArbId,
                propRow: 1,
                firstColumn: 1,
                includePropertyName: false,
                autoFocusInput: false
            });

            $tbl.CswTable('cell', 1, 5).CswButton('init', {
                ID: propArbId + '_addfiltbtn',
                enabledText: 'Add',
                disabledText: 'Adding'
            }); // CswButton
            return $span;
        }
        
        function makeViewListItem(arbid, linkclass, name, showDelete, skipme, stepno, propName, rel) {
            var $ret = $('<li></li>');
            if (!skipme) {
                $ret = $('<li id="' + arbid + '" rel="' + rel + '" class="jstree-open"></li>');
                $ret.append($('<a href="#" class="' + linkclass + '" arbid="' + arbid + '">' + name + '</a>'));
                if (showDelete) {
                    $ret.append(makeDeleteSpan(arbid, stepno));
                }
            }
            return $ret;
        }
        
        function makeDeleteSpan(arbid) {
            var $td = $('<span style="" class="' + viewEditClasses.vieweditor_deletespan.name + '" arbid="' + arbid + '"></span>');
            $td.CswImageButton({
                ButtonType: CswImageButton_ButtonType.Delete,
                AlternateText: 'Delete',
                ID: arbid + '_delete'
            });
            return $td;
        }
        
        function getTreeDiv(stepno) {
            var ret = '';
            switch (stepno) {
                case CswViewEditor_WizardSteps.relationships.step:
                    ret = $('#' + CswViewEditor_WizardSteps.relationships.divId);
                    break;
                case CswViewEditor_WizardSteps.properties.step:
                    ret = $('#' + CswViewEditor_WizardSteps.properties.divId);
                    break;
                case CswViewEditor_WizardSteps.filters.step:
                    ret = $('#' + CswViewEditor_WizardSteps.filters.divId);
                    break;
                case CswViewEditor_WizardSteps.tuning.step:
                    ret = $('#' + o.ID);
                    break;
            }
            return ret;
        }
        
        function makeChildSelect(stepno, arbid, propName) {
            var $select = '';
            
            if (canAddChildSelect(stepno,propName,arbid)) {
                $select = $('<ul><li><select id="' + stepno + '_' + arbid + '_child" arbid="' + arbid + '" class="' + viewEditClasses.vieweditor_childselect.name + '"></select></li></ul>');

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
                                    var dataOpt = { };
                                    dataOpt[optionName] = thisOpt;
                                    var $option = $('<option value="' + thisOpt.arbitraryid + '">' + optionName + '</option>')
                                        .appendTo($select);
                                    $option.data('thisViewJson', dataOpt);
                                }
                            }
                            
                            $select.change(function() {
                                var $this = $(this);
                                var childJson = $this.find('option:selected').data('thisViewJson');
                                if (arbid === "root") {
                                    $.extend(currentViewJson.childrelationships, childJson);
                                } else {
                                    var objUtil = new ObjectHelper(currentViewJson);
                                    var parentObj = objUtil.find('arbitraryid', arbid);
                                    var collection = '';
                                    switch (stepno) {
                                        case CswViewEditor_WizardSteps.relationships.step:
                                            collection = childPropNames.childrelationships.name;
                                            break;
                                        case CswViewEditor_WizardSteps.properties.step:
                                            collection = childPropNames.properties.name;
                                            break;
                                    }
                                    var objCollection = parentObj[collection];
                                    if (isNullOrEmpty(objCollection)) {
                                        objCollection = { };
                                        parentObj[collection] = objCollection;
                                    }
                                    $.extend(objCollection, childJson);
                                }
                                _makeViewTree(stepno);
                            });
                            
                        } // success
                    }); // ajax
            }
            return $select;
        }
        
        function canAddChildSelect(stepno,propName,arbid) {
            var ret = false;
            
            switch (stepno) {
                case CswViewEditor_WizardSteps.relationships.step:
                    if (propName === childPropNames.childrelationships || propName === childPropNames.root) {
                        ret = true;
                    }
                    break;
                case CswViewEditor_WizardSteps.properties.step:
                    if (propName === childPropNames.properties && arbid !== 'root') {
                        ret = true;
                    }
                    break;
                case CswViewEditor_WizardSteps.filters.step:
                    if (propName === childPropNames.propfilters && arbid !== 'root') {
                        ret = true;
                    }
                    break;
            }
            
            return ret;
        }
        
        return $div;

    }; // $.fn.CswViewEditor
}) (jQuery);

