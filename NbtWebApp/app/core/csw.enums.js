/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswEnums() {
    'use strict';

    Csw.enums = Csw.enums ||
        Csw.register('enums', Csw.makeNameSpace());

    Csw.enums.constants = Csw.enums.constants ||
        Csw.enums.register('constants', { unknownEnum: 'unknown' });

    Csw.enums.tryParse = Csw.enums.tryParse ||
        Csw.enums.register('tryParse', function (cswEnum, enumMember, caseSensitive) {
            'use strict';
            /// <summary>   Try to fetch an enum based on a string value. </summary>
            var ret = Csw.enums.constants.unknownEnum;
            if (Csw.contains(cswEnum, enumMember)) {
                ret = cswEnum[enumMember];
            } else if (false === caseSensitive) {
                Csw.each(cswEnum, function (member) {
                    if (Csw.contains(cswEnum, member) &&
                        Csw.string(member).toLowerCase() === Csw.string(enumMember).toLowerCase()) {
                        ret = member;
                    }
                });
            }
            return ret;
        });

    Csw.enums.getName = Csw.enums.getName ||
        Csw.enums.register('getName', function (cswEnum, enumValue) {
            'use strict';
            /// <summary>   Try to fetch an enum name based on its value. </summary>
            var ret = '';

            Csw.each(cswEnum, function (value, name) {
                if(value === enumValue) {
                    ret = name;
                    return true;
                }
            });
            return ret;
        });

    Csw.enums.editMode = Csw.enums.editMode ||
        Csw.enums.register('editMode', {
            Edit: 'Edit',
            Add: 'Add',
            EditInPopup: 'EditInPopup',
            Demo: 'Demo',
            PrintReport: 'PrintReport',
            DefaultValue: 'DefaultValue',
            AuditHistoryInPopup: 'AuditHistoryInPopup',
            Preview: 'Preview',
            Table: 'Table',
            Temp: 'Temp'
        });

    Csw.enums.errorType = Csw.enums.errorType ||
        Csw.enums.register('errorType', {
            warning: {
                name: 'warning',
                cssclass: 'CswErrorMessage_Warning'
            },
            error: {
                name: 'error',
                cssclass: 'CswErrorMessage_Error'
            }
        });

    Csw.enums.ajaxUrlPrefix = Csw.enums.ajaxUrlPrefix ||
        Csw.enums.register('ajaxUrlPrefix', '/NbtWebApp/wsNBT.asmx/');

    Csw.enums.events = Csw.enums.events ||
        Csw.enums.register('events', {
            CswNodeDelete: 'CswNodeDelete',
            ajax: {
                ajaxStart: 'ajaxStart',
                ajaxStop: 'ajaxStop',
                globalAjaxStart: 'globalAjaxStart',
                globalAjaxStop: 'globalAjaxStop'
            },
            domready: 'DOM_Ready',
            Submit_Request: 'Submit_Request',
            objectClassButtonClick: 'objectClassButtonClick',
            afterObjectClassButtonClick: 'afterObjectClassButtonClick',
            RestoreViewContext: 'RestoreViewContext',
            DispenseContainer: 'DispenseContainer',
            main: {
                handleAction: 'handleAction',
                refresh: 'refresh',
                refreshHeader: 'refreshHeader',
                refreshSelected: 'refreshSelected',
                clear: 'clear'
            }
        });

    Csw.enums.wizardSteps_InspectionDesign = Csw.enums.wizardSteps_InspectionDesign ||
        Csw.enums.register('wizardSteps_InspectionDesign', {
            step1: { step: 1, description: 'Select an Inspection Target' },
            step2: { step: 2, description: 'Select an Inspection Design' },
            step3: { step: 3, description: 'Upload Template' },
            step4: { step: 4, description: 'Review Inspection Design' },
            step5: { step: 5, description: 'Finish' },
            stepcount: 5
        });

    Csw.enums.wizardSteps_ScheduleRulesGrid = Csw.enums.wizardSteps_ScheduleRulesGrid ||
        Csw.enums.register('wizardSteps_ScheduleRulesGrid', {
            step1: { step: 1, description: 'Select a Customer ID' },
            step2: { step: 2, description: 'Review the Scheduled Rules' },
            stepcount: 2
        });

    Csw.enums.wizardSteps_FutureScheduling = Csw.enums.wizardSteps_FutureScheduling ||
        Csw.enums.register('wizardSteps_FutureScheduling', {
            step1: { step: 1, description: 'Specify Schedules' },
            step2: { step: 2, description: 'Review' },
            stepcount: 2
        });

    Csw.enums.dialogButtons = Csw.enums.dialogButtons ||
        Csw.enums.register('dialogButtons', {
            1: 'ok',
            2: 'ok/cancel',
            3: 'yes/no'
        });

    Csw.enums.nbtButtonAction = Csw.enums.nbtButtonAction ||
        Csw.enums.register('nbtButtonAction', {
            dispense: 'dispense',
            reauthenticate: 'reauthenticate',
            //home: 'home',
            receive: 'receive',
            refresh: 'refresh',
            popup: 'popup',
            request: 'request',
            loadView: 'loadview',
            editprop: 'editprop',
            nothing: 'nothing'
        });

    Csw.enums.inputTypes = Csw.enums.inputTypes ||
        Csw.enums.register('inputTypes', {
            button: { id: 0, name: 'button', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
            checkbox: { id: 1, name: 'checkbox', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '' },
            color: { id: 2, name: 'color', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
            date: { id: 3, name: 'date', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
            datetime: { id: 4, name: 'datetime', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px' },
            'datetime-local': { id: 5, name: 'datetime-local', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
            email: { id: 6, name: 'email', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
            file: { id: 7, name: 'file', placeholder: false, autocomplete: false, value: { required: false, allowed: false }, defaultwidth: '' },
            hidden: { id: 8, name: 'hidden', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
            image: { id: 9, name: 'image', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
            month: { id: 10, name: 'month', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
            number: { id: 11, name: 'number', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px' },
            password: { id: 12, name: 'password', placeholder: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
            radio: { id: 13, name: 'radio', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '' },
            range: { id: 14, name: 'range', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
            reset: { id: 15, name: 'reset', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
            search: { id: 16, name: 'search', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
            submit: { id: 17, name: 'submit', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
            tel: { id: 18, name: 'button', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
            text: { id: 19, name: 'text', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
            time: { id: 20, name: 'time', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
            url: { id: 21, name: 'url', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
            week: { id: 22, name: 'week', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' }
        });

    Csw.enums.viewMode = Csw.enums.viewMode ||
        Csw.enums.register('viewMode', {
            grid: { name: 'Grid' },
            tree: { name: 'Tree' },
            list: { name: 'List' },
            table: { name: 'Table' }
        });

    Csw.enums.rateIntervalTypes = Csw.enums.rateIntervalTypes ||
        Csw.enums.register('rateIntervalTypes', {
            WeeklyByDay: 'WeeklyByDay',
            MonthlyByDate: 'MonthlyByDate',
            MonthlyByWeekAndDay: 'MonthlyByWeekAndDay',
            YearlyByDate: 'YearlyByDate'
        });

    Csw.enums.multiEditDefaultValue = Csw.enums.multiEditDefaultValue ||
        Csw.enums.register('multiEditDefaultValue', '[Unchanged]');

    Csw.enums.imageButton_ButtonType = Csw.enums.imageButton_ButtonType ||
        Csw.enums.register('imageButton_ButtonType', {
            None: -1,
            Add: 27,
            ArrowNorth: 28,
            ArrowEast: 29,
            ArrowSouth: 30,
            ArrowWest: 31,
            Calendar: 6,
            CheckboxFalse: 18,
            CheckboxNull: 19,
            CheckboxTrue: 20,
            Clear: 4,
            Clock: 10,
            ClockGrey: 11,
            Configure: 26,
            Delete: 4,
            Edit: 3,
            Fire: 5,
            PageFirst: 23,
            PagePrevious: 24,
            PageNext: 25,
            PageLast: 22,
            PinActive: 17,
            PinInactive: 15,
            Print: 2,
            Refresh: 9,
            SaveStatus: 13,
            Select: 32,
            TableSingleColumn: 33,
            TableMultiColumn: 34,
            ToggleActive: 1,
            ToggleInactive: 0,
            View: 8
        });


    Csw.enums.iconState = Csw.enums.iconState ||
        Csw.enums.register('iconState', {
            none: -1,
            normal: 0,
            hover: 1,
            selected: 2,
            disabled: 3
        });

    Csw.enums.iconType = Csw.enums.iconType ||
        Csw.enums.register('iconType', {
            none: -1,
            infobox: 0,
            info: 1,
            plus: 2,
            minus: 3,
            up: 4,
            right: 5,
            down: 6,
            left: 7,
            play: 8,
            fastforward: 9,
            rewind: 10,
            undo: 11,
            redo: 12,
            refresh: 13,
            check: 14,
            x: 15,
            stop: 16,
            cancel: 17,
            xbutton: 18,
            rightbutton: 19,
            explode: 20,
            star: 21,
            staradd: 22,
            docrefresh: 23,
            docimport: 24,
            docexport: 25,
            person: 26,
            people: 27,
            contact: 28,
            calendar: 29,
            calculator: 30,
            phone: 31,
            world: 32,
            cart: 33,
            flag: 34,
            docs: 35,
            barchart: 36,
            harddrive: 37,
            bell: 38,
            questionmark: 39,
            trash: 40,
            scissors: 41,
            lock: 42,
            unlock: 43,
            envelope: 44,
            toolbox: 45,
            wrench: 46,
            doc: 47,
            magglass: 48,
            magplus: 49,
            magminus: 50,
            pencil: 51,
            folder: 52,
            foldergear: 53,
            gear: 54,
            gears: 55,
            trianglegear: 56,
            clipgear: 57,
            clipboard: 58,
            target: 59,
            targetgroup: 60,
            compass: 61,
            barcode: 62,
            dna: 63,
            atom: 64,
            flask: 65,
            house: 66,
            door: 67,
            cabinet: 68,
            box: 69,
            save: 70, //not actually in the img, but the file is present
            back: 71, //called left above but the file is 'back.png'
            add: 72, //called left above but the file is 'back.png'
            search: 73 //called ?? above but the file is 'back.png'
        });
    
    Csw.enums.searchCssClasses = Csw.enums.searchCssClasses ||
        Csw.enums.register('searchCssClasses', {
            nodetype_select: { name: 'csw_search_nodetype_select' },
            property_select: { name: 'csw_search_property_select' }
        });

    Csw.enums.appMode = Csw.enums.appMode ||
        Csw.enums.register('appMode', {
            mode: 'full'
        });

    Csw.enums.wizardSteps_ViewEditor = Csw.enums.wizardSteps_ViewEditor ||
        Csw.enums.register('wizardSteps_ViewEditor', {
            viewselect: { step: 1, description: 'Choose a View', divId: 'step1_viewselect' },
            attributes: { step: 2, description: 'Edit View Attributes', divId: 'step2_attributes' },
            relationships: { step: 3, description: 'Add Relationships', divId: 'step3_relationships' },
            properties: { step: 4, description: 'Select Properties', divId: 'step4_properties' },
            filters: { step: 5, description: 'Set Filters', divId: 'step5_filters' },
            tuning: { step: 6, description: 'Fine Tuning', divId: 'step6_tuning' }
        });

    Csw.enums.cssClasses_ViewBuilder = Csw.enums.cssClasses_ViewBuilder ||
        Csw.enums.register('cssClasses_ViewBuilder', {
            conjunction_select: { name: 'csw_viewbuilder_conjunction_select' },
            subfield_select: { name: 'csw_viewbuilder_subfield_select' },
            filter_select: { name: 'csw_viewbuilder_filter_select' },
            default_filter: { name: 'csw_viewbuilder_default_filter' },
            filter_value: { name: 'csw_viewbuilder_filter_value' },
            metadatatype_static: { name: 'csw_viewbuilder_metadatatype_static' }
        });

    Csw.enums.domElementEvent = Csw.enums.domElementEvent ||
        Csw.enums.register('domElementEvent', {
            click: { name: 'click' },
            change: { name: 'change' },
            vclick: { name: 'vclick' },
            tap: { name: 'tap' }
        });

    Csw.enums.objectClasses = Csw.enums.objectClasses ||
        Csw.enums.register('objectClasses', {
            GenericClass: 'GenericClass',
            InspectionDesignClass: 'InspectionDesignClass'
        });

    Csw.enums.nodeSpecies = Csw.enums.nodeSpecies ||
        Csw.enums.register('nodeSpecies', {
            Plain: 'Plain',
            More: 'More'
        });

    Csw.enums.subFieldNames = Csw.enums.subFieldNames ||
        Csw.enums.register('subFieldNames', {
            Unknown: { name: 'unknown' },
            AllowedAnswers: { name: 'allowedanswers' },
            Answer: { name: 'answer' },
            Barcode: { name: 'barcode' },
            Blob: { name: 'blob' },
            Checked: { name: 'checked' },
            Column: { name: 'column' },
            Comments: { name: 'comments' },
            CompliantAnswers: { name: 'compliantanswers' },
            ContentType: { name: 'contenttype' },
            CorrectiveAction: { name: 'correctiveaction' },
            DateAnswered: { name: 'dateanswered' },
            DateCorrected: { name: 'datecorrected' },
            Href: { name: 'href' },
            Image: { name: 'image' },
            Interval: { name: 'interval' },
            IsCompliant: { name: 'iscompliant' },
            Mol: { name: 'mol' },
            Name: { name: 'name' },
            NodeID: { name: 'nodeid' },
            NodeType: { name: 'nodetype' },
            Number: { name: 'number' },
            Password: { name: 'password' },
            Path: { name: 'path' },
            Required: { name: 'required' },
            Row: { name: 'row' },
            Sequence: { name: 'sequence' },
            StartDateTime: { name: 'startdatetime' },
            Text: { name: 'text' },
            Units: { name: 'units' },
            Value: { name: 'value' },
            ViewID: { name: 'viewid' },
            ChangedDate: { name: 'changeddate' },
            Base: { name: 'base' },
            Exponent: { name: 'exponent' }
        });

    Csw.enums.subFieldsMap = Csw.enums.subFieldsMap ||
        Csw.enums.register('subFieldsMap', {
            AuditHistoryGrid: { name: 'AuditHistoryGrid', subfields: {} },
            Barcode: {
                name: 'Barcode',
                subfields: {
                    Barcode: Csw.enums.subFieldNames.Barcode,
                    Sequence: Csw.enums.subFieldNames.Number
                }
            },
            Button: {
                name: 'Button',
                subfields: {
                    Text: Csw.enums.subFieldNames.Text
                }
            },
            Composite: { name: 'Composite', subfields: {} },
            DateTime: {
                name: 'DateTime',
                subfields: {
                    Value: {
                        Date: { name: 'date' },
                        Time: { name: 'time' },
                        DateFormat: { name: 'dateformat' },
                        TimeFormat: { name: 'timeformat' }
                    },
                    DisplayMode: {
                        Date: { name: 'Date' },
                        Time: { name: 'Time' },
                        DateTime: { name: 'DateTime' }
                    }
                }
            },
            File: { name: 'File', subfields: {} },
            Grid: { name: 'Grid', subfields: {} },
            Image: { name: 'Image', subfields: {} },
            Link: {
                name: 'Link',
                subfields: {
                    Text: Csw.enums.subFieldNames.Text,
                    Href: Csw.enums.subFieldNames.Href
                }
            },
            List: {
                name: 'List',
                subfields: {
                    Value: Csw.enums.subFieldNames.Value
                }
            },
            Location: { name: 'Location', subfields: {} },
            LocationContents: { name: 'LocationContents', subfields: {} },
            Logical: {
                name: 'Logical',
                subfields: {
                    Checked: Csw.enums.subFieldNames.Checked
                }
            },
            LogicalSet: { name: 'LogicalSet', subfields: {} },
            Memo: {
                name: 'Memo',
                subfields: {
                    Text: Csw.enums.subFieldNames.Text
                }
            },
            MTBF: { name: 'MTBF', subfields: {} },
            MultiList: { name: 'MultiList', subfields: {} },
            NodeTypeSelect: { name: 'NodeTypeSelect', subfields: {} },
            Number: {
                name: 'Number',
                subfields: {
                    Value: Csw.enums.subFieldNames.Value
                }
            },
            Password: {
                name: 'Password',
                subfields: {
                    Password: Csw.enums.subFieldNames.Password,
                    ChangedDate: Csw.enums.subFieldNames.ChangedDate
                }
            },
            PropertyReference: { name: 'PropertyReference', subfields: {} },
            Quantity: {
                name: 'Quantity',
                subfields: {
                    Value: Csw.enums.subFieldNames.Value,
                    Units: Csw.enums.subFieldNames.Number
                }
            },
            Question: {
                name: 'Question',
                subfields: {
                    Answer: Csw.enums.subFieldNames.Answer,
                    CorrectiveAction: Csw.enums.subFieldNames.CorrectiveAction,
                    IsCompliant: Csw.enums.subFieldNames.IsCompliant,
                    Comments: Csw.enums.subFieldNames.Comments,
                    DateAnswered: Csw.enums.subFieldNames.DateAnswered,
                    DateCorrected: Csw.enums.subFieldNames.DateCorrected
                }
            },
            Relationship: { name: 'Relationship', subfields: {} },
            Scientific: { name: 'Scientific', subfields: {} },
            Sequence: { name: 'Sequence', subfields: {} },
            Static: {
                name: 'Static',
                subfields: {
                    Text: Csw.enums.subFieldNames.Text
                }
            },
            Text: {
                name: 'Text',
                subfields: {
                    Text: Csw.enums.subFieldNames.Text
                }
            },
            TimeInterval: { name: 'TimeInterval', subfields: {} },
            UserSelect: { name: 'UserSelect', subfields: {} },
            ViewPickList: { name: 'ViewPickList', subfields: {} },
            ViewReference: { name: 'ViewReference', subfields: {} }
        });

    Csw.enums.cssClasses_ViewEdit = Csw.enums.cssClasses_ViewEdit ||
        Csw.enums.register('cssClasses_ViewEdit', {
            vieweditor_viewrootlink: { name: 'vieweditor_viewrootlink' },
            vieweditor_viewrellink: { name: 'vieweditor_viewrellink' },
            vieweditor_viewproplink: { name: 'vieweditor_viewproplink' },
            vieweditor_viewfilterlink: { name: 'vieweditor_viewfilterlink' },
            vieweditor_addfilter: { name: 'vieweditor_addfilter' },
            vieweditor_deletespan: { name: 'vieweditor_deletespan' },
            vieweditor_childselect: { name: 'vieweditor_childselect' }
        });

    Csw.enums.viewChildPropNames = Csw.enums.viewChildPropNames ||
        Csw.enums.register('viewChildPropNames', {
            root: { name: 'root' },
            childrelationships: { name: 'childrelationships' },
            properties: { name: 'properties' },
            filters: { name: 'filters' },
            propfilters: { name: 'filters' },
            filtermodes: { name: 'filtermodes' }
        });

    Csw.enums.nodeTree_DefaultSelect = Csw.enums.nodeTree_DefaultSelect ||
        Csw.enums.register('nodeTree_DefaultSelect', {
            root: { name: 'root' },
            firstchild: { name: 'firstchild' },
            none: { name: 'none' }
        });

    Csw.enums.toggleState = Csw.enums.toggleState ||
        Csw.enums.register('toggleState', {
            on: {name: 'on'},
            off: {name: 'off'}
        });

} ());
