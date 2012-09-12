/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    $.fn.CswViewEditor = function (options) {
        var o = {
            ViewGridUrl: 'getViewGrid',
            ViewInfoUrl: 'getViewInfo',
            SaveViewUrl: 'saveViewInfo',
            CopyViewUrl: 'copyView',
            DeleteViewUrl: 'deleteView',
            ChildOptionsUrl: 'getViewChildOptions',
            PropNamesUrl: 'getPropNames',
            FiltersUrlMethod: 'getAllViewPropFilters',
            filtersData: {},
            newProps: [],
            viewid: '',
            viewname: '',
            viewmode: '',
            ID: 'vieweditor',
            ColumnViewName: 'viewname',
            ColumnViewId: 'nodeviewid',
            ColumnFullViewId: 'viewid',
            ColumnViewMode: 'viewmode',
            onCancel: null, // function ($wizard) {},
            onFinish: null, // function (viewid, viewmode) {},
            onDeleteView: null, // function (deletedviewid) {},
            onAddView: null,
            startingStep: 1
        };
        if (options) Csw.extend(o, options);

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
        } else {
            _initStepTwo($wizard);
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

        var div1BtnTable = Csw.literals.table({
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
                        urlMethod: o.CopyViewUrl,
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
                            urlMethod: o.DeleteViewUrl,
                            data: dataJson,
                            success: function () {
                                _getViewsGrid();
                                copyViewBtn.disable();
                                deleteViewBtn.disable();  // button reenables itself, so need to disable it again

                                Csw.tryExec(o.onDeleteView, viewid);
                            },
                            error: function () {
                                deleteViewBtn.enable();
                            }
                        });
                    } else {
                        deleteViewBtn.enable();
                    }
                } else {
                    deleteViewBtn.enable();
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
                    onAddView: function (newviewid, viewmode) {
                        _getViewsGrid(newviewid);
                        Csw.tryExec(o.onAddView, newviewid, viewmode);
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
        var table2 = Csw.literals.table({
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
            visSelect = Csw.controls.makeViewVisibilitySelect(table2, 3, 'View Visibility:');
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
        var table6 = Csw.literals.table({
            $parent: $div6,
            'ID': o.ID + '_6_tbl'
        });

        var currentViewJson;

        function _onBeforePrevious($prevWizard, stepno) {
            /* remember: confirm is globally blocking call */
            return (stepno !== Csw.enums.wizardSteps_ViewEditor.attributes.step || confirm("You will lose any changes made to the current view if you continue.  Are you sure?"));
        }

        function _handleNext($nextWizard, newstepno) {
            //var $nextWizard = table.$;
            CurrentStep = newstepno;
            switch (newstepno) {
                case Csw.enums.wizardSteps_ViewEditor.viewselect.step:
                    break;
                case Csw.enums.wizardSteps_ViewEditor.attributes.step:
                    _initStepTwo($nextWizard);

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

        function _initStepTwo($nextWizard) {
            $nextWizard.CswWizard('button', 'finish', 'enable');
            $nextWizard.CswWizard('button', 'next', 'disable');

            var jsonData = {
                ViewId: _getSelectedViewId()
            };

            Csw.ajax.post({
                urlMethod: o.ViewInfoUrl,
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
        } // _initStepTwo()

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
            CurrentStep = newstepno;
            switch (newstepno) {
                case Csw.enums.wizardSteps_ViewEditor.viewselect.step:
                    $wizard.CswWizard('button', 'finish', 'disable');
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
                    urlMethod: o.SaveViewUrl,
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
                    urlMethod: o.ViewGridUrl,
                    data: dataJson,
                    success: function (gridJson) {

                        if (Csw.isNullOrEmpty($viewgrid) || $viewgrid.length === 0) {
                            $viewgrid = $('<div id="' + o.ID + '_csw_viewGrid_outer"></div>').appendTo($viewgriddiv);
                        } else {
                            $viewgrid.empty();
                        }

                        //                        var g = {
                        //                            ID: o.ID,
                        //                            pagermode: 'none',
                        //                            gridOpts: {
                        //                                autowidth: true,
                        //                                height: 180,
                        //                                onSelectRow: function (id, selected) {
                        //                                    rowid = id;
                        //                                    if (selected) {
                        //                                        copyViewBtn.enable();
                        //                                        deleteViewBtn.enable();
                        //                                        $selview_span.text(_getSelectedViewName(id));
                        //                                        $wizard.CswWizard('button', 'next', 'enable');
                        //                                    } else {
                        //                                        copyViewBtn.disable();
                        //                                        deleteViewBtn.disable();
                        //                                        $selview_span.text("");
                        //                                        $wizard.CswWizard('button', 'next', 'disable');
                        //                                        cswViewGrid.resetSelection();
                        //                                    }
                        //                                }
                        //                            }
                        //                        };
                        //                        Csw.extend(g.gridOpts, gridJson);
                        //                        g.gridOpts.rowNum = 100000;


                        var parent = Csw.literals.factory($viewgrid);
                        cswViewGrid = parent.grid({
                            ID: o.ID + '_grid',
                            storeId: o.ID + '_store',
                            title: '',
                            stateId: o.ID + '_gridstate',
                            usePaging: false,
                            showActionColumn: false,
                            height: 230,
                            fields: gridJson.grid.fields,
                            columns: gridJson.grid.columns,
                            data: gridJson.grid.data,
                            pageSize: gridJson.grid.pageSize,
                            canSelectRow: true,
                            onSelect: function (row) {
                                copyViewBtn.enable();
                                deleteViewBtn.enable();
                                $selview_span.text(row.viewname);
                                $wizard.CswWizard('button', 'next', 'enable');
                            },
                            onDeselect: function (row) {
                                copyViewBtn.disable();
                                deleteViewBtn.disable();
                                $selview_span.text("");
                                $wizard.CswWizard('button', 'next', 'disable');
                                //cswViewGrid.resetSelection();
                            },
                            onLoad: function (grid) {
                                if (false === Csw.isNullOrEmpty(gridJson.selectedpk)) {
                                    rowid = grid.getRowIdForVal(o.ColumnViewId, gridJson.selectedpk);
                                    grid.setSelection(rowid);
                                    grid.scrollToRow(rowid);
                                }
                            }
                        });
                        //cswViewGrid.gridPager.css({ width: '100%', height: '20px' });
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
                    ID: Csw.makeId(o.ID, 'editrel'),
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

                if (viewmode === 'Tree') {
                    subTable.cell(row, 1).text('Group By');
                }
                var groupBySelect = subTable.cell(row, 2)
                                            .select({ ID: o.ID + '_gbs',
                                                onChange: function () {
                                                    var selected = groupBySelect.find(':selected');
                                                    var selval = selected.val();

                                                    if (false === Csw.isNullOrEmpty(selval)) {
                                                        viewnodejson.groupbypropid = Csw.string(selected.val());
                                                        viewnodejson.groupbyproptype = Csw.string(selected.propNonDom('propType'));
                                                        viewnodejson.groupbypropname = Csw.string(selected.text());
                                                    } // if (false === Csw.isNullOrEmpty(selval)) {
                                                } // onChange
                                            }); // 
                if (viewmode !== 'Tree') {
                    groupBySelect.hide();
                }
                row += 1;

                var jsonData = {
                    Type: viewnodejson.secondtype,
                    Id: viewnodejson.secondid
                };
                Csw.ajax.post({
                    urlMethod: o.PropNamesUrl,
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
                        FirstCellRightAlign: false
                    });

                    var toggleShowInGridAttr = function () {
                        if (Csw.bool(viewNodeData.showingrid)) {
                            showInGridTable.show();
                        } else {
                            showInGridTable.hide();
                        }
                    };

                    var showInGridcheck = gridTable.cell(1, 1)
                                                   .append('Show In Grid')
                                                   .input({
                                                       ID: o.ID + '_showingrid',
                                                       type: Csw.enums.inputTypes.checkbox,
                                                       onChange: function () {
                                                           viewNodeData.showingrid = showInGridcheck.$.is(':checked');
                                                           toggleShowInGridAttr();
                                                       }
                                                   });
                    if (Csw.bool(viewNodeData.showingrid)) {
                        showInGridcheck.propDom('checked', 'true');
                    }

                    var showInGridTable = gridTable.cell(2, 1).table();

                    showInGridTable.cell(1, 1).text('Sort By');
                    var sortByCheckCell = showInGridTable.cell(1, 2);
                    var sortbycheck = sortByCheckCell.input({
                        ID: o.ID + '_sortcb',
                        type: Csw.enums.inputTypes.checkbox,
                        onChange: function () {
                            viewNodeData.sortby = sortbycheck.$.is(':checked');
                        }
                    });
                    if (Csw.bool(viewNodeData.sortby)) {
                        sortbycheck.propDom('checked', 'true');
                    }

                    showInGridTable.cell(2, 1).text('Grid Column Order');
                    var colOrderTextCell = showInGridTable.cell(2, 2);
                    var colordertextbox = colOrderTextCell.input({
                        ID: o.ID + '_gcotb',
                        type: Csw.enums.inputTypes.text,
                        onChange: function () {
                            viewNodeData.order = colordertextbox.val();
                        }
                    });
                    colordertextbox.val(viewNodeData.order);

                    showInGridTable.cell(3, 1).text('Grid Column Width (in characters)');
                    var colWidthTextCell = showInGridTable.cell(3, 2);
                    var colwidthtextbox = colWidthTextCell.input(
                            {
                                ID: o.ID + '_gcwtb',
                                type: Csw.enums.inputTypes.text,
                                onChange: function () {
                                    viewNodeData.width = colwidthtextbox.val();
                                }
                            });
                    colwidthtextbox.val(viewNodeData.width);

                    toggleShowInGridAttr();
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

                filterTable.cell(1, 1).text('Case sensitive');
                var cbCaseSensitive = filterTable.cell(1, 2)
                    .input({
                        ID: o.ID + '_casecb',
                        type: Csw.enums.inputTypes.checkbox,
                        onChange: function () {
                            //var $this = $(this);
                            viewNodeData.casesensitive = cbCaseSensitive.$.is(':checked');
                        },
                        checked: Csw.bool(viewNodeData.casesensitive)
                    });

                filterTable.cell(2, 1).text('Show at runtime');
                var cbShowAtRuntime = filterTable.cell(2, 2)
                    .input({
                        ID: o.ID + '_showcb',
                        type: Csw.enums.inputTypes.checkbox,
                        onChange: function () {
                            //var $this = $(this);
                            viewNodeData.showatruntime = cbShowAtRuntime.$.is(':checked');
                        },
                        checked: Csw.bool(viewNodeData.showatruntime)
                    });

                filterTable.cell(3, 1).text('For non-matches');
                var listResultMode = filterTable.cell(3, 2)
                    .select({
                        ID: o.ID + '_resultmode',
                        values: ['Hide', { value: 'Disabled', display: 'Show Disabled'}],
                        onChange: function () {
                            //var $this = $(this);
                            viewNodeData.resultmode = listResultMode.val();
                        },
                        selected: viewNodeData.resultmode
                    });
            });


            // case 27553 - export to code
            var cell21 = table6.cell(2, 1);
            cell21.empty();
            cell21.buttonExt({
                ID: 'vieweditor_expcodebtn',
                enabledText: 'Export to Code',
                disableOnClick: false,
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.doc),
                onClick: function () {
                    var codepopup = window.open();
                    codepopup.document.write('<textarea rows="20" cols="60">' + _makeCode(currentViewJson) + '</textarea>');
                },
                isEnabled: true,
                size: 'small'
            });


        } // makeTuningStep()

        function _makeCode(viewJson) {
            var code = '';
            var elmcount = 1;
            var viewname = 'myView';

            function rootToCode(root) {
                code += 'CswNbtView ' + viewname + ' = _CswNbtSchemaModTrnsctn.makeView();\n';

                var visroleid = 'null';
                var visuserid = 'null';
                if (false === Csw.isNullOrEmpty(root.visibilityroleid)) {
                    visroleid = 'new CswPrimaryKey("nodes", ' + root.visibilityroleid + ')';
                }
                if (false === Csw.isNullOrEmpty(root.visibilityuserid)) {
                    visuserid = 'new CswPrimaryKey("nodes", ' + root.visibilityuserid + ')';
                }
                code += viewname + '.makeNew( "' + root.viewname + '", NbtViewVisibility.' + root.visibility + ', ' + visroleid + ', ' + visuserid + ');\n';

                code += viewname + '.ViewMode = NbtViewRenderingMode.' + root.mode + ';\n';
                if (false === Csw.isNullOrEmpty(root.category)) {
                    code += viewname + '.Category = "' + root.category + '";\n';
                }
                if (Csw.bool(root.formobile)) {
                    code += viewname + '.ForMobile = ' + root.formobile + ';\n';
                }
                if (false === Csw.isNullOrEmpty(root.width)) {
                    code += viewname + '.Width = ' + root.width + ';\n';
                }
                code += '\n';

                Csw.each(root.childrelationships, function (child) { rootRelationshipToCode(child); });

                code += viewname + '.save();\n';
            } // rootToCode()

            function rootRelationshipToCode(rel) {
                var elmid = "Rel" + elmcount;
                elmcount += 1;

                if (rel.secondtype === 'NodeTypeId') {
                    code += 'CswNbtMetaDataNodeType ' + elmid + 'SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "' + rel.secondname + '" );\n';
                    code += 'CswNbtViewRelationship ' + elmid + ' = ' + viewname + '.AddViewRelationship( ' + elmid + 'SecondNT, true );\n';
                } else {
                    code += 'CswNbtMetaDataObjectClass ' + elmid + 'SecondOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.' + rel.secondname + ' );\n';
                    code += 'CswNbtViewRelationship ' + elmid + ' = ' + viewname + '.AddViewRelationship( ' + elmid + 'SecondOC, true );\n';
                }
                if (false === Csw.bool(rel.showintree)) {
                    code += elmid + '.ShowInTree = ' + rel.showintree + ';\n';
                }
                code += '\n';

                Csw.each(rel.properties, function (child) { propertyToCode(elmid, rel, child); });
                Csw.each(rel.childrelationships, function (child) { childRelationshipToCode(elmid, rel, child); });
            } // rootRelationshipToCode()

            function childRelationshipToCode(parentelmid, parentrel, rel) {
                var elmid = "Rel" + elmcount;
                elmcount += 1;

                if (rel.propowner == "First") {
                    if (parentrel.secondtype === 'NodeTypeId') {
                        code += 'CswNbtMetaDataNodeTypeProp ' + elmid + 'Prop = ' + parentelmid + 'SecondNT.getNodeTypeProp( "' + rel.propname + '" );\n';
                        if (rel.secondtype === 'NodeTypeId') {
                            code += 'CswNbtMetaDataNodeType ' + elmid + 'SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "' + rel.secondname + '" );\n';
                        } else {
                            code += 'CswNbtMetaDataObjectClass ' + elmid + 'SecondOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.' + rel.secondname + ' );\n';
                        }
                    } else {
                        code += 'CswNbtMetaDataObjectClassProp ' + elmid + 'Prop = ' + parentelmid + 'SecondOC.getObjectClassProp( CswNbtObjClass' + parentrel.secondname.replace('Class', '') + '.PropertyName.' + rel.propname.replace(' ', '') + ' );\n';
                        if (rel.secondtype === 'NodeTypeId') {
                            code += 'CswNbtMetaDataNodeType ' + elmid + 'SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "' + rel.secondname + '" );\n';
                        } else {
                            code += 'CswNbtMetaDataObjectClass ' + elmid + 'SecondOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.' + rel.secondname + ' );\n';
                        }
                    }
                } else {
                    if (rel.secondtype === 'NodeTypeId') {
                        code += 'CswNbtMetaDataNodeType ' + elmid + 'SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "' + rel.secondname + '" );\n';
                        code += 'CswNbtMetaDataNodeTypeProp ' + elmid + 'Prop = ' + elmid + 'SecondNT.getNodeTypeProp( "' + rel.propname + '" );\n';
                    } else {
                        code += 'CswNbtMetaDataObjectClass ' + elmid + 'SecondOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.' + rel.secondname + ' );\n';

                        code += 'CswNbtMetaDataObjectClassProp ' + elmid + 'Prop = ' + elmid + 'SecondOC.getObjectClassProp( CswNbtObjClass' + rel.secondname.replace('Class', '') + '.PropertyName.' + rel.propname.replace(' ', '') + ' );\n';
                    }
                }

                code += 'CswNbtViewRelationship ' + elmid + ' = ' + viewname + '.AddViewRelationship( ' + parentelmid + ', NbtViewPropOwnerType.' + rel.propowner + ', ' + elmid + 'Prop, true );\n';
                if (false === Csw.bool(rel.showintree)) {
                    code += elmid + '.ShowInTree = ' + rel.showintree + ';\n';
                }
                code += '\n';

                Csw.each(rel.properties, function (child) { propertyToCode(elmid, rel, child); });
                Csw.each(rel.childrelationships, function (child) { childRelationshipToCode(elmid, rel, child); });
            } // childRelationshipToCode()

            function propertyToCode(parentelmid, parentrel, prop) {
                var elmid = "Prop" + elmcount;
                elmcount += 1;

                if (parentrel.secondtype === 'NodeTypeId') {
                    if (prop.type === 'NodeTypePropId') {
                        code += 'CswNbtMetaDataNodeType ' + elmid + 'NTP = ' + parentelmid + 'SecondNT.getNodeTypeProp( "' + prop.name + '" );\n';
                        code += 'CswNbtViewProperty ' + elmid + ' = ' + viewname + '.AddViewProperty( ' + parentelmid + ', ' + elmid + 'NTP );\n';
                    } else {
                        code += 'CswNbtMetaDataObjectClass ' + elmid + 'OCP = ' + parentelmid + 'SecondNT.getNodeTypeProp( "' + prop.name + '" ).ObjectClassProp;\n';
                        code += 'CswNbtViewProperty ' + elmid + ' = ' + viewname + '.AddViewProperty( ' + parentelmid + ', ' + elmid + 'OCP );\n';
                    }
                } else {
                    if (prop.type === 'NodeTypePropId') {
                        code += 'CswNbtViewProperty ' + elmid + ' = null;\n';
                        code += 'foreach( CswNbtMetaDataNodeType ' + parentelmid + 'NT in ' + parentelmid + 'SecondOC.getNodeTypes() )\n';
                        code += '{\n';
                        code += '    CswNbtMetaDataNodeType ' + elmid + 'NTP = ' + parentelmid + 'NT.getNodeTypeProp( "' + prop.name + '" );\n';
                        code += '    if( null != ' + elmid + 'NTP )\n';
                        code += '    {\n';
                        code += '        ' + elmid + ' = ' + viewname + '.AddViewProperty( ' + parentelmid + ', ' + elmid + 'NTP );\n';
                        code += '        break;\n';
                        code += '    }\n';
                        code += '}\n';
                    } else {
                        code += 'CswNbtMetaDataObjectClassProp ' + elmid + 'OCP = ' + parentelmid + 'SecondOC.getObjectClassProp( CswNbtObjClass' + parentrel.secondname.replace('Class', '') + '.PropertyName.' + prop.name.replace(' ', '') + ' );\n';
                        code += 'CswNbtViewProperty ' + elmid + ' = ' + viewname + '.AddViewProperty( ' + parentelmid + ', ' + elmid + 'OCP );\n';
                    }
                }

                if (Csw.bool(prop.sortby)) {
                    code += elmid + '.SortBy = ' + prop.sortby + ';\n';
                    code += elmid + '.SortMethod = NbtViewPropertySortMethod.' + prop.sortmethod + ';\n';
                }
                if (false === Csw.isNullOrEmpty(prop.order)) {
                    code += elmid + '.Order = ' + prop.order + ';\n';
                }
                if (false === Csw.isNullOrEmpty(prop.width)) {
                    code += elmid + '.Width = ' + prop.width + ';\n';
                }
                if (false === Csw.bool(prop.showingrid)) {
                    code += elmid + '.ShowInGrid = ' + prop.showingrid + ';\n';
                }

                Csw.each(prop.filters, function (child) { filterToCode(elmid, child); });
            } // propertyToCode()

            function filterToCode(parentelmid, filt) {
                var elmid = "Filt" + elmcount;
                elmcount += 1;

                code += 'CswNbtViewFilter ' + elmid + ' = ' + viewname + '.AddViewPropertyFilter( ' + parentelmid + ',\n';
                code += '                                          CswNbtPropFilterSql.PropertyFilterConjunction.' + filt.conjunction + ',\n';
                code += '                                          CswNbtPropFilterSql.FilterResultMode.' + filt.resultmode + ',\n';
                code += '                                          CswNbtSubField.SubFieldName.' + filt.subfieldname + ',\n';
                code += '                                          CswNbtPropFilterSql.PropertyFilterMode.' + filt.filtermode + ',\n';
                code += '                                          "' + filt.value + '",\n';
                code += '                                          ' + filt.casesensitive + ',\n';
                code += '                                          ' + filt.showatruntime + ' );\n';
                code += '\n';

            } // filterToCode()

            rootToCode(viewJson);
            return code;
        } // _makeCode()


        function _makeViewTree(stepno, $content) {
            var doTree = function () {

                var treecontent = viewJsonHtml(stepno, currentViewJson);

                try {
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
                } catch (e) {
                    Csw.error.showError(Csw.enums.errorType.error, 'Unable to render this tree element.', 'Exception: ' + e.name + ' occurred with ' + e.message);
                }
            };

            var $tree = $content;
            if (Csw.isNullOrEmpty($tree)) {
                $tree = getTreeDiv(stepno);
            }

            if (stepno === Csw.enums.wizardSteps_ViewEditor.filters.step) {
                Csw.ajax.post({
                    urlMethod: o.FiltersUrlMethod,
                    data: {
                        ViewId: _getSelectedViewId(),
                        NewPropArbIds: o.newProps.join(','),
                        ViewJson: JSON.stringify(currentViewJson)
                    },
                    success: function (data) {
                        o.filtersData = data;
                        doTree();
                    }
                });
            } else {
                doTree();
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
                    $this.button({ label: $this.attr('disabledText'), disabled: true });
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
                            Csw.extend(propJson[Csw.enums.viewChildPropNames.propfilters.name], newPropJson);
                            _makeViewTree(stepno);
                        } // onSuccess
                    }); // CswViewPropFilter
                });

                $span.find('.ViewPropFilterLogical').each(function () {
                    var $this = $(this);
                    var id = $this.prop('id');
                    var $parent = $this.parent();
                    $parent.empty();
                    var parent = Csw.literals.factory($parent);
                    parent.triStateCheckBox({ ID: id,
                        Checked: 'false',
                        cssclass: 'ViewPropFilterLogical ' + Csw.enums.cssClasses_ViewBuilder.filter_value.name
                    });
                    /* This may not be necessary */
                    //$this.CswTristateCheckBox('reBindClick');
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
                                    var $filtLi = makeViewPropertyFilterHtml(thisFilt, stepno, types, arbid, false);
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
                    var filterData = null;
                    if (Csw.contains(o.filtersData, arbid)) {
                        filterData = o.filtersData[arbid];
                    }
                    var $filtBuilderLi = makeViewPropertyFilterHtml(filterData, stepno, types, arbid, true);
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

        function makeViewPropertyFilterHtml(itemJson, stepno, types, propArbId, isAdd) {
            var $ret = null;
            if (stepno >= Csw.enums.wizardSteps_ViewEditor.filters.step) {
                $ret = $('<li></li>');
                var rel = 'filter';
                if (false === isAdd && false === Csw.isNullOrEmpty(itemJson)) {
                    var filtArbitraryId = Csw.string(itemJson.arbitraryid);
                    if (stepno === Csw.enums.wizardSteps_ViewEditor.tuning.step) {
                        var selectedConjunction = Csw.string(itemJson.conjunction);
                        var selectedSubfield = Csw.string(itemJson.subfield, itemJson.subfieldname);
                        var selectedFilterMode = Csw.string(itemJson.filtermode);
                        var filterValue = Csw.string(itemJson.value);
                        var name = selectedConjunction + ' ' + selectedSubfield + ' ' + selectedFilterMode + ' ' + filterValue;
                        var $filtLink = makeViewListItem(filtArbitraryId, Csw.enums.cssClasses_ViewEdit.vieweditor_viewfilterlink.name, name, false, stepno, Csw.enums.viewChildPropNames.filters, rel);
                        if (false === Csw.isNullOrEmpty($filtLink)) {
                            $ret = $filtLink;
                        }
                    } else {
                        $ret.append(makeViewPropFilterStaticSpan(propArbId, itemJson, filtArbitraryId, rel));
                        makeDeleteSpan(filtArbitraryId, $ret);
                    }
                } else {
                    $ret.append(makeViewPropFilterAddSpan(propArbId, itemJson));
                }
            }
            types.filter = { icon: { image: "Images/view/filter.gif"} };
            return $ret;
        }

        function makeViewPropFilterStaticSpan(propArbId, filterJson, filtArbitraryId) {
            var $span = $('<span class="' + Csw.enums.cssClasses_ViewEdit.vieweditor_addfilter.name + '" arbid="' + filtArbitraryId + '"></span>');
            var table = Csw.literals.table({
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

        function makeViewPropFilterAddSpan(propArbId, itemJson) {
            var $span = $('<span class="' + Csw.enums.cssClasses_ViewEdit.vieweditor_addfilter.name + '" arbid="' + propArbId + '"></span>');
            var table = Csw.literals.table({
                $parent: $span,
                'ID': o.ID + '_' + propArbId + '_propfilttbl'
            });
            table.css({ 'display': 'inline-table' });

            table.$.CswViewPropFilter('init', {
                viewJson: currentViewJson,
                ID: o.ID + '_' + propArbId + '_propfilttbl',
                propsData: itemJson,
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
            });
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
            var td = Csw.literals.span({
                $parent: $parent,
                cssclass: Csw.enums.cssClasses_ViewEdit.vieweditor_deletespan.name
            }).propNonDom('arbid', arbid);
            td.icon({
                ID: arbid + '_delete',
                iconType: Csw.enums.iconType.x,
                hovertext: 'Delete'
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
                    urlMethod: o.ChildOptionsUrl,
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
                                Csw.extend(currentViewJson.childrelationships, childJson);
                            } else {
                                var objUtil = Csw.object(currentViewJson);
                                var parentObj = objUtil.find('arbitraryid', arbid);
                                var collection = '';
                                switch (stepno) {
                                    case Csw.enums.wizardSteps_ViewEditor.relationships.step:
                                        collection = Csw.enums.viewChildPropNames.childrelationships.name;
                                        break;
                                    case Csw.enums.wizardSteps_ViewEditor.properties.step:
                                        o.newProps.push($selected.val());
                                        collection = Csw.enums.viewChildPropNames.properties.name;
                                        break;
                                }
                                var objCollection = parentObj[collection];
                                if (Csw.isNullOrEmpty(objCollection)) {
                                    objCollection = {};
                                    parentObj[collection] = objCollection;
                                }
                                Csw.extend(objCollection, childJson);
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

