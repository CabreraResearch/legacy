/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";
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
            onCancel: null, // function ($wizard) {},
            onFinish: null, // function (viewid, viewmode) {},
            startingStep: 1
        };
        if (options) $.extend(o, options);
                
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
        var $viewgriddiv = $('<div />')
                            .appendTo($div1);
        var cswViewGrid;
        var rowid;
        var $viewgrid;

        _getViewsGrid(o.viewid);

        var $div1_btntbl = $div1.CswTable({ ID: o.ID + '_1_btntbl', width: '100%' });
        var $div1_btntbl_cell11 = $div1_btntbl.CswTable('cell', 1, 1);
        var $div1_btntbl_cell12 = $div1_btntbl.CswTable('cell', 1, 2);
        $div1_btntbl_cell12.CswAttrDom('align', 'right');
        var $allcheck_div = $('<div></div>').appendTo($div1_btntbl_cell12);

        Csw.clientSession.isAdministrator({
                'Yes': function () {
                    /* Show Other */
                    $allcheck_div.CswInput('init', {ID: o.ID + '_all',
                        type: CswInput_Types.checkbox,
                        onChange: function () {
                            _getViewsGrid();
                        }
                    });
                    $allcheck_div.append('Show Other Roles/Users');
                }
            });

        var $copyviewbtn = $div1_btntbl_cell11.CswButton({
                'ID': o.ID + '_copyview',
                'enabledText': 'Copy View',
                'disableOnClick': true,
                'onclick': function () {
                    var viewid = _getSelectedViewId();
                    if (!Csw.isNullOrEmpty(viewid))
                    {
                        var dataJson = {
                            ViewId: viewid
                        };

                        Csw.ajax.post({
                                url: o.CopyViewUrl,
                                data: dataJson,
                                success: function (gridJson) {
                                    _getViewsGrid(gridJson.copyviewid);
                                },
                                error: function () {
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
                onclick: function () {
                    var viewid = _getSelectedViewId();
                    if (!Csw.isNullOrEmpty(viewid)) {
                        /* remember: confirm is globally blocking call */
                        if (confirm("Are you sure you want to delete: " + _getSelectedViewName())) {
                            var dataJson = {
                                ViewId: viewid
                            };

                            Csw.ajax.post({
                                    url: o.DeleteViewUrl,
                                    data: dataJson,
                                    success: function () {
                                        _getViewsGrid();
                                        $copyviewbtn.CswButton('disable');
                                    },
                                    error: function () {
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
                'onclick': function () {
                    $.CswDialog('AddViewDialog', {
                        onAddView: function (newviewid) {
                            _getViewsGrid(newviewid);
                        },
                        onClose: function () {
                            $newviewbtn.CswButton('enable');
                        }
                    }); // CswDialog
                } // onclick
            });

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
            v = Csw.makeViewVisibilitySelect($table2, 3, 'View Visibility:');
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
            'onchange': function () { }
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

        function _onBeforePrevious($prevWizard, stepno) {
            /* remember: confirm is globally blocking call */
            return (stepno !== CswViewEditor_WizardSteps.attributes.step || confirm("You will lose any changes made to the current view if you continue.  Are you sure?"));
        }

        function _handleNext($nextWizard, newstepno) {
            CurrentStep = newstepno;
            switch (newstepno)
            {
                case CswViewEditor_WizardSteps.viewselect.step:
                    break;
                case CswViewEditor_WizardSteps.attributes.step:
                    $nextWizard.CswWizard('button', 'finish', 'enable');
                    $nextWizard.CswWizard('button', 'next', 'disable');

                    var jsonData = {
                        ViewId: _getSelectedViewId()
                    };

                    Csw.ajax.post({
                            url: o.ViewInfoUrl,
                            data: jsonData,
                            success: function (data) {
                                currentViewJson = data.TreeView;

                                $viewnametextbox.val(currentViewJson.viewname);
                                $categorytextbox.val(currentViewJson.category);
                                var visibility = Csw.string(currentViewJson.visibility);
                                if (visibility !== 'Property') {
                                    if (v.getvisibilityselect() !== undefined) {
                                        v.getvisibilityselect().val(visibility).trigger('change');
                                        v.getvisroleselect().val('nodes_' + currentViewJson.visibilityroleid);
                                        v.getvisuserselect().val('nodes_' + currentViewJson.visibilityuserid);
                                    }
                                }

                                if (Csw.bool(currentViewJson.formobile)) {
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

                                $nextWizard.CswWizard('button', 'next', 'enable');
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
                        if (!Csw.isNullOrEmpty(rolenodeid)) {
                            rolenodeid = rolenodeid.substr('nodes_'.length);
                        }
                    }
                    currentViewJson.visibilityroleid = rolenodeid;

                    var usernodeid = '';
                    if (visibility === 'User') {
                        usernodeid = v.getvisuserselect().val();
                        if (!Csw.isNullOrEmpty(usernodeid)) {
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

        function gridHasOneProp() {
            var ret = false;
            if(Csw.contains(currentViewJson, 'childrelationships')) {
                Csw.crawlObject(currentViewJson.childrelationships, function (childObj) {
                    if (ret) {
                        return false;
                    } 
                    else if (Csw.contains(childObj, 'properties')) {
                        Csw.crawlObject(childObj.properties, function (propObj) {
                            if(false === Csw.isNullOrUndefined(propObj)) {
                                ret = true;
                                return false;
                            }
                        }, false);
                    }
                }, true);
            }
            return ret;
        }

        function _handleFinish($finishWizard)
        {
            var viewid = _getSelectedViewId();
            var processView = true;

            if (!Csw.isNullOrEmpty(currentViewJson)) {
                if (CurrentStep === CswViewEditor_WizardSteps.attributes.step) {
                    cacheStepTwo();
                }
                
                if (currentViewJson.mode === 'Grid' && false === gridHasOneProp()) {
                    processView = confirm('You are attempting to create a Grid without properties. This will not display any information. Do you want to continue?');
                    if (false === processView) $finishWizard.CswWizard('button', 'finish', 'enable');
                }
            }

            if (processView)
            {
                var jsonData = {
                    ViewId: viewid,
                    ViewJson: JSON.stringify(currentViewJson)
                };

                Csw.ajax.post({
                        url: o.SaveViewUrl,
                        data: jsonData,
                        success: function () {
                            o.onFinish(viewid, _getSelectedViewMode());
                        } // success
                    });
            } // ajax
        } //_handleFinish

        function _getViewsGrid(selectedviewid)
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
                SelectedViewId: Csw.string(selectedviewid, '')
            };

            Csw.ajax.post({
                    url: o.ViewGridUrl,
                    data: dataJson,
                    success: function (gridJson) {

                        if (Csw.isNullOrEmpty($viewgrid) || $viewgrid.length === 0) {
                            $viewgrid = $('<div id="' + o.ID + '_csw_viewGrid_outer"></div>').appendTo($viewgriddiv);
                        } else {
                            $viewgrid.empty();
                        }

                        var g = {
                            ID: o.ID,
                            pagermode: 'none',
                            gridOpts: {
                                autowidth: true,
                                height: 180,
                                onSelectRow: function (id, selected) {
                                    rowid = id;
                                    if (selected)
                                    {
                                        $copyviewbtn.CswButton('enable');
                                        $deleteviewbtn.CswButton('enable');
                                        $selview_span.text(_getSelectedViewName(id));
                                        $wizard.CswWizard('button', 'next', 'enable');
                                    }
                                    else
                                    {
                                        $copyviewbtn.CswButton('disable');
                                        $deleteviewbtn.CswButton('disable');
                                        $selview_span.text("");
                                        $wizard.CswWizard('button', 'next', 'disable');
                                    }
                                }
                            }
                        };
                        $.extend(g.gridOpts, gridJson);
                        cswViewGrid = CswGrid(g, $viewgrid);
                        cswViewGrid.$gridPager.css({width: '100%', height: '20px' });
                        
                        cswViewGrid.hideColumn(o.ColumnFullViewId);
                        if (!Csw.isNullOrEmpty(gridJson.selectedpk))
                        {
                            rowid = cswViewGrid.getRowIdForVal(gridJson.selectedpk, o.ColumnViewId);
                            cswViewGrid.setSelection(rowid);
                            cswViewGrid.scrollToRow(rowid);
                        }
                    } // success
                }); // ajax
        } // _getViewsGrid()

        function _getSelectedViewId(selRowId)
        {
            var ret;
            if (o.startingStep === 1) {
                ret = cswViewGrid.getValueForColumn(o.ColumnFullViewId, selRowId);
            } else {
                ret = o.viewid;
            }
            return ret;
        }

        function _getSelectedViewMode(selRowId)
        {
            var ret;
            if (o.startingStep === 1) {
                ret = cswViewGrid.getValueForColumn(o.ColumnViewMode, selRowId);
            } else {
                ret = o.viewmode;
            }
            return ret;
        }

        function _getSelectedViewName(selRowId)
        {
            var ret;
            if (o.startingStep === 1) {
                ret = cswViewGrid.getValueForColumn(o.ColumnViewName, selRowId);
            } else {
                ret = o.viewname;
            }
            return ret;
        }
        
        function makeTuningStep($content) {
            var $cell = $table6.CswTable('cell', 1, 2);
            var viewmode = _getSelectedViewMode();

            // Root
            $content.find('.' + viewEditClasses.vieweditor_viewrootlink.name).click(function () {
                $cell.empty();
            });

            // Relationship
            $content.find('.' + viewEditClasses.vieweditor_viewrellink.name).click(function () {
                var $a = $(this);
                $cell.empty();
                //$cell.append('For ' + $a.text());

                var objHelper = new Csw.object(currentViewJson);
                var arbitraryId = $a.CswAttrDom('arbid');
                var viewnodejson = objHelper.find('arbitraryid', arbitraryId);

                var $table = $cell.CswTable({ 'ID': o.ID + '_editrel', 'FirstCellRightAlign': true });
                $table.CswTable('cell', 1, 1).append('Allow Deleting');
                var $allowdeletingcell = $table.CswTable('cell', 1, 2);
                var $allowdeletingcheck = $allowdeletingcell.CswInput('init', 
                                                                { ID: o.ID + '_adcb',
                                                                  type: CswInput_Types.checkbox,
                                                                  onChange: function () {
                                                                      var $this = $(this);
                                                                      viewnodejson.allowdelete = $this.is(':checked');
                                                                  }
                                                            });

                if (Csw.bool(viewnodejson.allowdelete)) {
                    $allowdeletingcheck.CswAttrDom('checked', 'true');
                }

                $table.CswTable('cell', 2, 1).append('Group By');
                var $groupbyselect = $table.CswTable('cell', 2, 2)
                                            .CswSelect('init', 
                                                { ID: o.ID + '_gbs',
                                                  onChange: function () {
                                                      var $selected = $groupbyselect.find(':selected');
                                                      var selval = $selected.val();
                                                      var propData;

                                                      if (false === Csw.isNullOrEmpty(selval)) {
                                                          if(selval === 'none') {
                                                              viewnodejson.groupbypropid = '';
                                                              viewnodejson.groupbyproptype = '';
                                                              viewnodejson.groupbypropname = '';
                                                          } else {
                                                              propData = $selected.data('thisPropData');
                                                              viewnodejson.groupbypropid = Csw.string(propData.propid);
                                                              viewnodejson.groupbyproptype = Csw.string(propData.proptype);
                                                              viewnodejson.groupbypropname = Csw.string(propData.propname);
                                                          }
                                                      } // if (false === Csw.isNullOrEmpty(selval)) {
                                                } // onChange
                                            }); // CswSelect
                
                var jsonData = {
                    Type: viewnodejson.secondtype,
                    Id: viewnodejson.secondid
                };
                Csw.ajax.post({
                        url: o.PropNamesUrl,
                        data: jsonData,
                        success: function (data) {
                            $groupbyselect.empty();
                            var groupOpts = [{ value: 'none', display: '[None]' }];
                            var groupSel = 'none';
                            for (var propKey in data) {
                                if (data.hasOwnProperty(propKey)) {
                                    var thisProp = data[propKey];
                                    groupOpts.push({ value: thisProp.propid, 
                                                     display: thisProp.propname, 
                                                     data: thisProp, 
                                                     dataName: 'thisPropData' });
                                    if (viewnodejson.groupbypropid === thisProp.propid &&
                                        viewnodejson.groupbyproptype === thisProp.proptype &&
                                        viewnodejson.groupbypropname === thisProp.propname)
                                    {
                                        groupSel = thisProp.propid;
                                    }
                                }
                            } // each
                            $groupbyselect.CswSelect('setoptions', groupOpts, groupSel);
                        } // success
                    }); // ajax

                var $showtreecheck;
                if (viewmode === "Tree") {
                    $table.CswTable('cell', 3, 1).append('Show In Tree');
                    var $showtreecheckcell = $table.CswTable('cell', 3, 2);
                    $showtreecheck = $showtreecheckcell.CswInput('init', 
                                                            { ID: o.ID + '_stcb',
                                                              type: CswInput_Types.checkbox,
                                                              onChange: function () {
                                                                  var $this = $(this);
                                                                  viewnodejson.showintree = $this.is(':checked');
                                                              }
                                                        });
                    if (Csw.bool(viewnodejson.showintree)) {
                        $showtreecheck.CswAttrDom('checked', 'true');
                    }
                }
            }); // $content.find('.vieweditor_viewrellink').click(function () {

            // Property
            $content.find('.' + viewEditClasses.vieweditor_viewproplink.name).click(function () {
                var $a = $(this);
                $cell.empty();

                if (viewmode === "Grid") {
                    var objHelper = new Csw.object(currentViewJson);
                    var arbitraryId = $a.CswAttrDom('arbid');
                    var viewNodeData = objHelper.find('arbitraryid', arbitraryId);

                    //$cell.append('For ' + $a.text());
                    var $table = $cell.CswTable({ 'ID': o.ID + '_editprop', 'FirstCellRightAlign': true });

                    $table.CswTable('cell', 1, 1).append('Sort By');
                    var $sortbycheckcell = $table.CswTable('cell', 1, 2);
                    var $sortbycheck = $sortbycheckcell.CswInput('init', 
                                                            { ID: o.ID + '_sortcb',
                                                              type: CswInput_Types.checkbox,
                                                              onChange: function () {
                                                                  var $this = $(this);
                                                                  viewNodeData.sortby = $this.is(':checked');
                                                              }
                    });
                    if (Csw.bool(viewNodeData.sortby)) {
                        $sortbycheck.CswAttrDom('checked', 'true');
                    }

                    $table.CswTable('cell', 2, 1).append('Grid Column Order');
                    var $colordertextcell = $table.CswTable('cell', 2, 2);
                    var $colordertextbox = $colordertextcell.CswInput('init', 
                                                                { ID: o.ID + '_gcotb',
                                                                  type: CswInput_Types.text,
                                                                  onChange: function () {
                                                                      var $this = $(this);
                                                                      viewNodeData.order = $this.val();
                                                                  }
                    });
                    $colordertextbox.val(viewNodeData.order);

                    $table.CswTable('cell', 3, 1).append('Grid Column Width (in characters)');
                    var $colwidthtextcell = $table.CswTable('cell', 3, 2);
                    var $colwidthtextbox = $colwidthtextcell.CswInput('init', 
                                                                { ID: o.ID + '_gcwtb',
                                                                  type: CswInput_Types.text,
                                                                  onChange: function () {
                                                                      var $this = $(this);
                                                                      viewNodeData.width = $this.val();
                                                                  }
                    });
                    $colwidthtextbox.val(viewNodeData.width);
                }
            });

            // Filter
            $content.find('.' + viewEditClasses.vieweditor_viewfilterlink.name).click(function () {
                var $a = $(this);
                $cell.empty();
                //$cell.append('For ' + $a.text());
                var objHelper = new Csw.object(currentViewJson);
                var arbitraryId = $a.CswAttrDom('arbid');
                var viewNodeData = objHelper.find('arbitraryid', arbitraryId);

                var $table = $cell.CswTable({ 'ID': o.ID + '_editfilt', 'FirstCellRightAlign': true });
                $table.CswTable('cell', 1, 1).append('Case Sensitive');
                var $casecheck = $table.CswTable('cell', 1, 2)
                                       .CswInput('init', 
                                            { ID: o.ID + '_casecb',
                                              type: CswInput_Types.checkbox,
                                              onChange: function () {
                                                  var $this = $(this);
                                                  viewNodeData.casesensitive = $this.is(':checked');
                                              }
                                       });
                if (Csw.bool(viewNodeData.casesensitive)) {
                    $casecheck.CswAttrDom('checked', 'true');
                }
            });
        }
        
        function _makeViewTree(stepno, $content) {
            var $tree = $content;
            if (Csw.isNullOrEmpty($tree)) {
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
            
            if (stepno >= CswViewEditor_WizardSteps.relationships.step && stepno <= CswViewEditor_WizardSteps.filters.step) {
                bindDeleteBtns(stepno);
            }
            
            if (stepno === CswViewEditor_WizardSteps.filters.step) {
                bindViewPropFilterBtns(stepno);
            }
            
            if (stepno === CswViewEditor_WizardSteps.tuning.step) {
                makeTuningStep($tree);
            }
            return $tree;
        } // _makeViewTree()
        
        function bindDeleteBtns(stepno) {
            $('.' + viewEditClasses.vieweditor_deletespan.name).each(function () {
                var $span = $(this);
                var arbid = $span.CswAttrNonDom('arbid');
                var $btn = $span.children('div').first();
                $btn.bind('click', function () {
                    var objUtil = new Csw.object(currentViewJson);
                    objUtil.remove('arbitraryid', arbid);
                    _makeViewTree(stepno);
                    return CswImageButton_ButtonType.None;
                });
            });
        }
        
        function bindViewPropFilterBtns(stepno) {
            $('.' + viewEditClasses.vieweditor_addfilter.name).each(function () {
                var $span = $(this);
                var arbitraryId = $span.CswAttrNonDom('arbid');

                var $btn = $span.find('#' + arbitraryId + '_addfiltbtn');
                $btn.bind('click', function () {
                    var $this = $(this);
                    $this.CswButton('disable');
                    var objHelper = new Csw.object(currentViewJson);
                
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
                        onSuccess: function (newPropJson) {
                            if(false === propJson.hasOwnProperty(childPropNames.propfilters.name)) {
                                propJson[childPropNames.propfilters.name] = { };
                            }
                            $.extend(propJson[childPropNames.propfilters.name], newPropJson);
                            _makeViewTree(stepno);
                        } // onSuccess
                    }); // CswViewPropFilter
                });

                $span.find('.ViewPropFilterLogical').each(function () {
                    var $this = $(this);
                    $this.CswTristateCheckBox('reBindClick');
                });
                
            }); 
        }
        
        function viewJsonHtml(stepno, viewJson) {
            var types = { };
            var $ret = $('<ul></ul>');
            /* Root */
            makeViewRootHtml(stepno, viewJson, types)
                            .appendTo($ret);

            return { html: $ret.html(), types: types };
        }
        
        function makeViewRootHtml(stepno, itemJson, types) {
            var arbid = 'root';
            var name = itemJson.viewname;
            var rel = 'root';
            types.root = { icon: { image: Csw.string(itemJson.iconfilename) } };
            var linkclass = viewEditClasses.vieweditor_viewrootlink.name;

            var $ret = makeViewListItem(arbid, linkclass, name, false, stepno, childPropNames.root, rel);
            
            if(itemJson.hasOwnProperty(childPropNames.childrelationships.name)) {
                var rootRelationships = itemJson[childPropNames.childrelationships.name];
                makeViewRelationshipsRecursive(stepno, rootRelationships, types, $ret);
            }
            
            var $selectLi = makeChildSelect(stepno, arbid, childPropNames.childrelationships);
            if (false === Csw.isNullOrEmpty($selectLi)) {
                $ret.append($selectLi);
            }

            return $ret;
        }
        
        function makeViewRelationshipHtml(stepno, itemJson, types) {
            var arbid = itemJson.arbitraryid;
            //var nodename = itemJson.nodename;
            var name = itemJson.secondname;
            var propname = Csw.string(itemJson.propname);
            if (!Csw.isNullOrEmpty(propname)) {
                if (itemJson.propowner === "First") {
                    name += " (by " + itemJson.firstname + "'s " + propname + ")";
                } else {
                    name += " (by " + propname + ")";
                }
            }
            var rel = Csw.string(itemJson.secondtype) + '_' + Csw.string(itemJson.secondid);
            var skipchildoptions = (stepno <= CswViewEditor_WizardSteps.relationships.step);
            var linkclass = viewEditClasses.vieweditor_viewrellink.name;
            var showDelete = (stepno === CswViewEditor_WizardSteps.relationships.step);
            types[rel] = { icon: { image: Csw.string(itemJson.secondiconfilename) } };
            
            var $ret = makeViewListItem(arbid, linkclass, name, showDelete, stepno, childPropNames.childrelationships, rel);
            
            if (!skipchildoptions)
            {
                if(itemJson.hasOwnProperty(childPropNames.properties.name)) {
                    var propJson = itemJson[childPropNames.properties.name];
                    if (!Csw.isNullOrEmpty(propJson)) {
                        var $propUl = $('<ul></ul>');
                        for (var prop in propJson) {
                            if (propJson.hasOwnProperty(prop)) {
                                var thisProp = propJson[prop];
                                if(false === Csw.isNullOrEmpty(thisProp)) {
                                    var $propLi = makeViewPropertyHtml(thisProp, types, stepno);
                                    if (false === Csw.isNullOrEmpty($propLi)) {
                                        $propUl.append($propLi);
                                    }
                                }
                            }
                        }
                        if ($propUl.children().length > 0 ) {
                            $ret.append($propUl);
                        }
                    }
                }
                var $selectLi = makeChildSelect(stepno, arbid, childPropNames.properties);
                if (false === Csw.isNullOrEmpty($selectLi)) {
                    $ret.append($selectLi);
                }
            }
            return $ret;                
        }
        
        function makeViewRelationshipsRecursive(stepno, relationshipJson, types, $content) {
            if (!Csw.isNullOrEmpty(relationshipJson)) {
                var $ul = $('<ul></ul>');
                for (var relationship in relationshipJson) {
                    if (relationshipJson.hasOwnProperty(relationship)) {
                        var thisRelationship = relationshipJson[relationship];
                        var $rel = makeViewRelationshipHtml(stepno, thisRelationship, types);
                        if (false === Csw.isNullOrEmpty($rel)) {
                            $ul.append($rel);
                        }
                        if(thisRelationship.hasOwnProperty(childPropNames.childrelationships.name)) {
                            var childRelationships = thisRelationship[childPropNames.childrelationships.name];
                            makeViewRelationshipsRecursive(stepno, childRelationships, types, $rel);
                        }
                        if (stepno === CswViewEditor_WizardSteps.relationships.step) {
                            var $selectLi = makeChildSelect(stepno, thisRelationship.arbitraryid, childPropNames.childrelationships);
                            if (false === Csw.isNullOrEmpty($selectLi)) {
                                $rel.append($selectLi);
                            }
                        }
                    }
                }
                if ($ul.children().length > 0) {
                    $content.append($ul);
                }
            }
        }
        
        function makeViewPropertyHtml(itemJson, types, stepno) {
            var $ret = $('<li></li>');
            var arbid = itemJson.arbitraryid;
            //var nodename = itemJson.nodename;
            var name = itemJson.name;
            var rel = 'property';
            var skipme = (stepno <= CswViewEditor_WizardSteps.relationships.step);
            var skipchildoptions = (stepno <= CswViewEditor_WizardSteps.properties.step);
            var linkclass = viewEditClasses.vieweditor_viewproplink.name;
            var showDelete = (stepno === CswViewEditor_WizardSteps.properties.step);
            if (false === Csw.isNullOrEmpty(name) && false === skipme) {
                $ret = makeViewListItem(arbid, linkclass, name, showDelete, stepno, childPropNames.properties, rel);
            }
            if (!Csw.isNullOrEmpty($ret) && !skipchildoptions) {
                var $filtUl = $('<ul></ul>');
                if (itemJson.hasOwnProperty(childPropNames.propfilters.name)) {
                    var filterJson = itemJson[childPropNames.propfilters.name];
                    if (!Csw.isNullOrEmpty(filterJson)) {
                        for (var filter in filterJson) {
                            if (filterJson.hasOwnProperty(filter)) {
                                var thisFilt = filterJson[filter];
                                if (false === Csw.isNullOrEmpty(thisFilt)) {
                                    var $filtLi = makeViewPropertyFilterHtml(thisFilt, stepno, types, arbid);
                                    if (false === Csw.isNullOrEmpty($filtLi)) {
                                        $filtUl.append($filtLi);
                                    }
                                }
                            }
                        }
                    }
                }
                if (stepno !== CswViewEditor_WizardSteps.tuning.step) {
                    var $filtBuilderLi = makeViewPropertyFilterHtml(null, stepno, types, arbid);
                    if (false === Csw.isNullOrEmpty($filtBuilderLi)) {
                        $filtUl.append($filtBuilderLi);
                    }
                }
                if ($filtUl.children().length > 0) {
                    $ret.append($filtUl);
                }
            }
            types.property = { icon: { image: "Images/view/property.gif" } };
            return $ret;
        }
        
        function makeViewPropertyFilterHtml(itemJson, stepno, types, propArbId) {
            var $ret = null;
            if (stepno >= CswViewEditor_WizardSteps.filters.step) {
                $ret = $('<li></li>');
                var rel = 'filter';
                if (!Csw.isNullOrEmpty(itemJson)) {
                    var filtArbitraryId = Csw.string(itemJson.arbitraryid);
                    if (stepno === CswViewEditor_WizardSteps.tuning.step) {
                        var selectedSubfield = Csw.string(itemJson.subfield, itemJson.subfieldname);
                        var selectedFilterMode = Csw.string(itemJson.filtermode);
                        var filterValue = Csw.string(itemJson.value);
                        var name = selectedSubfield + ' ' + selectedFilterMode + ' ' + filterValue;
                        var $filtLink = makeViewListItem(filtArbitraryId, viewEditClasses.vieweditor_viewfilterlink.name, name, false, stepno, childPropNames.filters, rel);
                        if (false === Csw.isNullOrEmpty($filtLink)) {
                            $ret = $filtLink;
                        }
                    } else {
                        $ret.append(makeViewPropFilterStaticSpan(propArbId, itemJson, filtArbitraryId, rel));
                        $ret.append(makeDeleteSpan(filtArbitraryId, stepno));
                    }
                } else {
                    $ret.append(makeViewPropFilterAddSpan(propArbId, rel));                    
                }
            }
            types.filter = { icon: { image: "Images/view/filter.gif" } };
            return $ret;            
        }

        function makeViewPropFilterStaticSpan(propArbId, filterJson, filtArbitraryId) {
            var $span = $('<span class="' + viewEditClasses.vieweditor_addfilter.name + '" arbid="' + filtArbitraryId + '"></span>');
            var $tbl = $span.CswTable({ 'ID': o.ID + '_' + filtArbitraryId + '_propfilttbl' });
            $tbl.css('display', 'inline-table');

            $tbl.CswViewPropFilter('static', {
                ID: o.ID + '_' + filtArbitraryId + '_propfilttbl',
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
            var $span = $('<span class="' + viewEditClasses.vieweditor_addfilter.name + '" arbid="' + propArbId + '"></span>');
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
        
        function makeViewListItem(arbid, linkclass, name, showDelete, stepno, propName, rel) {
            var $ret = $('<li id="' + arbid + '" rel="' + rel + '" class="jstree-open"></li>');
            $ret.append($('<a href="#" class="' + linkclass + '" arbid="' + arbid + '">' + name + '</a>'));
            if (showDelete) {
                $ret.append(makeDeleteSpan(arbid, stepno));
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

                Csw.ajax.post({
                        url: o.ChildOptionsUrl,
                        data: dataJson,
                        success: function (data)
                        {
                            var $successSelect = $('#' + stepno + '_' + arbid + '_child');
                            $successSelect.empty();
                            $successSelect.append('<option value="">Select...</option>');
                            for (var optionName in data) {
                                if (data.hasOwnProperty(optionName)) {
                                    var thisOpt = data[optionName];
                                    var dataOpt = { };
                                    dataOpt[optionName] = thisOpt;
                                    var $option = $('<option value="' + thisOpt.arbitraryid + '">' + optionName + '</option>')
                                        .appendTo($successSelect);
                                    $option.data('thisViewJson', dataOpt);
                                }
                            }
                            
                            $successSelect.change(function () {
                                var $this = $(this);
                                var childJson = $this.find('option:selected').data('thisViewJson');
                                if (arbid === "root") {
                                    $.extend(currentViewJson.childrelationships, childJson);
                                } else {
                                    var objUtil = new Csw.object(currentViewJson);
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
                                    if (Csw.isNullOrEmpty(objCollection)) {
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

