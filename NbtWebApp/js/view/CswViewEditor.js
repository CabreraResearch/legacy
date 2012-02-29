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

        var WizardStepArray = [Csw.enums.wizardSteps_ViewEditor.viewselect, Csw.enums.wizardSteps_ViewEditor.attributes, Csw.enums.wizardSteps_ViewEditor.relationships,
            Csw.enums.wizardSteps_ViewEditor.properties, Csw.enums.wizardSteps_ViewEditor.filters, Csw.enums.wizardSteps_ViewEditor.tuning];
        var WizardSteps = {};
        for (var i = 1; i <= WizardStepArray.length; i++) {
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
        if (o.startingStep === 1) {
            $wizard.CswWizard('button', 'finish', 'disable');
        }
        // Step 1 - Choose a View
        var $div1 = $($wizard.CswWizard('div', Csw.enums.wizardSteps_ViewEditor.viewselect.step));
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

        var div1BtnTable = Csw.controls.table({
            $parent: $div1,
            ID: o.ID + '_1_btntbl',
            width: '100%'
        });
        var div1BtnTblCell11 = div1BtnTable.cell(1, 1);
        var $allcheck_div = $('<div></div>');
        div1BtnTable.cell(1, 2)
            .append($allcheck_div)
            .propDom('align', 'right');

        Csw.clientSession.isAdministrator({
            'Yes': function () {
                /* Show Other */
                $allcheck_div.CswInput('init', { ID: o.ID + '_all',
                    type: Csw.enums.inputTypes.checkbox,
                    onChange: function () {
                        _getViewsGrid();
                    }
                });
                $allcheck_div.append('Show Other Roles/Users');
            }
        });

        var copyViewBtn = div1BtnTblCell11.button({
            'ID': o.ID + '_copyview',
            'enabledText': 'Copy View',
            'disableOnClick': true,
            'onClick': function () {
                var viewid = _getSelectedViewId();
                if (false === Csw.isNullOrEmpty(viewid)) {
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
                            copyViewBtn.enable();
                        }
                    });
                } // if(viewid !== '' && viewid !== undefined)
            } // onClick
        }); // copy button
        copyViewBtn.disable();

        var deleteViewBtn = div1BtnTblCell11.button({
            ID: o.ID + '_deleteview',
            enabledText: 'Delete View',
            disableOnClick: true,
            onClick: function () {
                var viewid = _getSelectedViewId();
                if (false === Csw.isNullOrEmpty(viewid)) {
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
                                copyViewBtn.disable();
                            },
                            error: function () {
                                deleteViewBtn.enable();
                            }
                        });
                    }
                }
            } // onClick
        }); // delete button
        deleteViewBtn.disable();

        var newViewBtn = div1BtnTblCell11.button({
            'ID': o.ID + '_newview',
            'enabledText': 'Create New View',
            'disableOnClick': false,
            'onClick': function () {
                $.CswDialog('AddViewDialog', {
                    onAddView: function (newviewid) {
                        _getViewsGrid(newviewid);
                    },
                    onClose: function () {
                        newViewBtn.enable();
                    }
                }); // CswDialog
            } // onClick
        });

        //$wizard.CswWizard('button', 'next', 'disable');

        // Step 2 - Edit View Attributes
        var $div2 = $wizard.CswWizard('div', Csw.enums.wizardSteps_ViewEditor.attributes.step);
        var table2 = Csw.controls.table({
            $parent: $div2,
            ID: o.ID + '_tbl2',
            FirstCellRightAlign: true
        });

        table2.cell(1, 1).text('View Name:');
        var viewNameTextBox = table2.cell(1, 2).input({ ID: o.ID + '_viewname',
            type: Csw.enums.inputTypes.text
        });

        table2.cell(2, 1).text('Category:');
        var categoryTextBox = table2.cell(2, 2).input({ ID: o.ID + '_category',
            type: Csw.enums.inputTypes.text
        });

        var visSelect;
        // we don't have json to see whether this is a Property view or not yet,
        // so checking startingStep will have to suffice
        if (o.startingStep === 1) {
            visSelect = Csw.makeViewVisibilitySelect(table2, 3, 'View Visibility:');
        }

        table2.cell(4, 1).text('For Mobile:');
        var forMobileCheckBox = table2.cell(4, 2)
                                      .input({ ID: o.ID + '_formobile',
                                          type: Csw.enums.inputTypes.checkbox
                                      });

        table2.cell(5, 1).text('Display Mode:');
        var displayModeSpan = table2.cell(5, 2)
            .span({ ID: o.ID + '_displaymode' });

        var gridWidthLabelCell = table2.cell(6, 1).text('Grid Width (in characters):');
        var gridWidthTextBox = table2.cell(6, 2)
            .numberTextBox({
                ID: o.ID + '_gridwidth',
                value: '',
                MinValue: '1',
                MaxValue: '',
                Precision: '0'
            });

        // Step 3 - Add Relationships
        var $div3 = $wizard.CswWizard('div', Csw.enums.wizardSteps_ViewEditor.relationships.step);
        $div3.append('Add relationships from the select boxes below:<br/><br/>');
        var $treediv3 = $('<div id="' + Csw.enums.wizardSteps_ViewEditor.relationships.divId + '"><div/>').appendTo($div3);

        // Step 4 - Select Properties
        var $div4 = $wizard.CswWizard('div', Csw.enums.wizardSteps_ViewEditor.properties.step);
        $div4.append('Add properties from the select boxes below:<br/><br/>');
        var $treediv4 = $('<div id="' + Csw.enums.wizardSteps_ViewEditor.properties.divId + '"><div/>').appendTo($div4);

        // Step 5 - Set Filters
        var $div5 = $wizard.CswWizard('div', Csw.enums.wizardSteps_ViewEditor.filters.step);
        $div5.append('Add filters by selecting properties from the tree:<br/><br/>');
        var $treediv5 = $('<div id="' + Csw.enums.wizardSteps_ViewEditor.filters.divId + '"><div/>').appendTo($div5);

        // Step 6 - Fine Tuning
        var $div6 = $wizard.CswWizard('div', Csw.enums.wizardSteps_ViewEditor.tuning.step);
        $div6.append('Select what you want to edit from the tree:<br/><br/>');
        var table6 = Csw.controls.table({
            $parent: $div6,
            'ID': o.ID + '_6_tbl'
        });

        var currentViewJson;

        function _onBeforePrevious($prevWizard, stepno) {
            /* remember: confirm is globally blocking call */
            return (stepno !== Csw.enums.wizardSteps_ViewEditor.attributes.step || confirm("You will lose any changes made to the current view if you continue.  Are you sure?"));
        }

        function _handleNext(table, newstepno) {
            var $nextWizard = table.$;
            CurrentStep = newstepno;
            switch (newstepno) {
                case Csw.enums.wizardSteps_ViewEditor.viewselect.step:
                    break;
                case Csw.enums.wizardSteps_ViewEditor.attributes.step:
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

                            viewNameTextBox.val(currentViewJson.viewname);
                            categoryTextBox.val(currentViewJson.category);
                            var visibility = Csw.string(currentViewJson.visibility);
                            if (visibility !== 'Property') {
                                if (visSelect.$visibilityselect !== undefined) {
                                    visSelect.$visibilityselect.val(visibility).trigger('change');
                                    visSelect.$visroleselect.val('nodes_' + currentViewJson.visibilityroleid);
                                    visSelect.$visuserselect.val('nodes_' + currentViewJson.visibilityuserid);
                                }
                            }

                            if (Csw.bool(currentViewJson.formobile)) {
                                forMobileCheckBox.propDom('checked', 'checked');
                            }
                            var mode = currentViewJson.mode;
                            displayModeSpan.text(mode);
                            gridWidthTextBox.val(currentViewJson.width);
                            if (mode === "Grid") {
                                gridWidthLabelCell.show();
                                gridWidthTextBox.show();
                            } else {
                                gridWidthLabelCell.hide();
                                gridWidthTextBox.hide();
                            }

                            $nextWizard.CswWizard('button', 'next', 'enable');
                        } // success
                    }); // ajax
                    break;
                case Csw.enums.wizardSteps_ViewEditor.relationships.step:
                    // save step 2 content to currentviewjson
                    if (currentViewJson !== undefined) {
                        cacheStepTwo();
                    } // if(currentViewJson !== undefined)

                    // make step 3 tree
                    _makeViewTree(Csw.enums.wizardSteps_ViewEditor.relationships.step, $treediv3);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.properties.step:
                    _makeViewTree(Csw.enums.wizardSteps_ViewEditor.properties.step, $treediv4);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.filters.step:
                    _makeViewTree(Csw.enums.wizardSteps_ViewEditor.filters.step, $treediv5);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.tuning.step:
                    _makeViewTree(Csw.enums.wizardSteps_ViewEditor.tuning.step, table6.cell(1, 1).$);
                    break;
            } // switch(newstepno)
        } // _handleNext()

        function cacheStepTwo() {
            currentViewJson.viewname = viewNameTextBox.val();
            currentViewJson.category = categoryTextBox.val();
            if (currentViewJson.visibility !== 'Property') {
                if (visSelect.$visibilityselect !== undefined) {
                    var visibility = visSelect.$visibilityselect.val();
                    currentViewJson.visibility = visibility;

                    var rolenodeid = '';
                    if (visibility === 'Role') {
                        rolenodeid = visSelect.$visroleselect.val();
                        if (!Csw.isNullOrEmpty(rolenodeid)) {
                            rolenodeid = rolenodeid.substr('nodes_'.length);
                        }
                    }
                    currentViewJson.visibilityroleid = rolenodeid;

                    var usernodeid = '';
                    if (visibility === 'User') {
                        usernodeid = visSelect.$visuserselect.val();
                        if (!Csw.isNullOrEmpty(usernodeid)) {
                            usernodeid = usernodeid.substr('nodes_'.length);
                        }
                    }
                    currentViewJson.visibilityuserid = usernodeid;
                }
            }
            var formobile = (forMobileCheckBox.$.is(':checked') ? 'true' : 'false');
            currentViewJson.formobile = formobile;
            currentViewJson.width = gridWidthTextBox.val();
        }

        function _handlePrevious($wizard, newstepno) {
            if (newstepno === 1)
                $wizard.CswWizard('button', 'finish', 'disable');

            CurrentStep = newstepno;
            switch (newstepno) {
                case Csw.enums.wizardSteps_ViewEditor.viewselect.step:
                    break;
                case Csw.enums.wizardSteps_ViewEditor.attributes.step:
                    break;
                case Csw.enums.wizardSteps_ViewEditor.relationships.step:
                    _makeViewTree(Csw.enums.wizardSteps_ViewEditor.relationships.step, $treediv3);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.properties.step:
                    _makeViewTree(Csw.enums.wizardSteps_ViewEditor.properties.step, $treediv4);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.filters.step:
                    _makeViewTree(Csw.enums.wizardSteps_ViewEditor.filters.step, $treediv5);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.tuning.step:
                    _makeViewTree(Csw.enums.wizardSteps_ViewEditor.tuning.step, table6.cell(1, 1).$);
                    break;
            }
        }

        function gridHasOneProp() {
            var ret = false;
            if (Csw.contains(currentViewJson, 'childrelationships')) {
                Csw.crawlObject(currentViewJson.childrelationships, function (childObj) {
                    if (ret) {
                        return false;
                    }
                    else if (Csw.contains(childObj, 'properties')) {
                        Csw.crawlObject(childObj.properties, function (propObj) {
                            if (false === Csw.isNullOrUndefined(propObj)) {
                                ret = true;
                                return false;
                            }
                        }, false);
                    }
                }, true);
            }
            return ret;
        }

        function _handleFinish(finishWizard) {
            var viewid = _getSelectedViewId();
            var processView = true;

            if (!Csw.isNullOrEmpty(currentViewJson)) {
                if (CurrentStep === Csw.enums.wizardSteps_ViewEditor.attributes.step) {
                    cacheStepTwo();
                }

                if (currentViewJson.mode === 'Grid' && false === gridHasOneProp()) {
                    processView = confirm('You are attempting to create a Grid without properties. This will not display any information. Do you want to continue?');
                    if (false === processView) {
                        finishWizard.$.CswWizard('button', 'finish', 'enable');
                    }
                }
            }

            if (processView) {
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

        function _getViewsGrid(selectedviewid) {
            var all = ($('#' + o.ID + '_all:checked').length > 0);
            $selview_span.text('');
            if (o.startingStep === 1) {
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
                                    if (selected) {
                                        copyViewBtn.enable();
                                        deleteViewBtn.enable();
                                        $selview_span.text(_getSelectedViewName(id));
                                        $wizard.CswWizard('button', 'next', 'enable');
                                    } else {
                                        copyViewBtn.disable();
                                        deleteViewBtn.disable();
                                        $selview_span.text("");
                                        $wizard.CswWizard('button', 'next', 'disable');
                                    }
                                }
                            }
                        };
                        $.extend(g.gridOpts, gridJson);
                        g.$parent = $viewgrid;
                        cswViewGrid = Csw.controls.grid(g);
                        cswViewGrid.gridPager.css({ width: '100%', height: '20px' });

                        cswViewGrid.hideColumn(o.ColumnFullViewId);
                        if (false === Csw.isNullOrEmpty(gridJson.selectedpk)) {
                            rowid = cswViewGrid.getRowIdForVal(gridJson.selectedpk, o.ColumnViewId);
                            cswViewGrid.setSelection(rowid);
                            cswViewGrid.scrollToRow(rowid);
                        }
                    } // success
                }); // ajax
            }
        } // _getViewsGrid()

        function _getSelectedViewId(selRowId) {
            var ret;
            if (o.startingStep === 1) {
                ret = cswViewGrid.getValueForColumn(o.ColumnFullViewId, selRowId);
            } else {
                ret = o.viewid;
            }
            return ret;
        }

        function _getSelectedViewMode(selRowId) {
            var ret;
            if (o.startingStep === 1) {
                ret = cswViewGrid.getValueForColumn(o.ColumnViewMode, selRowId);
            } else {
                ret = o.viewmode;
            }
            return ret;
        }

        function _getSelectedViewName(selRowId) {
            var ret;
            if (o.startingStep === 1) {
                ret = cswViewGrid.getValueForColumn(o.ColumnViewName, selRowId);
            } else {
                ret = o.viewname;
            }
            return ret;
        }

        function makeTuningStep($content) {
            var cell = table6.cell(1, 2);
            var viewmode = _getSelectedViewMode();

            // Root
            $content.find('.' + Csw.enums.cssClasses_ViewEdit.vieweditor_viewrootlink.name).click(function () {
                cell.empty();
            });

            // Relationship
            $content.find('.' + Csw.enums.cssClasses_ViewEdit.vieweditor_viewrellink.name).click(function () {
                var row = 1;
                var $a = $(this);
                cell.empty();

                var objHelper = Csw.object(currentViewJson);
                var arbitraryId = $a.CswAttrDom('arbid');
                var viewnodejson = objHelper.find('arbitraryid', arbitraryId);

                var subTable = table6.cell(1, 2).table({
                    ID: Csw.controls.dom.makeId(o.ID, 'editrel'),
                    FirstCellRightAlign: true
                });

                function _makeAllowCB(row, idsuffix, text, checked, onChange) {
                    subTable.cell(row, 1).text('Allow ' + text);
                    var allowCell = subTable.cell(row, 2);
                    var $allowcheck = allowCell.$.CswInput('init', {
                        ID: o.ID + '_adcb',
                        type: Csw.enums.inputTypes.checkbox,
                        onChange: function () {
                            var $this = $(this);
                            if (isFunction(onChange)) {
                                onChange($this.is(':checked'));
                            }
                        }
                    });

                    if (checked) {
                        $allowcheck.CswAttrDom('checked', 'true');
                    }
                } // makeAllowCB()

                _makeAllowCB(row, 'editrel_view', 'View', Csw.bool(viewnodejson.allowview), function (checked) { viewnodejson.allowview = checked; });
                row += 1;
                _makeAllowCB(row, 'editrel_edit', 'Edit', Csw.bool(viewnodejson.allowedit), function (checked) { viewnodejson.allowedit = checked; });
                row += 1;
                _makeAllowCB(row, 'editrel_del', 'Delete', Csw.bool(viewnodejson.allowdelete), function (checked) { viewnodejson.allowdelete = checked; });
                row += 1;

                subTable.cell(row, 1).text('Group By');
                var groupBySelect = subTable.cell(row, 2)
                                            .select({ ID: o.ID + '_gbs',
                                                onChange: function () {
                                                    var selected = groupBySelect.find(':selected');
                                                    var selval = selected.val();
                                                    var propData;

                                                    if (false === Csw.isNullOrEmpty(selval)) {
                                                        viewnodejson.groupbypropid = Csw.string(selected.val());
                                                        viewnodejson.groupbyproptype = Csw.string(selected.propNonDom('propType'));
                                                        viewnodejson.groupbypropname = Csw.string(selected.text());
                                                    } // if (false === Csw.isNullOrEmpty(selval)) {
                                                } // onChange
                                            }); // CswSelect
                row += 1;

                var jsonData = {
                    Type: viewnodejson.secondtype,
                    Id: viewnodejson.secondid
                };
                Csw.ajax.post({
                    url: o.PropNamesUrl,
                    data: jsonData,
                    success: function (data) {
                        groupBySelect.empty();
                        groupBySelect.option({ value: 'None' });
                        for (var propKey in data) {
                            if (Csw.contains(data, propKey)) {
                                var thisProp = data[propKey];
                                var isSelected = (viewnodejson.groupbypropid === thisProp.propid &&
                                                  viewnodejson.groupbyproptype === thisProp.proptype &&
                                                  viewnodejson.groupbypropname === thisProp.propname);
                                groupBySelect.option({
                                    value: thisProp.propid,
                                    display: thisProp.propname,
                                    isSelected: isSelected
                                }).propNonDom({
                                    propType: thisProp.proptype
                                });
                            }
                        } // each
                    } // success
                }); // ajax

                var $showtreecheck;
                if (viewmode === "Tree") {
                    subTable.cell(row, 1).text('Show In Tree');
                    var showTreeCheckCell = subTable.cell(row, 2);
                    $showtreecheck = showTreeCheckCell.$.CswInput('init',
                                                            { ID: o.ID + '_stcb',
                                                                type: Csw.enums.inputTypes.checkbox,
                                                                onChange: function () {
                                                                    var $this = $(this);
                                                                    viewnodejson.showintree = $this.is(':checked');
                                                                }
                                                            });
                    if (Csw.bool(viewnodejson.showintree)) {
                        $showtreecheck.CswAttrDom('checked', 'true');
                    }
                }
                row += 1;

            }); // $content.find('.vieweditor_viewrellink').click(function () {

            // Property
            $content.find('.' + Csw.enums.cssClasses_ViewEdit.vieweditor_viewproplink.name).click(function () {
                var $a = $(this);
                cell.empty();

                if (viewmode === "Grid") {
                    var objHelper = Csw.object(currentViewJson);
                    var arbitraryId = $a.CswAttrDom('arbid');
                    var viewNodeData = objHelper.find('arbitraryid', arbitraryId);

                    var gridTable = table6.cell(1, 2).table({
                        ID: o.ID + '_editprop',
                        FirstCellRightAlign: true
                    });

                    gridTable.cell(1, 1).text('Sort By');
                    var sortByCheckCell = gridTable.cell(1, 2);
                    var $sortbycheck = sortByCheckCell.$.CswInput('init',
                                                            { ID: o.ID + '_sortcb',
                                                                type: Csw.enums.inputTypes.checkbox,
                                                                onChange: function () {
                                                                    var $this = $(this);
                                                                    viewNodeData.sortby = $this.is(':checked');
                                                                }
                                                            });
                    if (Csw.bool(viewNodeData.sortby)) {
                        $sortbycheck.CswAttrDom('checked', 'true');
                    }

                    gridTable.cell(2, 1).text('Grid Column Order');
                    var colOrderTextCell = gridTable.cell(2, 2);
                    var $colordertextbox = colOrderTextCell.$.CswInput('init',
                                                                { ID: o.ID + '_gcotb',
                                                                    type: Csw.enums.inputTypes.text,
                                                                    onChange: function () {
                                                                        var $this = $(this);
                                                                        viewNodeData.order = $this.val();
                                                                    }
                                                                });
                    $colordertextbox.val(viewNodeData.order);

                    gridTable.cell(3, 1).text('Grid Column Width (in characters)');
                    var colWidthTextCell = gridTable.cell(3, 2);
                    var $colwidthtextbox = colWidthTextCell.$.CswInput('init',
                                                                { ID: o.ID + '_gcwtb',
                                                                    type: Csw.enums.inputTypes.text,
                                                                    onChange: function () {
                                                                        var $this = $(this);
                                                                        viewNodeData.width = $this.val();
                                                                    }
                                                                });
                    $colwidthtextbox.val(viewNodeData.width);
                }
            });

            // Filter
            $content.find('.' + Csw.enums.cssClasses_ViewEdit.vieweditor_viewfilterlink.name).click(function () {
                var $a = $(this);
                cell.empty();
                //$cell.append('For ' + $a.text());
                var objHelper = Csw.object(currentViewJson);
                var arbitraryId = $a.CswAttrDom('arbid');
                var viewNodeData = objHelper.find('arbitraryid', arbitraryId);

                var filterTable = table6.cell(1, 2).table({
                    'ID': o.ID + '_editfilt',
                    'FirstCellRightAlign': true
                });
                filterTable.cell(1, 1).text('Case Sensitive');

                filterTable.cell(1, 2)
                    .input({
                        ID: o.ID + '_casecb',
                        type: Csw.enums.inputTypes.checkbox,
                        onChange: function () {
                            var $this = $(this);
                            viewNodeData.casesensitive = $this.is(':checked');
                        },
                        checked: Csw.bool(viewNodeData.casesensitive)
                    });
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

            if (stepno >= Csw.enums.wizardSteps_ViewEditor.relationships.step && stepno <= Csw.enums.wizardSteps_ViewEditor.filters.step) {
                bindDeleteBtns(stepno);
            }

            if (stepno === Csw.enums.wizardSteps_ViewEditor.filters.step) {
                bindViewPropFilterBtns(stepno);
            }

            if (stepno === Csw.enums.wizardSteps_ViewEditor.tuning.step) {
                makeTuningStep($tree);
            }
            return $tree;
        } // _makeViewTree()

        function bindDeleteBtns(stepno) {
            $('.' + Csw.enums.cssClasses_ViewEdit.vieweditor_deletespan.name).each(function () {
                var $span = $(this);
                var arbid = $span.CswAttrNonDom('arbid');
                var $btn = $span.children('div').first();
                $btn.bind('click', function () {
                    var objUtil = Csw.object(currentViewJson);
                    objUtil.remove('arbitraryid', arbid);
                    _makeViewTree(stepno);
                    return Csw.enums.imageButton_ButtonType.None;
                });
            });
        }

        function bindViewPropFilterBtns(stepno) {
            $('.' + Csw.enums.cssClasses_ViewEdit.vieweditor_addfilter.name).each(function () {
                var $span = $(this);
                var arbitraryId = $span.CswAttrNonDom('arbid');

                var $btn = $span.find('#' + arbitraryId + '_addfiltbtn');
                $btn.bind('click', function () {
                    var $this = $(this);
                    $this.CswButton('disable');
                    var objHelper = Csw.object(currentViewJson);

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
                            if (false === propJson.hasOwnProperty(Csw.enums.viewChildPropNames.propfilters.name)) {
                                propJson[Csw.enums.viewChildPropNames.propfilters.name] = {};
                            }
                            $.extend(propJson[Csw.enums.viewChildPropNames.propfilters.name], newPropJson);
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
            var types = {};
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
            types.root = { icon: { image: Csw.string(itemJson.iconfilename)} };
            var linkclass = Csw.enums.cssClasses_ViewEdit.vieweditor_viewrootlink.name;

            var $ret = makeViewListItem(arbid, linkclass, name, false, stepno, Csw.enums.viewChildPropNames.root, rel);

            if (itemJson.hasOwnProperty(Csw.enums.viewChildPropNames.childrelationships.name)) {
                var rootRelationships = itemJson[Csw.enums.viewChildPropNames.childrelationships.name];
                makeViewRelationshipsRecursive(stepno, rootRelationships, types, $ret);
            }

            var $selectLi = makeChildSelect(stepno, arbid, Csw.enums.viewChildPropNames.childrelationships);
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
            var skipchildoptions = (stepno <= Csw.enums.wizardSteps_ViewEditor.relationships.step);
            var linkclass = Csw.enums.cssClasses_ViewEdit.vieweditor_viewrellink.name;
            var showDelete = (stepno === Csw.enums.wizardSteps_ViewEditor.relationships.step);
            types[rel] = { icon: { image: Csw.string(itemJson.secondiconfilename)} };

            var $ret = makeViewListItem(arbid, linkclass, name, showDelete, stepno, Csw.enums.viewChildPropNames.childrelationships, rel);

            if (!skipchildoptions) {
                if (itemJson.hasOwnProperty(Csw.enums.viewChildPropNames.properties.name)) {
                    var propJson = itemJson[Csw.enums.viewChildPropNames.properties.name];
                    if (!Csw.isNullOrEmpty(propJson)) {
                        var $propUl = $('<ul></ul>');
                        for (var prop in propJson) {
                            if (propJson.hasOwnProperty(prop)) {
                                var thisProp = propJson[prop];
                                if (false === Csw.isNullOrEmpty(thisProp)) {
                                    var $propLi = makeViewPropertyHtml(thisProp, types, stepno);
                                    if (false === Csw.isNullOrEmpty($propLi)) {
                                        $propUl.append($propLi);
                                    }
                                }
                            }
                        }
                        if ($propUl.children().length > 0) {
                            $ret.append($propUl);
                        }
                    }
                }
                var $selectLi = makeChildSelect(stepno, arbid, Csw.enums.viewChildPropNames.properties);
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
                        if (thisRelationship.hasOwnProperty(Csw.enums.viewChildPropNames.childrelationships.name)) {
                            var childRelationships = thisRelationship[Csw.enums.viewChildPropNames.childrelationships.name];
                            makeViewRelationshipsRecursive(stepno, childRelationships, types, $rel);
                        }
                        if (stepno === Csw.enums.wizardSteps_ViewEditor.relationships.step) {
                            var $selectLi = makeChildSelect(stepno, thisRelationship.arbitraryid, Csw.enums.viewChildPropNames.childrelationships);
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
            var skipme = (stepno <= Csw.enums.wizardSteps_ViewEditor.relationships.step);
            var skipchildoptions = (stepno <= Csw.enums.wizardSteps_ViewEditor.properties.step);
            var linkclass = Csw.enums.cssClasses_ViewEdit.vieweditor_viewproplink.name;
            var showDelete = (stepno === Csw.enums.wizardSteps_ViewEditor.properties.step);
            if (false === Csw.isNullOrEmpty(name) && false === skipme) {
                $ret = makeViewListItem(arbid, linkclass, name, showDelete, stepno, Csw.enums.viewChildPropNames.properties, rel);
            }
            if (!Csw.isNullOrEmpty($ret) && !skipchildoptions) {
                var $filtUl = $('<ul></ul>');
                /* Show existing read-only filters */
                if (itemJson.hasOwnProperty(Csw.enums.viewChildPropNames.propfilters.name)) {
                    var filterJson = itemJson[Csw.enums.viewChildPropNames.propfilters.name];
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
                /* Show add new filter */
                if (stepno !== Csw.enums.wizardSteps_ViewEditor.tuning.step) {
                    var $filtBuilderLi = makeViewPropertyFilterHtml(null, stepno, types, arbid);
                    if (false === Csw.isNullOrEmpty($filtBuilderLi)) {
                        $filtUl.append($filtBuilderLi);
                    }
                }
                if ($filtUl.children().length > 0) {
                    $ret.append($filtUl);
                }
            }
            types.property = { icon: { image: "Images/view/property.gif"} };
            return $ret;
        }

        function makeViewPropertyFilterHtml(itemJson, stepno, types, propArbId) {
            var $ret = null;
            if (stepno >= Csw.enums.wizardSteps_ViewEditor.filters.step) {
                $ret = $('<li></li>');
                var rel = 'filter';
                if (!Csw.isNullOrEmpty(itemJson)) {
                    var filtArbitraryId = Csw.string(itemJson.arbitraryid);
                    if (stepno === Csw.enums.wizardSteps_ViewEditor.tuning.step) {
                        var selectedSubfield = Csw.string(itemJson.subfield, itemJson.subfieldname);
                        var selectedFilterMode = Csw.string(itemJson.filtermode);
                        var filterValue = Csw.string(itemJson.value);
                        var name = selectedSubfield + ' ' + selectedFilterMode + ' ' + filterValue;
                        var $filtLink = makeViewListItem(filtArbitraryId, Csw.enums.cssClasses_ViewEdit.vieweditor_viewfilterlink.name, name, false, stepno, Csw.enums.viewChildPropNames.filters, rel);
                        if (false === Csw.isNullOrEmpty($filtLink)) {
                            $ret = $filtLink;
                        }
                    } else {
                        $ret.append(makeViewPropFilterStaticSpan(propArbId, itemJson, filtArbitraryId, rel));
                        makeDeleteSpan(filtArbitraryId, $ret);
                    }
                } else {
                    $ret.append(makeViewPropFilterAddSpan(propArbId, rel));
                }
            }
            types.filter = { icon: { image: "Images/view/filter.gif"} };
            return $ret;
        }

        function makeViewPropFilterStaticSpan(propArbId, filterJson, filtArbitraryId) {
            var $span = $('<span class="' + Csw.enums.cssClasses_ViewEdit.vieweditor_addfilter.name + '" arbid="' + filtArbitraryId + '"></span>');
            var table = Csw.controls.table({
                $parent: $span,
                'ID': o.ID + '_' + filtArbitraryId + '_propfilttbl'
            });
            table.css({ 'display': 'inline-table' });

            table.$.CswViewPropFilter('static', {
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
            var $span = $('<span class="' + Csw.enums.cssClasses_ViewEdit.vieweditor_addfilter.name + '" arbid="' + propArbId + '"></span>');
            var table = Csw.controls.table({
                $parent: $span,
                'ID': o.ID + '_' + propArbId + '_propfilttbl'
            });
            table.css({ 'display': 'inline-table' });

            table.$.CswViewPropFilter('init', {
                viewJson: currentViewJson,
                ID: o.ID + '_' + propArbId + '_propfilttbl',
                propsData: null,
                proparbitraryid: propArbId,
                propRow: 1,
                firstColumn: 1,
                includePropertyName: false,
                autoFocusInput: false
            });

            table.cell(1, 5).button({
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
                makeDeleteSpan(arbid, $ret);
            }
            return $ret;
        }

        function makeDeleteSpan(arbid, $parent) {
            var td = Csw.controls.span({
                $parent: $parent,
                cssclass: Csw.enums.cssClasses_ViewEdit.vieweditor_deletespan.name
            }).propNonDom('arbid', arbid);
            td.imageButton({
                ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                AlternateText: 'Delete',
                ID: arbid + '_delete'
            }).css({ display: 'inline-block' });
            return td.$;
        }

        function getTreeDiv(stepno) {
            var ret = '';
            switch (stepno) {
                case Csw.enums.wizardSteps_ViewEditor.relationships.step:
                    ret = $('#' + Csw.enums.wizardSteps_ViewEditor.relationships.divId);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.properties.step:
                    ret = $('#' + Csw.enums.wizardSteps_ViewEditor.properties.divId);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.filters.step:
                    ret = $('#' + Csw.enums.wizardSteps_ViewEditor.filters.divId);
                    break;
                case Csw.enums.wizardSteps_ViewEditor.tuning.step:
                    ret = $('#' + o.ID);
                    break;
            }
            return ret;
        }

        function makeChildSelect(stepno, arbid, propName) {
            var $select = '';

            if (canAddChildSelect(stepno, propName, arbid)) {
                $select = $('<ul><li><select id="' + stepno + '_' + arbid + '_child" arbid="' + arbid + '" class="' + Csw.enums.cssClasses_ViewEdit.vieweditor_childselect.name + '"></select></li></ul>');

                var dataJson = {
                    StepNo: stepno,
                    ArbitraryId: arbid,
                    ViewJson: JSON.stringify(currentViewJson)
                };

                Csw.ajax.post({
                    url: o.ChildOptionsUrl,
                    data: dataJson,
                    success: function (data) {
                        var $successSelect = $('#' + stepno + '_' + arbid + '_child');
                        $successSelect.empty();
                        $successSelect.append('<option value="">Select...</option>');
                        for (var optionName in data) {
                            if (data.hasOwnProperty(optionName)) {
                                var thisOpt = data[optionName];
                                var dataOpt = {};
                                dataOpt[optionName] = thisOpt;
                                $('<option value="' + thisOpt.arbitraryid + '">' + optionName + '</option>')
                                        .appendTo($successSelect);
                                $wizard.data(thisOpt.arbitraryid + '_thisViewJson', dataOpt);
                            }
                        }

                        $successSelect.change(function () {
                            var $this = $(this);
                            var $selected = $this.find('option:selected');
                            var childJson = $wizard.data($selected.val() + '_thisViewJson');
                            if (arbid === "root") {
                                $.extend(currentViewJson.childrelationships, childJson);
                            } else {
                                var objUtil = Csw.object(currentViewJson);
                                var parentObj = objUtil.find('arbitraryid', arbid);
                                var collection = '';
                                switch (stepno) {
                                    case Csw.enums.wizardSteps_ViewEditor.relationships.step:
                                        collection = Csw.enums.viewChildPropNames.childrelationships.name;
                                        break;
                                    case Csw.enums.wizardSteps_ViewEditor.properties.step:
                                        collection = Csw.enums.viewChildPropNames.properties.name;
                                        break;
                                }
                                var objCollection = parentObj[collection];
                                if (Csw.isNullOrEmpty(objCollection)) {
                                    objCollection = {};
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

        function canAddChildSelect(stepno, propName, arbid) {
            var ret = false;

            switch (stepno) {
                case Csw.enums.wizardSteps_ViewEditor.relationships.step:
                    if (propName === Csw.enums.viewChildPropNames.childrelationships || propName === Csw.enums.viewChildPropNames.root) {
                        ret = true;
                    }
                    break;
                case Csw.enums.wizardSteps_ViewEditor.properties.step:
                    if (propName === Csw.enums.viewChildPropNames.properties && arbid !== 'root') {
                        ret = true;
                    }
                    break;
                case Csw.enums.wizardSteps_ViewEditor.filters.step:
                    if (propName === Csw.enums.viewChildPropNames.propfilters && arbid !== 'root') {
                        ret = true;
                    }
                    break;
            }

            return ret;
        }

        return $div;

    }; // $.fn.CswViewEditor
})(jQuery);

