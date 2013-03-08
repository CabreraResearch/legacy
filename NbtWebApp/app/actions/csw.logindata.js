/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
Csw.actions.logindata = Csw.actions.template ||
    Csw.actions.register('logindata', function (cswParent, options) {
        'use strict';
        
        var cswPublic = {};
        var cswPrivate = {
            name: 'View Login Data'
        };
        if (options) {
            Csw.extend(cswPrivate, options);
        }
        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.logindata', 'csw.logindata.js', 10);
        }
        
        //#region _preCtor
        (function _preCtor() {
            cswPrivate.onCancel = cswPrivate.onCancel || function () { };
            cswPrivate.LoginRecords = [];

            cswParent.empty();
        }());
        //#endregion _preCtor

        //#region Action Functions
        cswPrivate.onCancelClick = function() {
            Csw.tryExec(cswPrivate.onCancel);
        };
        
        cswPrivate.getCurrentDate = function (monthIndex) {
            var monthOffset = Csw.isNullOrEmpty(monthIndex) ? 1 : monthIndex;
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + monthOffset; //January is 0!
            if (mm === 0) {
                mm = 12;
            }
            var yyyy = today.getFullYear();
            if (dd < 10) {
                dd = '0' + dd;
            }
            if (mm < 10) {
                mm = '0' + mm;
            }
            today = mm + '/' + dd + '/' + yyyy;
            return today;
        };

        cswPrivate.getLastMonth = function() {
            return cswPrivate.getCurrentDate(0);
        };

        cswPrivate.updateGrid = function() {
            cswPrivate.loadGrid({
                StartDate: cswPrivate.StartDate,
                EndDate: cswPrivate.EndDate
            });
        };
        //#endregion Action Functions
        
        //#region Setup Controls
        cswPrivate.createSetupControls = function() {
            //StartDate
            cswPrivate.controlTbl.cell(1, 1).span({ text: 'Start Date:' }).addClass('propertylabel');
            var startDatePicker = cswPrivate.controlTbl.cell(1, 2).dateTimePicker({
                name: 'startDate',
                Date: cswPrivate.getLastMonth(),
                isRequired: true,
                maxDate: cswPrivate.getCurrentDate(),
                onChange: function () {
                    if (startDatePicker.val().date > cswPrivate.EndDate) {
                        startDatePicker.setMaxDate(cswPrivate.EndDate);
                    }
                    cswPrivate.StartDate = startDatePicker.val().date;
                    cswPrivate.updateGrid();
                }
            });
            cswPrivate.StartDate = startDatePicker.val().date;
            //EndDate
            cswPrivate.controlTbl.cell(2, 1).span({ text: 'End Date:' }).addClass('propertylabel');
            var endDatePicker = cswPrivate.controlTbl.cell(2, 2).dateTimePicker({
                name: 'endDate',
                Date: cswPrivate.getCurrentDate(),
                isRequired: true,
                maxDate: cswPrivate.getCurrentDate(),
                onChange: function () {
                    if (endDatePicker.val().date > cswPrivate.getCurrentDate()) {
                        endDatePicker.setMaxDate(cswPrivate.getCurrentDate());
                    }
                    cswPrivate.EndDate = endDatePicker.val().date;
                    startDatePicker.setMaxDate(cswPrivate.EndDate);
                    cswPrivate.StartDate = startDatePicker.val().date;
                    cswPrivate.updateGrid();
                }
            });
            cswPrivate.EndDate = endDatePicker.val().date;
            //Load grid on page load
            cswPrivate.updateGrid();
        };
        //#endregion Setup Controls
        
        //#region Grid Control
        cswPrivate.loadGrid = function (loginRequestData) {
            Csw.ajaxWcf.post({
                urlMethod: 'Session/getLoginData',
                data: loginRequestData,
                success: function (ajaxdata) {
                    //Grid Data
                    cswPrivate.Logins = [{
                        Username: '',
                        IPAddress: '',
                        LoginDate: '',
                        LoginStatus: '',
                        FailureReason: '',
                        FailedLoginCount: 0,
                    }];
                    Csw.extend(cswPrivate.Logins, ajaxdata.Logins);
                    if (ajaxdata.Logins.length === 0) {
                        cswPrivate.Logins = [];
                    }

                    var LoginGridData = [];
                    Csw.each(cswPrivate.Logins, function (row) {
                        if (false === Csw.isNullOrEmpty(row.Username)) {
                            LoginGridData.push({
                                username: row.Username,
                                ipaddress: row.IPAddress,
                                logindate: row.LoginDate,
                                loginstatus: row.LoginStatus,
                                failurereason: row.FailureReason,
                                failedlogincount: row.FailedLoginCount
                            });
                        }
                    });

                    //Grid Fields and Columns
                    var LoginGridColumns = [];
                    var LoginGridFields = [];
                    var addColumn = function (colName, displayName) {
                        LoginGridColumns.push({
                            dataIndex: colName,
                            filterable: true,
                            header: displayName,
                            id: 'login_' + colName
                        });
                        LoginGridFields.push({
                            name: colName,
                            type: 'string',
                            useNull: true
                        });
                    };
                    
                    addColumn('username', 'Username');
                    addColumn('ipaddress', 'IP Address');
                    addColumn('logindate', 'Login Date');
                    addColumn('loginstatus', 'Login Status');
                    addColumn('failurereason', 'Failure Reason');
                    addColumn('failedlogincount', 'Failed Login Count');

                    //Grid Control
                    var LoginGridId = 'LoginGrid';
                    cswPrivate.gridOptions = {
                        name: LoginGridId,
                        storeId: LoginGridId,
                        title: 'Login Data from ' + cswPrivate.StartDate + ' to ' + cswPrivate.EndDate,
                        stateId: LoginGridId,
                        usePaging: false,
                        showActionColumn: false,
                        height: 400,
                        forcefit: true,
                        width: '100%',
                        fields: LoginGridFields,
                        columns: LoginGridColumns,
                        data: LoginGridData,
                        printingEnabled: true
                    };
                    cswPrivate.gridTbl.cell(1, 1).empty();
                    cswPrivate.LoginGrid = cswPrivate.gridTbl.cell(1, 1).grid(cswPrivate.gridOptions);
                }
            });
        };
        //#endregion Grid Control

        //#region _postCtor
        (function _postCtor() {
            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'View Login Data',
                useFinish: false,
                cancelText: 'Close',
                onCancel: cswPrivate.onCancelClick,
                hasButtonGroup: true
            });
            
            cswPrivate.controlTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_control_tbl',
                cellpadding: '5px',
                cellalign: 'left',
                cellvalign: 'middle',
                width: '50%',
                FirstCellRightAlign: true
            });
            
            cswPrivate.gridTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_control_tbl',
                cellpadding: '5px',
                cellalign: 'left',
                align: 'center',
                cellvalign: 'middle',
                width: '95%'
            });

            cswPrivate.createSetupControls();
        }());
        //#endregion _postCtor
        
        return cswPublic;
    });
}());